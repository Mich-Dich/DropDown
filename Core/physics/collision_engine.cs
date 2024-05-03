using Core.game_objects;
using Core.util;
using System.Drawing;
using OpenTK;
//using System.Numerics;

using OpenTK.Mathematics;
using Core.physics;

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

        private hit_data collision_AABB_AABB(game_object AABB, game_object AABB_2) {
            hit_data hit = new hit_data();

            // Check for collision
            if (AABB.transform.position.X < AABB_2.transform.position.X + AABB_2.transform.size.X &&
                AABB.transform.position.X + AABB.transform.size.X > AABB_2.transform.position.X &&
                AABB.transform.position.Y < AABB_2.transform.position.Y + AABB_2.transform.size.Y &&
                AABB.transform.position.Y + AABB.transform.size.Y > AABB_2.transform.position.Y) {
                
                hit.is_hit = true;
                hit.hit_position = AABB.transform.position;
                hit.hit_object = AABB_2;
            }

            return hit;
        }

        public void update(List<game_object> all_objects) {
            hit_data current = new hit_data();

            for(int x = 0; x < all_objects.Count; x++) {
                for(int y = 0; y < all_objects.Count; y++) {
                    // Skip unneeded
                    if(all_objects[x] == all_objects[y] ||
                        (all_objects[x].transform.mobility == mobility.STATIC && all_objects[y].transform.mobility == mobility.STATIC))
                        continue;

                    if(all_objects[x].collider.Value.shape == collision_shape.Circle) {
                        if(all_objects[y].collider.Value.shape == collision_shape.Circle)
                            current = collision_circle_circle(all_objects[x], all_objects[y]);
                        else if(all_objects[y].collider.Value.shape == collision_shape.Square)
                            current = collision_circle_AABB(all_objects[x], all_objects[y]);
                    }
                    else if(all_objects[x].collider.Value.shape == collision_shape.Square) {
                        if(all_objects[y].collider.Value.shape == collision_shape.Circle)
                            current = collision_AABB_circle(all_objects[x], all_objects[y]);
                        else if(all_objects[y].collider.Value.shape == collision_shape.Square)
                            current = collision_AABB_AABB(all_objects[x], all_objects[y]);
                    }

                    if(current.is_hit) {
                        // Call hit on game_object
                        all_objects[x].hit(current);
                    }
                }
            }
        }

        private hit_data collision_circle_circle(game_object circle1, game_object circle2) {
            hit_data hit = new hit_data();

            float distance = (circle1.transform.position - circle2.transform.position).Length;
            if (distance < circle1.transform.size.X + circle2.transform.size.X) {
                hit.is_hit = true;
                hit.hit_position = circle1.transform.position;
                hit.hit_object = circle2;
            }

            return hit;
        }

        private hit_data collision_circle_AABB(game_object circle, game_object AABB) {
            hit_data hit = new hit_data();

            Vector2 circleDistance = new Vector2(Math.Abs(circle.transform.position.X - AABB.transform.position.X), Math.Abs(circle.transform.position.Y - AABB.transform.position.Y));

            if (circleDistance.X > (AABB.transform.size.X/2 + circle.transform.size.X)) { return hit; }
            if (circleDistance.Y > (AABB.transform.size.Y/2 + circle.transform.size.Y)) { return hit; }

            if (circleDistance.X <= (AABB.transform.size.X/2)) {
                hit.is_hit = true;
                hit.hit_position = circle.transform.position;
                hit.hit_object = AABB;
                return hit;
            } 
            if (circleDistance.Y <= (AABB.transform.size.Y/2)) {
                hit.is_hit = true;
                hit.hit_position = circle.transform.position;
                hit.hit_object = AABB;
                return hit;
            }

            float cornerDistance_sq = (float)(Math.Pow(circleDistance.X - AABB.transform.size.X/2, 2) +
                                  Math.Pow(circleDistance.Y - AABB.transform.size.Y/2, 2));

            if (cornerDistance_sq <= Math.Pow(circle.transform.size.X, 2)) {
                hit.is_hit = true;
                hit.hit_position = circle.transform.position;
                hit.hit_object = AABB;
            }

            return hit;
        }

        private hit_data collision_AABB_circle(game_object AABB, game_object circle) {
            return collision_circle_AABB(circle, AABB);
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
