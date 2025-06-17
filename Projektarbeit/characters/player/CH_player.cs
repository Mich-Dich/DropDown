namespace Projektarbeit.characters.player
{
    using Box2DX.Common;
    using Box2DX.Dynamics;
    using Core.physics;
    using Core.util;
    using Core.world;
    using ImGuiNET;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player.abilities;
    using Projektarbeit.projectiles;

    internal class CH_player : Character
    {
        private const float DefaultCooldownBarWidth = 40;
        private const float DefaultCooldownBarHeight = 4;
        private readonly uint defaultCooldownColTransparent;

        // Hit flash variables
        private bool isFlashing = false;
        private float flashStartTime = 0f;
        private const float FlashDuration = 0.2f; // Duration of the red flash in seconds
        private const float FlashIntensity = 0.7f; // How red the flash should be (0.0 = no red, 1.0 = full red)

        public float CooldownBarWidth { get; set; } = DefaultCooldownBarWidth;

        public float CooldownBarHeight { get; set; } = DefaultCooldownBarHeight;

        private uint cooldownColTransparent;

        public CH_player()
        {
            defaultCooldownColTransparent = ImGui.GetColorU32(new System.Numerics.Vector4(0, 0, 0, 0));
            cooldownColTransparent = defaultCooldownColTransparent;
            InitializePlayer();
        }

        public void DisplayCooldownBar(System.Numerics.Vector2? display_size = null, System.Numerics.Vector2? pos_offset = null, System.Numerics.Vector2? padding = null, float rounding = 0.0f)
        {
            if (IsRemoved) return;

            string uniqueId = $"Helthbar_for_character_{GetHashCode()}";
            display_size ??= new System.Numerics.Vector2(CooldownBarWidth, CooldownBarHeight);

            ImGuiWindowFlags window_flags = GetWindowFlags();

            System.Numerics.Vector2 position = util.convert_Vector<System.Numerics.Vector2>(util.Convert_World_To_Screen_Coords(transform.position))
                + (pos_offset ?? System.Numerics.Vector2.Zero);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, padding ?? new System.Numerics.Vector2(4));

            if (rounding != 0.0f)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, rounding);
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, rounding);
            }

            // Disable the border
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);

            ImGui.SetNextWindowPos(position, ImGuiCond.Always, new System.Numerics.Vector2(0.5f));
            ImGui.Begin(uniqueId, window_flags);

            // Calculate Cooldown
            float cooldownRatio = CalculateCooldownRatio();

            Imgui_Util.Progress_Bar_Stylised(
                cooldownRatio,
                display_size.Value,
                4294944000,
                cooldownColTransparent,
                0f,
                0f,
                0.35f);

            ImGui.End();
            ImGui.PopStyleVar();

            if (rounding != 0.0f)
            {
                ImGui.PopStyleVar(2);
            }
        }

        public override void draw_imgui()
        {
            base.draw_imgui();

            if (CalculateCooldownRemaining() > 0)
            {
                DisplayCooldownBar(null, new System.Numerics.Vector2(0, 20), new System.Numerics.Vector2(1), 5);
            }
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile testProjectile && !testProjectile.HasHit)
            {
                apply_damage(testProjectile.Damage);
                Game.Instance.camera.transform.ApplyShake(CameraShake.LargeProjectileHit);
                testProjectile.HasHit = true;
            }
            else
            {
                base.Hit(hit);
            }
        }

        public override void apply_damage(float damage)
        {
            base.apply_damage(damage);
            
            // Trigger hit flash whenever damage is taken
            TriggerHitFlash();
        }

        private void InitializePlayer()
        {
            IsDead = false;
            transform.size = new Vector2(100);
            health = 100;
            auto_heal_amout = 0;
            Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/Angel-1.png")));
            Add_Collider(new Collider(Collision_Shape.Circle).Set_Offset(new Transform(Vector2.Zero, new Vector2(-10))));
            movement_speed = 400.0f;
            rotation_offset = float.Pi / 2;

            Ability = null;
        }

        private ImGuiWindowFlags GetWindowFlags()
        {
            return ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove;
        }

        private float CalculateCooldownRemaining()
        {
            return Math.Max(0, fireDelay - (Game_Time.total - lastFireTime));
        }

        private float CalculateCooldownRatio()
        {
            return CalculateCooldownRemaining() / fireDelay;
        }
            
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            ApplyBounding();
            UpdateHitFlash();
        }

        private void TriggerHitFlash()
        {
            isFlashing = true;
            flashStartTime = Game_Time.total;
        }

        private void UpdateHitFlash()
        {
            if (!isFlashing) return;

            float elapsedTime = Game_Time.total - flashStartTime;
            if (elapsedTime >= FlashDuration)
            {
                // Flash finished, reset to normal
                isFlashing = false;
                if (sprite != null)
                {
                    sprite.Set_Tint_Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)); // Reset to white
                }
                return;
            }

            // Calculate flash intensity (fade out over time)
            float flashProgress = elapsedTime / FlashDuration;
            float currentIntensity = FlashIntensity * (1.0f - flashProgress);
            
            // Set red tint (increase red channel, keep others at 1.0)
            if (sprite != null)
            {
                sprite.Set_Tint_Color(new Vector4(1.0f + currentIntensity, 1.0f - currentIntensity * 0.3f, 1.0f - currentIntensity * 0.3f, 1.0f));
            }
        }
    }
}
