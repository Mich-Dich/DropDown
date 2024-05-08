
namespace Hell {
 
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class projectile : game_object {

        private Vector2 direction;
        private float speed;
        private float life_time;



        public projectile(Vector2 position, Vector2 direction, float speed, float life_time) {

            this.transform.position = position;
            this.direction = direction;
            this.speed = speed;
            this.life_time = life_time;

            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Beam/Beam.png", true)));
            add_collider(new collider(collision_shape.Square)
                .set_physics_material(new physics_material(0.05f, 0.1f)));

        }

        public override void update(float delta_time) {

            base.update(delta_time);

            this.transform.position += direction * speed * delta_time;

            life_time -= delta_time;
            //if (life_time <= 0)
            //destroy();





        }

        public override void hit(hit_data hit) {

            base.hit(hit);
            //Console.WriteLine("Projectile collided with an object");
            //destroy();
        }
    }
}