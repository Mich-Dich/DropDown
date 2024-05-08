
namespace Hell {

    using Core.controllers.player;
    using Core.input;
    using OpenTK.Mathematics;
    using System.Collections.Generic;

    public class PC_default : player_controller {

        public action move { get; set; }
        public action look { get; set; }
        public action shoot { get; set; }

        public PC_default() {

            actions.Clear();

            move = new action(
                "move",
                (uint)action_modefier_flags.auto_reset,
                false,
                action_type.VEC_2D,
                0f,
                new List<key_binding_detail> {

                    //new key_binding_detail(key_code.W, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_2 | key_modefier_flags.negate),
                    //new key_binding_detail(key_code.S, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_2),
                    new key_binding_detail(key_code.D, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_1),
                    new key_binding_detail(key_code.A, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_1 | key_modefier_flags.negate),
                });
            add_input_action(move);


            look = new action(
                "look",
                (uint)action_modefier_flags.none,
                false,
                action_type.VEC_1D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.MouseWheelY, reset_flags.reset_on_key_move_up, trigger_flags.mouse_pos_and_neg),
                });
            add_input_action(look);


            shoot = new action(
                "shoot",
                (uint)action_modefier_flags.none,
                false,
                action_type.BOOL,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.Space, reset_flags.reset_on_key_up, trigger_flags.key_down),
                });
        }

        protected override void update(float delta_time) {
            if(move.X != 0 || move.Y != 0)
                character.add_velocity(Vector2.NormalizeFast((Vector2)move.get_value()) * character.movement_speed * delta_time);
            //Console.WriteLine($"velocity:" + character.collider.velocity);
        }


        public void Shoot() {
            /*
            // Assuming shoot is a method that returns a bool
            if (shoot()) 
            {
                Vector2 projectilePosition = player.transform.position;
                Vector2 projectileDirection = new Vector2(1, 0);
                float projectileSpeed = 10.0f;
                float projectileLifeTime = 5.0f;

                // Assuming mapInstance is an instance of the map class
                projectile newProjectile = new projectile(projectilePosition, projectileDirection, projectileSpeed, projectileLifeTime, mapInstance);

                mapInstance.add_game_object(newProjectile);
            }
            */
        }
    }
}