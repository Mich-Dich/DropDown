using Core;
using Core.physics;
using Core.render;
using OpenTK.Mathematics;

namespace Hell.weapon {

    public class TestProjectile : Projectile {

        private static readonly Texture texture = new Texture("assets/textures/projectiles/beam/beam.png");
        private static readonly Vector2 size = new Vector2(35, 80);
        private static readonly float speed = 1500f;
        private static readonly float damage = 2f;
        private static readonly bool bounce = true;
        private static readonly Collision_Shape shape = Collision_Shape.Square;

        public TestProjectile(Vector2 position, Vector2 direction) 
            : base(position, direction, speed, damage, bounce, shape) {

            Sprite sprite = new Sprite(texture);
            Set_Sprite(sprite);
            this.transform.size = size;
        }
    }
}