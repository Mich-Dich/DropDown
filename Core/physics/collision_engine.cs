using Core.game_objects;
using ImGuiNET;
using OpenTK.Mathematics;

namespace Core.physics {

    public struct hit_data {

        public bool is_hit;
        public Vector2 hit_position;
        public Vector2 hit_direction;
        public Vector2 hit_normal;
        public Vector2 hit_impact_point;
        public game_object hit_object;
    }

    // first game_object is the main object to consider, the second is the compare and will be set as the hit_object
    public class collision_engine {

        public collision_engine() { }

        public void update(List<game_object> all_objects, float delta_time, float min_distanc_for_collision = 1600) {

            if(game.instance.show_debug)
                debug_data.colidable_objects = all_objects.Count;

            hit_data current = new hit_data();

            for (int x = 0; x < all_objects.Count; x++) {

                var obj_X = all_objects[x];

                if(obj_X.collider == null)
                    continue;       // early skip for colliderless objects

                // update position
                if(obj_X.transform.mobility != mobility.STATIC) {

                    obj_X.collider.velocity /= 1 + obj_X.collider.material.friction * delta_time;
                    obj_X.transform.position += obj_X.collider.velocity * delta_time;
                }

                for(int y = 1+x; y < all_objects.Count; y++) {

                    var obj_Y = all_objects[y];

                    if(obj_Y.collider == null)        // both objects are colliderless
                        continue;

                    // Skip unneeded
                    if(obj_X.transform.mobility == mobility.STATIC && obj_Y.transform.mobility == mobility.STATIC)
                        continue;

                    if((obj_X.transform.position - obj_Y.transform.position).LengthFast > min_distanc_for_collision)
                        continue;


                    if(game.instance.show_debug) {

                        debug_data.collision_checks_num++;
                        if(obj_Y.transform.mobility == mobility.STATIC)
                            debug_data.colidable_objects_static++;
                        else
                            debug_data.colidable_objects_dynamic++;
                    }

                    if(obj_X.collider.shape == collision_shape.Circle) {
                        if(obj_Y.collider.shape == collision_shape.Circle)
                            current = collision_circle_circle(obj_X, obj_Y);
                        else if(obj_Y.collider.shape == collision_shape.Square)
                            current = collision_circle_AABB(obj_X, obj_Y);
                    }
                    else if(obj_X.collider.shape == collision_shape.Square) {
                        if(obj_Y.collider.shape == collision_shape.Circle)
                            current = collision_circle_AABB(obj_Y, obj_X);
                        else if(obj_Y.collider.shape == collision_shape.Square)
                            current = collision_AABB_AABB(obj_X, obj_Y);
                    }


                    // early exit
                    if(!current.is_hit)
                        continue;

                    // proccess hit => change position, velocity ...
                    float total_mass = obj_X.collider.mass + obj_Y.collider.mass;
                    if(obj_X.transform.mobility != mobility.STATIC || obj_X.collider.offset.mobility != mobility.STATIC) {

                        obj_X.transform.position -= current.hit_direction; // * (obj_Y.collider.mass / total_mass) * obj_Y.collider.material.bounciness;
                        obj_X.collider.velocity -= current.hit_direction * (obj_Y.collider.mass / total_mass);// * obj_Y.collider.material.bounciness;
                        obj_X.hit(current);
                    }

                    if(obj_Y.transform.mobility != mobility.STATIC || obj_Y.collider.offset.mobility != mobility.STATIC) {

                        obj_Y.transform.position += current.hit_direction;// * (obj_X.collider.mass / total_mass) * obj_X.collider.material.bounciness;
                        obj_Y.collider.velocity += current.hit_direction * (obj_X.collider.mass / total_mass);// * obj_X.collider.material.bounciness;
                        obj_Y.hit(current);
                    }
                }
            }
        }

        private hit_data collision_AABB_AABB(game_object AABB, game_object AABB_2) {
            hit_data hit = new hit_data();

            Vector2 posA = AABB.transform.position + AABB.collider.offset.position;
            Vector2 posB = AABB_2.transform.position + AABB_2.collider.offset.position;

            float overlapX = (AABB.transform.size.X / 2 + AABB_2.transform.size.X / 2) - Math.Abs(posA.X - posB.X);
            float overlapY = (AABB.transform.size.Y / 2 + AABB_2.transform.size.Y / 2) - Math.Abs(posA.Y - posB.Y);

            if (overlapX > 0 && overlapY > 0) {
                hit.is_hit = true;
                hit.hit_position = posA;
                hit.hit_object = AABB_2;

                if (AABB.collider.Blocking && AABB_2.collider.Blocking) {
                    if (overlapX < overlapY) {
                        hit.hit_direction = new Vector2(Math.Sign(posB.X - posA.X) * overlapX, 0);
                    } else {
                        hit.hit_direction = new Vector2(0, Math.Sign(posB.Y - posA.Y) * overlapY);
                    }
                } else {
                    hit.hit_direction = Vector2.Zero;
                }
            }

            return hit;
        }

        private hit_data collision_circle_circle(game_object circle1, game_object circle2) {
            hit_data hit = new hit_data();

            Vector2 posA = circle1.transform.position + circle1.collider.offset.position;
            Vector2 posB = circle2.transform.position + circle2.collider.offset.position;

            Vector2 intersection_direction = posA - posB;
            float radiusSum = (circle1.transform.size.X / 2) + (circle2.transform.size.X / 2);

            if (intersection_direction.Length < radiusSum) {
                float overlap = radiusSum - intersection_direction.Length;

                hit.is_hit = true;
                hit.hit_position = posA;
                hit.hit_object = circle2;

                if (circle1.collider.Blocking && circle2.collider.Blocking) {
                    hit.hit_direction = intersection_direction.Normalized() * overlap;
                } else {
                    hit.hit_direction = Vector2.Zero;
                }
            }

            return hit;
        }

        private hit_data collision_circle_AABB(game_object circle, game_object AABB) {

            hit_data hit = new hit_data();

            Vector2 posA = circle.transform.position + circle.collider.offset.position;
            Vector2 posB = AABB.transform.position + AABB.collider.offset.position;

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

            if (circle.collider.Blocking && AABB.collider.Blocking) {
                hit.hit_direction = centerToNearest.Normalized() * overlap;
            } else {
                hit.hit_direction = Vector2.Zero;
            }

            hit.is_hit = true;
            hit.hit_position = posA;
            hit.hit_object = AABB;
            return hit;
        }


        /*
        // --------------------------------------- static - static --------------------------------------- 

        // needed for pre-play calculations: e.g. level-generation
        public hit_data static_collision_AABB_AABB(game_object AABB, game_object AABB_2) {

            hit_data hit = new hit_data();

            if(!(AABB.position.X                    < AABB_2.position.X + AABB_2.size.X 
                && AABB.position.X + AABB.size.X    > AABB_2.position.X 
                && AABB.position.Y                  < AABB_2.position.Y + AABB_2.size.Y 
                && AABB.position.Y + AABB.size.Y    > AABB_2.position.Y))
                return hit;

            hit.is_hit = true;
            hit.hit_position = AABB.position;
            hit.hit_object = AABB_2;
            return hit;
        }

        public hit_data static_collision_circle_circle(game_object circle, game_object circle_2) {

            hit_data hit = new hit_data();
            if(((circle.position - circle_2.position).Length) >= (circle.size.X + circle_2.size.X))
                return hit;

            hit.is_hit = true;
            hit.hit_position = circle.position;
            hit.hit_object = circle_2;
            return hit;
        }

        public hit_data static_collision_circle_AABB(game_object circle, game_object AABB) {

            hit_data hit = new hit_data();

            float deltaX = circle.position.X - Math.Max(AABB.position.X, Math.Min(circle.position.X, AABB.position.X + AABB.size.X));
            float deltaY = circle.position.Y - Math.Max(AABB.position.Y, Math.Min(circle.position.Y, AABB.position.Y + AABB.size.Y));
            float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);
            if(distanceSquared >= (circle.size.X * circle.size.X))
                return hit;

            hit.is_hit = true;
            hit.hit_position = circle.position;
            hit.hit_object = AABB;
            return hit;
        }

        public void update(List<game_object> all_objects) {


            // FOR ALL OBJECTS [x] {
            //     FOR ALL OTHER OBJECTS [y] {
            // 
            //         1) call collision function based on mobility_type of [x] and [y] and save in hit variable
            // 
            //         2) use hit var & (physics_material of [y] & [y]) to update position and velocity
            // 
            //         3) adjust velocity besed on mobility (make new Velocity)
            //              STATIC can never move
            //              MOVABLE can move but is mosty static
            //              DYNAMIC can move every frame
            // 
            //         4) update (position & velocity) of game_object [x]
            // 
            //         5) obj_X.hit(loc_hit);   // call hit on game_object (this is already real code)
            //     }
            // }

            hit_data curent = new hit_data();

            for(int x = 0; x < all_objects.Count; x++) {
                for(int y = 0; y < all_objects.Count; y++) {

                    // skip unneeded
                    if(obj_X == obj_Y ||
                        (obj_X.mobility == mobility.STATIC && obj_Y.mobility == mobility.STATIC))
                        break;

                    if(obj_X.shape == primitive.CIRCLE) {

                        if(obj_Y.shape == primitive.CIRCLE)
                            curent = collision_circle_circle(obj_X, obj_Y);

                        else if(obj_X.shape == primitive.SQUARE)
                            curent = collision_circle_AABB(obj_X, obj_Y);
                    }

                    else if(obj_X.shape == primitive.SQUARE) {

                        if(obj_Y.shape == primitive.CIRCLE)
                            curent = collision_circle_AABB(obj_X, obj_Y);

                        else if(obj_X.shape == primitive.SQUARE)
                            curent = collision_AABB_AABB(obj_X, obj_Y);
                    }


                    if(curent.is_hit) {

                        if((obj_X.mobility == mobility.DYNAMIC || obj_X.mobility == mobility.MOVABLE) && (obj_Y.mobility == mobility.DYNAMIC || obj_Y.mobility == mobility.MOVABLE)) {

                            obj_X.position = obj_X.position - (curent.hit_normal / 2); // velocity fehlt noch
                            obj_Y.position = obj_Y.position + (curent.hit_normal / 2); // velocity fehlt noch
                        }

                        if((obj_X.mobility == mobility.DYNAMIC || obj_X.mobility == mobility.MOVABLE) && obj_Y.mobility == mobility.STATIC)
                            obj_X.position = obj_X.position - curent.hit_normal; // velocity fehlt noch

                        if(obj_X.mobility == mobility.STATIC && (obj_Y.mobility == mobility.DYNAMIC || obj_Y.mobility == mobility.MOVABLE))
                            obj_Y.position = obj_Y.position + curent.hit_normal; // velocity fehlt noch

                    }


                }
            }
        }

        // ======================================= private =======================================

        // --------------------------------------- dynamic - dynamic --------------------------------------- 
        private hit_data collision_AABB_AABB(game_object AABB, game_object AABB_2) {

            hit_data hit = new hit_data();

            return hit;
        }


        private hit_data collision_circle_circle(game_object circle, game_object circle_2) {

            hit_data hit = new hit_data();

            return hit;
        }


        private hit_data collision_circle_AABB(game_object circle, game_object AABB) {

            hit_data hit = new hit_data();

            return hit;
        }


        private hit_data collision_AABB_circle(game_object AABB, game_object circle) {

            hit_data hit = new hit_data();

            return hit;
        }

        // --------------------------------------- static - dynamic --------------------------------------- 
        private hit_data collision_static_AABB_dynamic_AABB(game_object AABB, game_object AABB_2) {

            hit_data hit = new hit_data();

            return hit;
        }

        private hit_data collision_static_circle_dynamic_circle(game_object circle, game_object circle_2) {

            hit_data hit = new hit_data();

            return hit;
        }


        private hit_data collision_static_circle_dynamic_AABB(game_object circle, game_object AABB) {

            hit_data hit = new hit_data();

            Vector2 nextRectanglePosition = AABB.position + AABB.velocity * game_time.elapsed.Seconds;

            float distanceX = Math.Abs(circle.position.X - nextRectanglePosition.X);
            float distanceY = Math.Abs(circle.position.Y - nextRectanglePosition.Y);

            float closestX = Math.Max(nextRectanglePosition.X - AABB.size.X / 2, Math.Min(circle.position.X, nextRectanglePosition.X + AABB.size.X / 2));
            float closestY = Math.Max(nextRectanglePosition.Y - AABB.size.Y / 2, Math.Min(circle.position.Y, nextRectanglePosition.Y + AABB.size.Y / 2));

            float distance = (new Vector2(closestX, closestY) - circle.position).Length;

            if(distance <= circle.size.X) {
                hit.is_hit = true;
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
