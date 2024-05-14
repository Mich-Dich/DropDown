using Core.physics;
using Core.render;
using Core.world;
using OpenTK.Mathematics;

namespace Hell.weapon {
    public class Projectile : Game_Object {
        public float Speed { get; set; }
        public float Damage { get; set; }
        public bool Bounce { get; set; }
        public Sprite Sprite { get; set; }

        public Projectile(Vector2 position, Vector2 direction, float speed = 10f, float damage = 1f, bool bounce = false, Collision_Shape shape = Collision_Shape.Square) : base(position) {
            Speed = speed;
            Damage = damage;
            Bounce = bounce;
            collider = new Collider(shape, Collision_Type.bullet, null, 1f, direction * speed);
            Sprite = new Sprite();
            Set_Sprite(Sprite);
        }

        public override void Update(float deltaTime) {
        }

        public override void Hit(hitData hit) {
            // Handle hit
            if (Bounce) {
                // Calculate reflection direction
            } else {
                // Destroy the projectile
                // TODO: Implement destruction logic
            }
        }
    }
}