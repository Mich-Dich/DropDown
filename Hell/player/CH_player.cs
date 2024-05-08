
namespace Hell {

    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class CH_player : character {

        public CH_player() {

            transform.size = new Vector2(100);
            this.transform.position = new Vector2(480, 6080);
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Angel-1/Angel-1.png")));
            add_collider(new collider(collision_shape.Circle) { Blocking = true })
                //.set_offset(new transform(Vector2.Zero, new Vector2(-10)))
                .set_mobility(mobility.DYNAMIC);

            movement_speed = 2000.0f;
        }


        public override void hit(hit_data hit) { }

    }
}