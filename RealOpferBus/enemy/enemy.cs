using Core.game_objects;
using Core.util;
using Core.visual;
using Core.physics;
using Core;
using OpenTK.Mathematics;
using Core.physics.material;

namespace Hell {

    public class enemy : character {

        private enemy_controller controller;
        public float Health { get; set; } = 100f;

        public enemy(Vector2? position = null, Vector2? size = null, Single rotation = 0) {

            this.transform.position = position ?? new Vector2();
            this.transform.size = size?? new Vector2(80);
            this.transform.rotation = rotation;

            set_sprite(new sprite().Add_Animation("assets/textures/Angel-2", true, true, 10, true));
            add_collider(new collider(Collision_Shape.Circle, Collision_Type.None) { blocking = false }
                .Set_Physics_Material(new Physics_Material(0.0f, 0.0f))
                .Set_Mass(100));
            
            controller = new enemy_controller();
            set_controller(controller);
        }

        public override void hit(hitData hit)
        {
            if (hit.hitObject.collider.type == Collision_Type.player_bullet)
            {
                this.Health -= hit.hitObject.collider.damage;

                Game.Instance.activeMap.remove_game_object(hit.hitObject);

                if (this.Health <= 0)
                {
                    Game.Instance.activeMap.remove_game_object(this);
                    Game.Instance.score++;
                }
            }
        }

        public override void Update(float deltaTime)
        {
            controller.Update();
        }

    }
}