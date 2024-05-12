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

            set_sprite(new sprite().add_animation("assets/textures/Angel-2", true, true, 10, true));
            add_collider(new collider(collision_shape.Circle, collision_type.None) { Blocking = false }
                .set_physics_material(new physics_material(0.0f, 0.0f))
                .set_mass(100));
            
            controller = new enemy_controller();
            set_controller(controller);
        }

        public override void hit(hit_data hit)
        {
            if (hit.hit_object.collider.type == collision_type.player_bullet)
            {
                this.Health -= hit.hit_object.collider.damage;

                game.instance.active_map.remove_game_object(hit.hit_object);

                if (this.Health <= 0)
                {
                    game.instance.active_map.remove_game_object(this);
                    game.instance.score++;
                }
            }
        }

        public override void update(float delta_time)
        {
            controller.update();
        }

    }
}