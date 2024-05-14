
namespace Hell {

    using Core;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class enemy : Character {

        private enemy_controller controller;
        public float Health { get; set; } = 100f;

        public enemy(Vector2? position = null, Vector2? size = null, Single rotation = 0) {

            this.transform.position = position ?? new Vector2();
            this.transform.size = size ?? new Vector2(80);
            this.transform.rotation = rotation;

            Set_Sprite(new Sprite().set_animation("assets/textures/Angel-2", true, true, 10, true));

            //controller = new enemy_controller();
            //Set_Controller(controller);
        }

        public override void Hit(hitData hit) {
 
            if(hit.hitObject.collider.type == Collision_Type.player_bullet) {
                this.Health -= hit.hitObject.collider.damage;

                Game.Instance.ctiveMap.remove_game_object(hit.hitObject);

                if(this.Health <= 0) {
                    Game.Instance.activeMap.remove_game_object(this);
                    Game.Instance.score++;
                }
            }
        }

        public override void Update(float deltaTime) {
            controller.Update();
        }

    }
}