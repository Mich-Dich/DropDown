
namespace Core.physics {

    using Core.world;
    using OpenTK.Mathematics;

    public struct hitData {

        public bool isHit;
        public Vector2 hitPosition;
        public Vector2 hitDirection;
        public Vector2 hitNormal;
        public Vector2 hitImpactPoint;
        public Game_Object hitObject;
    }

    // first game_object is the main object to consider, the second is the compare and will be set as the hitObject
    public sealed class Physics_Engine {

        public Physics_Engine() { }

        public void Update(List<Game_Object> all_objects, float deltaTime, float minDistancForCollision = 1600) {

            if(Game.instance.show_debug)
                Debug_Data.colidableObjects = all_objects.Count;

            hitData current = new hitData();

            for (int x = 0; x < all_objects.Count; x++) {

                var obj_X = all_objects[x];

                if(obj_X.collider == null)
                    continue;       // early skip for colliderless objects

                // Update position
                if(obj_X.transform.mobility != Mobility.STATIC) {

                    obj_X.collider.velocity /= 1 + obj_X.collider.material.friction * deltaTime;
                    obj_X.transform.position += obj_X.collider.velocity * deltaTime;
                }

                for(int y = 1+x; y < all_objects.Count; y++) {

                    var obj_Y = all_objects[y];

                    if(obj_Y.collider == null)        // both objects are colliderless
                        continue;

                    // Skip unneeded
                    if(obj_X.transform.mobility == Mobility.STATIC && obj_Y.transform.mobility == Mobility.STATIC)
                        continue;

                    if((obj_X.transform.position - obj_Y.transform.position).LengthFast > minDistancForCollision)
                        continue;


                    if(Game.instance.show_debug) {

                        Debug_Data.collisionChecksNum++;
                        if(obj_Y.transform.mobility == Mobility.STATIC)
                            Debug_Data.colidableObjects_Static++;
                        else
                            Debug_Data.colidableObjects_Dynamic++;
                    }

                    if(obj_X.collider.shape == Collision_Shape.Circle) {
                        if(obj_Y.collider.shape == Collision_Shape.Circle)
                            current = Collision_Circle_Circle(obj_X, obj_Y);
                        else if(obj_Y.collider.shape == Collision_Shape.Square)
                            current = Collision_Circle_AABB(obj_X, obj_Y);
                    }
                    else if(obj_X.collider.shape == Collision_Shape.Square) {
                        if(obj_Y.collider.shape == Collision_Shape.Circle)
                            current = Collision_Circle_AABB(obj_Y, obj_X);
                        else if(obj_Y.collider.shape == Collision_Shape.Square)
                            current = Collision_AABB_AABB(obj_X, obj_Y);
                    }


                    // early exit
                    if(!current.isHit)
                        continue;

                    // proccess hit => change position, velocity ...
                    float total_mass = obj_X.collider.mass + obj_Y.collider.mass;
                    if(obj_X.transform.mobility != Mobility.STATIC || obj_X.collider.offset.mobility != Mobility.STATIC) {

                        obj_X.transform.position -= current.hitDirection; // * (obj_Y.collider.mass / total_mass) * obj_Y.collider.material.bounciness;
                        obj_X.collider.velocity -= current.hitDirection * (obj_Y.collider.mass / total_mass);// * obj_Y.collider.material.bounciness;
                        obj_X.Hit(current);
                    }

                    if(obj_Y.transform.mobility != Mobility.STATIC || obj_Y.collider.offset.mobility != Mobility.STATIC) {

                        obj_Y.transform.position += current.hitDirection;// * (obj_X.collider.mass / total_mass) * obj_X.collider.material.bounciness;
                        obj_Y.collider.velocity += current.hitDirection * (obj_X.collider.mass / total_mass);// * obj_X.collider.material.bounciness;
                        obj_Y.Hit(current);
                    }
                }
            }
        }

        private hitData Collision_AABB_AABB(Game_Object AABB, Game_Object AABB_2) {
            hitData hit = new hitData();

#pragma warning disable CS8602 // Dereference of a possibly null reference.     => private function is only called after verifying both Game_Objects have a collider
            Vector2 posA = AABB.transform.position + AABB.collider.offset.position;
            Vector2 posB = AABB_2.transform.position + AABB_2.collider.offset.position;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            float overlapX = (AABB.transform.size.X / 2 + AABB_2.transform.size.X / 2) - Math.Abs(posA.X - posB.X);
            float overlapY = (AABB.transform.size.Y / 2 + AABB_2.transform.size.Y / 2) - Math.Abs(posA.Y - posB.Y);

            if (overlapX > 0 && overlapY > 0) {
                hit.isHit = true;
                hit.hitPosition = posA;
                hit.hitObject = AABB_2;

                if (AABB.collider.blocking && AABB_2.collider.blocking) {
                    if (overlapX < overlapY) {
                        hit.hitDirection = new Vector2(Math.Sign(posB.X - posA.X) * overlapX, 0);
                    } else {
                        hit.hitDirection = new Vector2(0, Math.Sign(posB.Y - posA.Y) * overlapY);
                    }
                } else {
                    hit.hitDirection = Vector2.Zero;
                }
            }

            return hit;
        }

        private hitData Collision_Circle_Circle(Game_Object circle1, Game_Object circle2) {
            hitData hit = new hitData();

#pragma warning disable CS8602 // Dereference of a possibly null reference.     => private function is only called after verifying both Game_Objects have a collider
            Vector2 posA = circle1.transform.position + circle1.collider.offset.position;
            Vector2 posB = circle2.transform.position + circle2.collider.offset.position;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            Vector2 intersection_direction = posA - posB;
            float radiusSum = (circle1.transform.size.X / 2) + (circle2.transform.size.X / 2);

            if (intersection_direction.Length < radiusSum) {
                float overlap = radiusSum - intersection_direction.Length;

                hit.isHit = true;
                hit.hitPosition = posA;
                hit.hitObject = circle2;

                if (circle1.collider.blocking && circle2.collider.blocking) {
                    hit.hitDirection = intersection_direction.Normalized() * overlap;
                } else {
                    hit.hitDirection = Vector2.Zero;
                }
            }

            return hit;
        }

        private hitData Collision_Circle_AABB(Game_Object circle, Game_Object AABB) {

            hitData hit = new hitData();

#pragma warning disable CS8602 // Dereference of a possibly null reference.     => private function is only called after verifying both Game_Objects have a collider
            Vector2 posA = circle.transform.position + circle.collider.offset.position;
            Vector2 posB = AABB.transform.position + AABB.collider.offset.position;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            float nearest_point_x = Math.Clamp(posA.X,
                posB.X - (AABB.transform.size.X / 2),
                posB.X + (AABB.transform.size.X / 2));

            float nearest_point_y = Math.Clamp(posA.Y,
                posB.Y - (AABB.transform.size.Y / 2),
                posB.Y + (AABB.transform.size.Y / 2));

            Vector2 nearestPoint = new Vector2(nearest_point_x, nearest_point_y);
            Vector2 centerToNearest = nearestPoint - posA;

            float overlap = ((circle.transform.size.X / 2) + (circle.collider.offset.size.X/2));
            if (nearestPoint != posA)
                overlap -= centerToNearest.Length;
            
            if (overlap < 0)    // not hit
                return hit;

            if (circle.collider.blocking && AABB.collider.blocking) {
                hit.hitDirection = centerToNearest.Normalized() * -overlap;
            } else {
                hit.hitDirection = Vector2.Zero;
            }

            hit.isHit = true;
            hit.hitPosition = posA;
            hit.hitObject = AABB;
            return hit;
        }


        /*
        // --------------------------------------- static - static --------------------------------------- 

        // needed for pre-play calculations: e.g. level-generation
        public hitData static_Collision_AABB_AABB(game_object AABB, game_object AABB_2) {

            hitData hit = new hitData();

            if(!(AABB.position.X                    < AABB_2.position.X + AABB_2.size.X 
                && AABB.position.X + AABB.size.X    > AABB_2.position.X 
                && AABB.position.Y                  < AABB_2.position.Y + AABB_2.size.Y 
                && AABB.position.Y + AABB.size.Y    > AABB_2.position.Y))
                return hit;

            hit.isHit = true;
            hit.hitPosition = AABB.position;
            hit.hitObject = AABB_2;
            return hit;
        }

        public hitData static_Collision_Circle_Circle(game_object circle, game_object circle_2) {

            hitData hit = new hitData();
            if(((circle.position - circle_2.position).Length) >= (circle.size.X + circle_2.size.X))
                return hit;

            hit.isHit = true;
            hit.hitPosition = circle.position;
            hit.hitObject = circle_2;
            return hit;
        }

        public hitData static_Collision_Circle_AABB(game_object circle, game_object AABB) {

            hitData hit = new hitData();

            float deltaX = circle.position.X - Math.Max(AABB.position.X, Math.Min(circle.position.X, AABB.position.X + AABB.size.X));
            float deltaY = circle.position.Y - Math.Max(AABB.position.Y, Math.Min(circle.position.Y, AABB.position.Y + AABB.size.Y));
            float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);
            if(distanceSquared >= (circle.size.X * circle.size.X))
                return hit;

            hit.isHit = true;
            hit.hitPosition = circle.position;
            hit.hitObject = AABB;
            return hit;
        }

        public void Update(List<game_object> all_objects) {


            // FOR ALL OBJECTS [x] {
            //     FOR ALL OTHER OBJECTS [y] {
            // 
            //         1) call collision function based on mobility_type of [x] and [y] and save in hit variable
            // 
            //         2) Use hit var & (Physics_Material of [y] & [y]) to Update position and velocity
            // 
            //         3) adjust velocity besed on mobility (make new Velocity)
            //              STATIC can never move
            //              MOVABLE can move but is mosty static
            //              DYNAMIC can move every frame
            // 
            //         4) Update (position & velocity) of game_object [x]
            // 
            //         5) obj_X.hit(loc_hit);   // call hit on game_object (this is already real code)
            //     }
            // }

            hitData curent = new hitData();

            for(int x = 0; x < all_objects.Count; x++) {
                for(int y = 0; y < all_objects.Count; y++) {

                    // skip unneeded
                    if(obj_X == obj_Y ||
                        (obj_X.mobility == mobility.STATIC && obj_Y.mobility == mobility.STATIC))
                        break;

                    if(obj_X.shape == primitive.CIRCLE) {

                        if(obj_Y.shape == primitive.CIRCLE)
                            curent = Collision_Circle_Circle(obj_X, obj_Y);

                        else if(obj_X.shape == primitive.SQUARE)
                            curent = Collision_Circle_AABB(obj_X, obj_Y);
                    }

                    else if(obj_X.shape == primitive.SQUARE) {

                        if(obj_Y.shape == primitive.CIRCLE)
                            curent = Collision_Circle_AABB(obj_X, obj_Y);

                        else if(obj_X.shape == primitive.SQUARE)
                            curent = Collision_AABB_AABB(obj_X, obj_Y);
                    }


                    if(curent.isHit) {

                        if((obj_X.mobility == mobility.DYNAMIC || obj_X.mobility == mobility.MOVABLE) && (obj_Y.mobility == mobility.DYNAMIC || obj_Y.mobility == mobility.MOVABLE)) {

                            obj_X.position = obj_X.position - (curent.hitNormal / 2); // velocity fehlt noch
                            obj_Y.position = obj_Y.position + (curent.hitNormal / 2); // velocity fehlt noch
                        }

                        if((obj_X.mobility == mobility.DYNAMIC || obj_X.mobility == mobility.MOVABLE) && obj_Y.mobility == mobility.STATIC)
                            obj_X.position = obj_X.position - curent.hitNormal; // velocity fehlt noch

                        if(obj_X.mobility == mobility.STATIC && (obj_Y.mobility == mobility.DYNAMIC || obj_Y.mobility == mobility.MOVABLE))
                            obj_Y.position = obj_Y.position + curent.hitNormal; // velocity fehlt noch

                    }


                }
            }
        }

        // ======================================= private =======================================

        // --------------------------------------- dynamic - dynamic --------------------------------------- 
        private hitData Collision_AABB_AABB(game_object AABB, game_object AABB_2) {

            hitData hit = new hitData();

            return hit;
        }


        private hitData Collision_Circle_Circle(game_object circle, game_object circle_2) {

            hitData hit = new hitData();

            return hit;
        }


        private hitData Collision_Circle_AABB(game_object circle, game_object AABB) {

            hitData hit = new hitData();

            return hit;
        }


        private hitData collision_AABB_circle(game_object AABB, game_object circle) {

            hitData hit = new hitData();

            return hit;
        }

        // --------------------------------------- static - dynamic --------------------------------------- 
        private hitData collision_static_AABB_dynamic_AABB(game_object AABB, game_object AABB_2) {

            hitData hit = new hitData();

            return hit;
        }

        private hitData collision_static_circle_dynamic_circle(game_object circle, game_object circle_2) {

            hitData hit = new hitData();

            return hit;
        }


        private hitData collision_static_circle_dynamic_AABB(game_object circle, game_object AABB) {

            hitData hit = new hitData();

            Vector2 nextRectanglePosition = AABB.position + AABB.velocity * game_time.elapsed.Seconds;

            float distanceX = Math.Abs(circle.position.X - nextRectanglePosition.X);
            float distanceY = Math.Abs(circle.position.Y - nextRectanglePosition.Y);

            float closestX = Math.Max(nextRectanglePosition.X - AABB.size.X / 2, Math.Min(circle.position.X, nextRectanglePosition.X + AABB.size.X / 2));
            float closestY = Math.Max(nextRectanglePosition.Y - AABB.size.Y / 2, Math.Min(circle.position.Y, nextRectanglePosition.Y + AABB.size.Y / 2));

            float distance = (new Vector2(closestX, closestY) - circle.position).Length;

            if(distance <= circle.size.X) {
                hit.isHit = true;
                return hit;
            }

            return hit;
        }


        private static bool LineIntersectsLine(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End) {

            Vector2 direction1 = line1End - line1Start;
            Vector2 direction2 = line2End - line2Start;

            float distance = direction2.Y * direction1.X - direction2.X * direction1.Y;

            if(distance == 0)
                return false;

            float distance1 = ((line2Start.X - line1Start.X) * direction2.Y - (line2Start.Y - line1Start.Y) * direction2.X) / distance;
            float distance2 = ((line2Start.X - line1Start.X) * direction1.Y - (line2Start.Y - line1Start.Y) * direction1.X) / distance;


            if(distance1 >= 0 && distance1 <= 1 && distance2 >= 0 && distance2 <= 1)
                return true;

            return false;
        }
        */
    }
}
