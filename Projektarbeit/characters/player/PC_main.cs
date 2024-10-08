namespace Projektarbeit.characters.player
{
    using Box2DX.Common;
    using Core;
    using Core.Controllers.player;
    using Core.defaults;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player.abilities;
    using Projektarbeit.projectiles;

    internal class PC_main : Player_Controller
    {
        private bool isEscapeKeyPressed = false;
        private bool isTestingKeyPressed = false;
        private SoundManager soundManager = new SoundManager();

        private static readonly List<KeyBindingDetail> MoveBindings = new()
        {
            new (Key_Code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
            new (Key_Code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
            new (Key_Code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
            new (Key_Code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
        };

        private static readonly List<KeyBindingDetail> LookBindings = new()
        {
            new (Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
        };

        private static readonly List<KeyBindingDetail> FireBindings = new()
        {
            new (Key_Code.Space, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
        };

        private static readonly List<KeyBindingDetail> UseAbilityBindings = new()
        {
            new (Key_Code.E, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
        };

        private static readonly List<KeyBindingDetail> InGameMenuBindings = new()
        {
            new (Key_Code.Escape, ResetFlags.reset_on_key_up, TriggerFlags.key_press),
        };

        private static readonly List<KeyBindingDetail> testBindings = new()
        {
            new (Key_Code.F, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
        };

        public Action move { get; } = new Action("move", (uint)Action_ModefierFlags.auto_reset, false, ActionType.VEC_2D, 0f, MoveBindings);

        public Action look { get; } = new Action("look", (uint)Action_ModefierFlags.none, false, ActionType.VEC_1D, 0f, LookBindings);

        public Action fire { get; } = new Action("fire", (uint)Action_ModefierFlags.auto_reset, false, ActionType.BOOL, 0f, FireBindings);

        public Action useAbility { get; } = new Action("useAbility", (uint)Action_ModefierFlags.auto_reset, false, ActionType.BOOL, 0f, UseAbilityBindings);
        public Action inGameMenu { get; } = new Action("inGameMenu", (uint)Action_ModefierFlags.auto_reset, false, ActionType.BOOL, 0f, InGameMenuBindings);
        public Action testing { get; } = new Action("testing", (uint)Action_ModefierFlags.auto_reset, false, ActionType.BOOL, 0f, testBindings);

        public Type ProjectileType { get; set; } = typeof(Reflect);

        public PC_main(Character character)
            : base(character, null)
        {
            actions.Clear();

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

            AddInputAction(move);
            AddInputAction(look);
            AddInputAction(fire);
            AddInputAction(useAbility);
            AddInputAction(inGameMenu);
            AddInputAction(testing);

            Game.Instance.camera.Add_Zoom_Offset(0.2f);
            Game.Instance.camera.zoom_offset = 0.2f;
            soundManager.LoadSound("testSound", "assets/sounds/sample1.WAV");
        }

        protected override void Update(float deltaTime)
        {
            if (character.IsDead)
            {
                return;
            }

            if((bool)inGameMenu.GetValue())
            {
                if (!isEscapeKeyPressed)
                {
                    if(Game.Instance.play_state == Play_State.Playing)
                    {
                        Game.Instance.play_state = Play_State.InGameMenu;
                    } 
                    else if(Game.Instance.play_state == Play_State.InGameMenu)
                    {
                        Game.Instance.play_state = Play_State.Playing;
                    }

                    isEscapeKeyPressed = true;
                }
            }
            else
            {
                isEscapeKeyPressed = false;
            }

            if((bool)testing.GetValue())
            {
                if (!isTestingKeyPressed)
                {
                    Game.Instance.camera.transform.ApplyShake(CameraShake.Explosion);
                    Console.WriteLine("Shake");

                    isTestingKeyPressed = true;
                }
            }
            else
            {
                isTestingKeyPressed = false;
            }

            if(Game.Instance.play_state == Play_State.LevelUp) { return; }
            if(Game.Instance.play_state == Play_State.InGameMenu) { return; }
            if(Game.Instance.play_state == Play_State.PauseMenuSkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PauseAbilitySkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PausePowerupSkillTree) { return; }

            float total_speed = character.movement_speed;

                if (move.X != 0 || move.Y != 0)
                {
                    Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                    character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * deltaTime);
                }

                character.transform.rotation = 0;

                if ((bool)fire.GetValue() && Game_Time.total - character.lastFireTime >= character.fireDelay)
                {
                    Vector2 playerLocation = character.transform.position;

                    if (character.Ability is OmniFireAbility omniFireAbility && omniFireAbility.IsActive)
                    {
                        for (int angle = 0; angle < 360; angle += 10)
                        {
                            Vector2 projectileDirection = new(
                                (float)System.Math.Cos(angle * System.Math.PI / 180),
                                (float)System.Math.Sin(angle * System.Math.PI / 180));
                            var projectile = (Projectile)Activator.CreateInstance(ProjectileType, playerLocation, projectileDirection);
                            Game.Instance.get_active_map().Add_Game_Object(projectile);
                        }
                    }
                    else
                    {
                        Vector2 playerDirection = new(0, -1);
                        var projectile = (Projectile)Activator.CreateInstance(ProjectileType, playerLocation, playerDirection);
                        Game.Instance.get_active_map().Add_Game_Object(projectile);
                    }

                    character.lastFireTime = Game_Time.total;
                }

                if ((bool)useAbility.GetValue())
                {
                    character.UseAbility();
                }
        }
    }
}
