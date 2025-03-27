namespace Hell.player
{
    using Box2DX.Common;
    using Core.Controllers.player;
    using Core.defaults;
    using Core.util;
    using Core.world;
    using Hell.player.ability;
    using Hell.weapon;
    using OpenTK.Mathematics;
    using Core;

    internal class PC_main : Player_Controller
    {
        public Action move { get; set; }

        public Action look { get; set; }

        public Action fire { get; set; }

        public Action useAbility { get; set; }

        public Type ProjectileType { get; set; } = typeof(Reflect);

        public PC_main(Character character)
            : base(character, null)
            {
            this.actions.Clear();

            character.death_callback = () =>
            {
                if (!character.IsDead)
                {
                    character.IsDead = true;
                    character.health = 0;
                    character.IsRemoved = true;
                    Game.Instance.play_state = Play_State.dead;
                }
            };

            this.move = new Action(
                "move",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail>
                {
                    new (Key_Code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
                    new (Key_Code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
                    new (Key_Code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
                    new (Key_Code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
                });
            this.AddInputAction(this.move);

            this.look = new Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail>
                {
                    new (Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            this.AddInputAction(this.look);

            this.fire = new Action(
                "fire",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail>
                {
                    new (Key_Code.Space, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            this.AddInputAction(this.fire);

            this.useAbility = new Action(
                "useAbility",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail>
                {
                    new (Key_Code.E, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            this.AddInputAction(this.useAbility);

            Game.Instance.camera.Add_Zoom_Offset(0.2f);
            Game.Instance.camera.zoom_offset = 0.2f;
        }

        protected override void Update(float deltaTime)
        {
            if (this.character.IsDead)
            {
                return;
            }

            float total_speed = this.character.movement_speed;

            if (this.move.X != 0 || this.move.Y != 0)
            {
                Vector2 direction = Vector2.NormalizeFast((Vector2)this.move.GetValue());
                this.character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * deltaTime);
            }

            this.character.transform.rotation = 0;

            if ((bool)this.fire.GetValue() && Game_Time.total - this.character.lastFireTime >= this.character.fireDelay)
            {
                Vector2 playerLocation = this.character.transform.position;

                if (this.character.Ability is OmniFireAbility omniFireAbility && omniFireAbility.IsActive)
                {
                    for (int angle = 0; angle < 360; angle += 10)
                    {
                        Vector2 projectileDirection = new (
                            (float)System.Math.Cos(angle * System.Math.PI / 180),
                            (float)System.Math.Sin(angle * System.Math.PI / 180));
                        var projectile = (Projectile)Activator.CreateInstance(this.ProjectileType, playerLocation, projectileDirection);
                        Game.Instance.get_active_map().Add_Game_Object(projectile);
                    }
                }
                else
                {
                    Vector2 playerDirection = new (0, -1);
                    var projectile = (Projectile)Activator.CreateInstance(this.ProjectileType, playerLocation, playerDirection);
                    Game.Instance.get_active_map().Add_Game_Object(projectile);
                }

                this.character.lastFireTime = Game_Time.total;
            }

            if ((bool)this.useAbility.GetValue())
            {
                this.character.UseAbility();
            }
        }
    }
}