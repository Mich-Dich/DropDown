namespace Hell.player {

    using Core.physics;
    using Core.util;
    using Core.world;
    using Hell.weapon;
    using OpenTK.Mathematics;
    using Hell.player.ability;

    internal class CH_player : Character {
        public int projectilesPerShot { get; set; } = 1;

        public CH_player() {
            
            transform.size = new Vector2(100);
            Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/Angel-1.png")));
            Add_Collider(new Collider(Collision_Shape.Circle)
                .Set_Offset(new Transform(Vector2.Zero, new Vector2(-10))));

            movement_speed = 400.0f;
            rotation_offset = float.Pi / 2;

            Ability = new OmniFireAbility();
        }

        public override void Hit(hitData hit) {
            if(hit.hit_object is EnemyTestProjectile testProjectile && !testProjectile.HasHit) {
                this.apply_damage(testProjectile.Damage);
                testProjectile.HasHit = true;
            } else {
                base.Hit(hit);
            }
        }
    }
}