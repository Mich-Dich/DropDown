using Core;
using Core.controllers.ai;
using Core.game_objects;
using Core.util;
using Core.physics;
using OpenTK.Mathematics;

namespace Hell {

    public class enemy_controller : ai_controller {

        public enemy_controller(float cooldown = 3) {
            
            set_statup_state(new AIS_idle());
        }

    }

    public class AIS_idle : I_AI_state {

        private float offset;
        private float lastFire = 0;
        private float fireCooldown = 3;
        
        public Type execute(ai_controller ai_controller)
        {
            if(!ai_controller.character.is_in_range(game.instance.player, 200) && ai_controller.character.is_in_range(game.instance.player, 1000)) {
                return typeof(AIS_seek);
            }

            if(game_time.total - lastFire > fireCooldown) {
                lastFire = game_time.total;
                return typeof(AIS_fire);
            }

            float side = MathF.Sin(game_time.total + offset * 2.5f);
            float up = MathF.Sin((game_time.total + offset * 1.5f) * 4.0f);
            ai_controller.character.set_velocity(new Vector2(150 * side, 60 * up));
            return typeof(AIS_idle);
        }

        public Type exit(ai_controller ai_controller)
        {
            return typeof(AIS_idle);
        }

        public Type enter(ai_controller ai_controller)
        {
            offset = game_time.total * 10;

            return typeof(AIS_idle);
        }

    }

    public class AIS_seek : I_AI_state {

        public Type execute(ai_controller ai_controller)
        {
            if(ai_controller.character.is_in_range(game.instance.player, 100)) {
                return typeof(AIS_idle);
            }
            Vector2 direction = game.instance.player.transform.position - ai_controller.character.transform.position;
            direction.Normalize();
            ai_controller.character.set_velocity(direction * 300);
            return typeof(AIS_seek);
        }

        public Type exit(ai_controller ai_controller)
        {
            ai_controller.character.set_velocity(new Vector2());
            return typeof(AIS_seek);
        }

        public Type enter(ai_controller ai_controller)
        {
            return typeof(AIS_seek);
        }

    }

    public class AIS_fire : I_AI_state {

        public Type execute(ai_controller ai_controller)
        {
            Vector2 pos = ai_controller.character.transform.position;
            Vector2 dir = game.instance.player.transform.position - pos;
            dir.Normalize();
            var proj = new projectile(pos + 2.0f * dir, dir, 300, 2, new Vector2(10, 10), 10);
            // Set the collision type of the projectile's collider to enemy_bullet
            proj.collider.type = collision_type.enemy_bullet;
            game.instance.active_map.add_game_object(proj);
            return typeof(AIS_idle);
        }

        public Type exit(ai_controller ai_controller)
        {
            return typeof(AIS_fire);
        }

        public Type enter(ai_controller ai_controller)
        {
            return typeof(AIS_fire);
        }
    }
}