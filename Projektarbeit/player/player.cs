
namespace Projektarbeit.player {

    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class CH_player : character {
        public CH_player() {
            
            transform.size = new Vector2(100);
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/player/Angel-1.png")));
            add_collider(new collider(collision_shape.Circle)
                .set_offset(new transform(Vector2.Zero, new Vector2(-10)))
                .set_physics_material(new physics_material(10.05f, 0.1f)));
            
            movement_speed = 5000.0f;
        }

        public override void hit(hit_data hit) { }



    }
}
