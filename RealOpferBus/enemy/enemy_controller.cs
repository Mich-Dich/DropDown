using Core;
using Core.Controllers.ai;
using Core.game_objects;
using Core.util;
using Core.physics;
using OpenTK.Mathematics;

namespace Hell {

    public class enemy_controller : aI_Controller {

        public enemy_controller(float cooldown = 3) {
            
            set_statup_state(new AIS_idle());
        }

    }

    public class AIS_idle : I_AI_state {

        private float offset;
        private float lastFire = 0;
        private float fireCooldown = 3;
        
        public Type execute(aI_Controller aI_Controller)
        {
            if(!aI_Controller.character.is_in_range(Game.Instance.player, 200) && aI_Controller.character.is_in_range(Game.Instance.player, 1000)) {
                return typeof(AIS_seek);
            }

            if(game_time.total - lastFire > fireCooldown) {
                lastFire = game_time.total;
                return typeof(AIS_fire);
            }

            float side = MathF.Sin(game_time.total + offset * 2.5f);
            float up = MathF.Sin((game_time.total + offset * 1.5f) * 4.0f);
            aI_Controller.character.set_velocity(new Vector2(150 * side, 60 * up));
            return typeof(AIS_idle);
        }

        public Type exit(aI_Controller aI_Controller)
        {
            return typeof(AIS_idle);
        }

        public Type enter(aI_Controller aI_Controller)
        {
            offset = game_time.total * 10;

            return typeof(AIS_idle);
        }

    }

    public class AIS_seek : I_AI_state {

        public Type execute(aI_Controller aI_Controller)
        {
            if(aI_Controller.character.is_in_range(Game.Instance.player, 100)) {
                return typeof(AIS_idle);
            }
            Vector2 direction = Game.Instance.player.transform.position - aI_Controller.character.transform.position;
            direction.Normalize();
            aI_Controller.character.set_velocity(direction * 300);
            return typeof(AIS_seek);
        }

        public Type exit(aI_Controller aI_Controller)
        {
            aI_Controller.character.set_velocity(new Vector2());
            return typeof(AIS_seek);
        }

        public Type enter(aI_Controller aI_Controller)
        {
            return typeof(AIS_seek);
        }

    }

    public class AIS_fire : I_AI_state {

        public Type execute(aI_Controller aI_Controller)
        {
            Vector2 pos = aI_Controller.character.transform.position;
            Vector2 dir = Game.Instance.player.transform.position - pos;
            dir.Normalize();
            var proj = new projectile(pos + 2.0f * dir, dir, 300, 2, new Vector2(10, 10), 10);
            // Set the collision type of the projectile's collider to enemy_bullet
            proj.collider.type = Collision_Type.enemy_bullet;
            Game.Instance.activeMap.add_game_object(proj);
            return typeof(AIS_idle);
        }

        public Type exit(aI_Controller aI_Controller)
        {
            return typeof(AIS_fire);
        }

        public Type enter(aI_Controller aI_Controller)
        {
            return typeof(AIS_fire);
        }
    }
}