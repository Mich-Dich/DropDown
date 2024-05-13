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
            add_collider(new collider(Collision_Shape.Square, Collision_Type.player_bullet)
                { blocking = false }
                .Set_Physics_Material(new Physics_Material(0.05f, 0.1f))
                .set_damage_value(damage));
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            this.transform.position += direction * speed * deltaTime;

            life_time -= deltaTime;

            if (life_time <= 0)
            {
                game.instance.activeMap.remove_game_object(this);
            }
        }

        public override void hit(hitData hit)
        {

            base.hit(hit);
        }
    }
}