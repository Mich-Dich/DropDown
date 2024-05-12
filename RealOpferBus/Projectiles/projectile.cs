using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using Core.visual;
using Core.defaults;
using Core;
using OpenTK.Mathematics;

namespace Hell
{

    public class projectile : game_object
    {

        private Vector2 direction;
        private float speed;
        public float life_time;



        public projectile(Vector2 position, Vector2 direction, float speed, float life_time, Vector2 size, float damage)
        {
            this.transform.position = position;
            this.direction = direction;
            this.speed = speed;
            this.life_time = life_time;

            this.transform.size = size;

            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Beam/Beam.png", false)));
            add_collider(new collider(collision_shape.Square, collision_type.player_bullet)
                { Blocking = false }
                .set_physics_material(new physics_material(0.05f, 0.1f))
                .set_damage_value(damage));
        }

        public override void update(float delta_time)
        {
            base.update(delta_time);

            this.transform.position += direction * speed * delta_time;

            life_time -= delta_time;

            if (life_time <= 0)
            {
                game.instance.active_map.remove_game_object(this);
            }
        }

        public override void hit(hit_data hit)
        {

            base.hit(hit);
        }
    }
}