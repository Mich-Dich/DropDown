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

        public void update(List<game_object> all_objects, float delta_time) {

            if(game.instance.show_debug)
                debug_data.colidable_objects = all_objects.Count;

            List<game_object> finished_objects = new List<game_object>();

            hit_data current = new hit_data();

            for (int x = 0; x < all_objects.Count; x++) {
                for(int y = 0; y < all_objects.Count; y++) {


                    // update position
                    if((all_objects[x].collider != null) && (all_objects[x].transform.mobility != mobility.STATIC) && (!finished_objects.Contains(all_objects[y]))) {
                        all_objects[x].collider.velocity /= 1 + all_objects[x].collider.material.friction;
                        all_objects[x].transform.position += all_objects[x].collider.velocity * delta_time;
                    }

                    // Skip unneeded
                    if(all_objects[x] == all_objects[y] ||
                        (all_objects[x].transform.mobility == mobility.STATIC && all_objects[y].transform.mobility == mobility.STATIC))
                        continue;

                    if(all_objects[x].collider == null || all_objects[y].collider == null || finished_objects.Contains(all_objects[y]))
                        continue;


                    if(game.instance.show_debug)
                        debug_data.collision_checks_num++;

                    if(all_objects[x].collider.shape == collision_shape.Circle) {
                        if(all_objects[y].collider.shape == collision_shape.Circle)
                            current = collision_circle_circle(all_objects[x], all_objects[y]);
                        else if(all_objects[y].collider.shape == collision_shape.Square)
                            current = collision_circle_AABB(all_objects[x], all_objects[y]);
                    }
                    else if(all_objects[x].collider.shape == collision_shape.Square) {
                        if(all_objects[y].collider.shape == collision_shape.Circle)
                            current = collision_circle_AABB(all_objects[y], all_objects[x]);
                        else if(all_objects[y].collider.shape == collision_shape.Square)
                            current = collision_AABB_AABB(all_objects[x], all_objects[y]);
                    }

                    finished_objects.Add(all_objects[x]);

                    // early exit
                    if(!current.is_hit)
                        continue;


                    // proccess hit => change position, velocity ...
                    float total_mass = all_objects[x].collider.mass + all_objects[y].collider.mass;
                    if(all_objects[x].transform.mobility != mobility.STATIC) {

                        all_objects[x].transform.position -= current.hit_direction; // * (all_objects[y].collider.mass / total_mass) * all_objects[y].collider.material.bounciness;
                        all_objects[x].collider.velocity -= current.hit_direction * (all_objects[y].collider.mass / total_mass);// * all_objects[y].collider.material.bounciness;
                        all_objects[x].hit(current);
                    }

                    if(all_objects[y].transform.mobility != mobility.STATIC) {

                        all_objects[y].transform.position += current.hit_direction;// * (all_objects[x].collider.mass / total_mass) * all_objects[x].collider.material.bounciness;
                        all_objects[y].collider.velocity += current.hit_direction * (all_objects[x].collider.mass / total_mass);// * all_objects[x].collider.material.bounciness;
                        all_objects[y].hit(current);
                    }
                }
            }
        }

        private hit_data collision_AABB_AABB(game_object AABB, game_object AABB_2) {
            hit_data hit = new hit_data();
            Vector2 direction = AABB.transform.position - AABB_2.transform.position;

            float overlapX = AABB.transform.size.X / 2 + AABB_2.transform.size.X / 2 - Math.Abs(direction.X);
            float overlapY = AABB.transform.size.Y / 2 + AABB_2.transform.size.Y / 2 - Math.Abs(direction.Y);

            if (overlapX > 0 && overlapY > 0) {
                hit.is_hit = true;
                hit.hit_position = AABB.transform.position;
                hit.hit_object = AABB_2;

                Vector2 prevPosition = AABB.transform.position;

                if (overlapX < overlapY) {
                    hit.hit_direction = new Vector2(Math.Sign(direction.X), 0) * overlapX;
                } else {
                    hit.hit_direction = new Vector2(0, Math.Sign(direction.Y)) * overlapY;
                }

                AABB.transform.position += hit.hit_direction;

                Vector2 moveDirection = AABB.transform.position - prevPosition;

                if (Vector2.Dot(moveDirection, hit.hit_direction) < 0) {
                    hit.hit_direction = -hit.hit_direction;
                }
            }

            return hit;
        }


        private hit_data collision_circle_circle(game_object circle1, game_object circle2) {

            hit_data hit = new hit_data();
            Vector2 insection_direction = circle1.transform.position - circle2.transform.position;
            if (insection_direction.Length <
                ((circle1.transform.size.X / 2) + (circle1.collider.offset.size.X / 2))
                + ((circle2.transform.size.X / 2) + (circle2.collider.offset.size.X / 2))) {

                float intersect_distance = insection_direction.Length -((circle1.transform.size.X / 2) + (circle2.transform.size.X / 2));

                hit.hit_direction = insection_direction * (intersect_distance * 0.002f);
                hit.is_hit = true;
                hit.hit_position = circle1.transform.position;
                hit.hit_object = circle2;
            }

            return hit;
        }

        private hit_data collision_circle_AABB(game_object circle, game_object AABB) {

            hit_data hit = new hit_data();

            float nearest_point_x = Math.Clamp(circle.transform.position.X,
                AABB.transform.position.X - (AABB.transform.size.X / 2),
                AABB.transform.position.X + (AABB.transform.size.X / 2));

            float nearest_point_y = Math.Clamp(circle.transform.position.Y,
                AABB.transform.position.Y - (AABB.transform.size.Y / 2),
                AABB.transform.position.Y + (AABB.transform.size.Y / 2));

            Vector2 nearestPoint = new Vector2(nearest_point_x, nearest_point_y);
            Vector2 centerToNearest = nearestPoint - circle.transform.position;

            float overlap = ((circle.transform.size.X / 2) + (circle.collider.offset.size.X/2));
            if (nearestPoint != circle.transform.position)
                overlap -= centerToNearest.Length;
            
            if (overlap < 0)    // not hit
                return hit;

            hit.hit_direction = centerToNearest.Normalized() * overlap;
            hit.is_hit = true;
            hit.hit_position = circle.transform.position;
            hit.hit_object = AABB;
            return hit;
        }


        /*
        // --------------------------------------- static - static --------------------------------------- 

        // needed for pre-play calculations: e.g. level-generation
        public hit_data static_collision_AABB_AABB(game_object AABB, game_object AABB_2) {

            hit_data hit = new hit_data();

            if(!(AABB.position.X < AABB_2.position.X + AABB_2.size.X &&
               AABB.position.X + AABB.size.X > AABB_2.position.X &&
               AABB.position.Y < AABB_2.position.Y + AABB_2.size.Y &&
               AABB.position.Y + AABB.size.Y > AABB_2.position.Y))
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
            //         5) all_objects[x].hit(loc_hit);   // call hit on game_object (this is already real code)
            //     }
            // }

            hit_data curent = new hit_data();

            for(int x = 0; x < all_objects.Count; x++) {
                for(int y = 0; y < all_objects.Count; y++) {

                    // skip unneeded
                    if(all_objects[x] == all_objects[y] ||
                        (all_objects[x].mobility == mobility.STATIC && all_objects[y].mobility == mobility.STATIC))
                        break;

                    if(all_objects[x].shape == primitive.CIRCLE) {

                        if(all_objects[y].shape == primitive.CIRCLE)
                            curent = collision_circle_circle(all_objects[x], all_objects[y]);

                        else if(all_objects[x].shape == primitive.SQUARE)
                            curent = collision_circle_AABB(all_objects[x], all_objects[y]);
                    }

                    else if(all_objects[x].shape == primitive.SQUARE) {

                        if(all_objects[y].shape == primitive.CIRCLE)
                            curent = collision_circle_AABB(all_objects[x], all_objects[y]);

                        else if(all_objects[x].shape == primitive.SQUARE)
                            curent = collision_AABB_AABB(all_objects[x], all_objects[y]);
                    }


                    if(curent.is_hit) {

                        if((all_objects[x].mobility == mobility.DYNAMIC || all_objects[x].mobility == mobility.MOVABLE) && (all_objects[y].mobility == mobility.DYNAMIC || all_objects[y].mobility == mobility.MOVABLE)) {

                            all_objects[x].position = all_objects[x].position - (curent.hit_normal / 2); // velocity fehlt noch
                            all_objects[y].position = all_objects[y].position + (curent.hit_normal / 2); // velocity fehlt noch
                        }

                        if((all_objects[x].mobility == mobility.DYNAMIC || all_objects[x].mobility == mobility.MOVABLE) && all_objects[y].mobility == mobility.STATIC)
                            all_objects[x].position = all_objects[x].position - curent.hit_normal; // velocity fehlt noch

                        if(all_objects[x].mobility == mobility.STATIC && (all_objects[y].mobility == mobility.DYNAMIC || all_objects[y].mobility == mobility.MOVABLE))
                            all_objects[y].position = all_objects[y].position + curent.hit_normal; // velocity fehlt noch

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
