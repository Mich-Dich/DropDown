namespace Hell.player {

    using Box2DX.Common;
    using Core.Controllers.player;
    using Core.util;
    using Core.world;
    using Hell.weapon;
    using Core.defaults;
    using OpenTK.Mathematics;
    using Hell.player.ability;

    internal class PC_main : Player_Controller {

        public Action move { get; set; }
        public Action look { get; set; }
        public Action fire { get; set; }
        public Action useAbility { get; set; }

        public float fireDelay = 0.5f; // Delay in seconds
        public float lastFireTime = 0f;
        public Type ProjectileType { get; set; } = typeof(TestProjectile);

        public PC_main(Character character)
            : base(character, null) {

            actions.Clear();

            character.death_callback = () => {
                if (!character.IsDead) {
                    character.IsDead = true;
                    character.health = 0;
                    Game.Instance.get_active_map().Remove_Game_Object(character);
                    Game.Instance.get_active_map().allCharacter.Remove(character);
                }
            };

            move = new Action(
                "move",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
                    new(Key_Code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
                    new(Key_Code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
                    new(Key_Code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
                });
            AddInputAction(move);


            look = new Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            AddInputAction(look);
            

            fire = new Action(
                "fire",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {
                    new(Key_Code.Space, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(fire);

            useAbility = new Action(
                "useAbility",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {
                    new(Key_Code.E, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(useAbility);

            Game.Instance.camera.Add_Zoom_Offset(0.2f);
            Game.Instance.camera.zoom_offset = 0.2f;
        }

        private Vector2 RotateVector(Vector2 v, float angle) {
            float sin = (float)System.Math.Sin(angle);
            float cos = (float)System.Math.Cos(angle);

            float tx = v.X;
            float ty = v.Y;

            return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        }
        protected override void Update(float deltaTime) {

            if (character.IsDead) {
                return;
            }

            float total_speed = character.movement_speed;

            // simple movement
            if(move.X != 0 || move.Y != 0) {
                Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * deltaTime);
            }

            // character always faces upwards
            character.transform.rotation = 0;

             if((bool)fire.GetValue() && Game_Time.total - lastFireTime >= fireDelay) {
                Vector2 playerLocation = character.transform.position;

                if (character.Ability is OmniFireAbility omniFireAbility && omniFireAbility.IsActive()) {
                    // Fire projectiles in all directions
                    for (int angle = 0; angle < 360; angle += 10) {
                        Vector2 projectileDirection = new Vector2((float)System.Math.Cos(angle * System.Math.PI / 180), (float)System.Math.Sin(angle * System.Math.PI / 180));
                        var projectile = (Projectile)Activator.CreateInstance(ProjectileType, playerLocation, projectileDirection);
                        Game.Instance.get_active_map().Add_Game_Object(projectile);
                    }
                } else {
                     if (character is CH_player playerCharacter) {
                        // Normal single projectile fire
                        Vector2 playerDirection = new Vector2(0, -1);
                        for (int i = 0; i < playerCharacter.projectilesPerShot; i++) {
                            Vector2 offsetDirection = playerCharacter.projectilesPerShot == 1 ? playerDirection : RotateVector(playerDirection, i * 0.1f - (playerCharacter.projectilesPerShot - 1) * 0.05f);
                            var projectile = (Projectile)Activator.CreateInstance(ProjectileType, playerLocation, offsetDirection);
                            Game.Instance.get_active_map().Add_Game_Object(projectile);
                        }
                    }
                }

                lastFireTime = Game_Time.total;
            }

            if ((bool)useAbility.GetValue()) {
                character.UseAbility();
            }
        }
    }
}