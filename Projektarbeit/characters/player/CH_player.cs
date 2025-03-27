namespace Hell.player
{
    using Core.physics;
    using Core.util;
    using Core.world;
    using Hell.player.ability;
    using Hell.weapon;
    using ImGuiNET;
    using OpenTK.Mathematics;

    internal class CH_player : Character
    {
        public float CooldownBarWidth { get; set; } = 40;

        public float CooldownBarHeight { get; set; } = 4;

        private uint cooldownColTransparent = 0;

        public CH_player()
        {
            this.IsDead = false;
            this.transform.size = new Vector2(100);
            this.health = 100;
            this.auto_heal_amout = 0;
            this.Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/Angel-1.png")));
            this.Add_Collider(new Collider(Collision_Shape.Circle)
                .Set_Offset(new Transform(Vector2.Zero, new Vector2(-10))));

            this.movement_speed = 400.0f;
            this.rotation_offset = float.Pi / 2;

            this.Ability = new ShieldAbility();
        }

        public void DisplayCooldownBar(System.Numerics.Vector2? display_size = null, System.Numerics.Vector2? pos_offset = null, System.Numerics.Vector2? padding = null, float rounding = 0.0f)
        {
            if(!this.IsRemoved)
            {
                string uniqueId = $"Helthbar_for_character_{this.GetHashCode()}";
                if (display_size == null)
                {
                    display_size = new System.Numerics.Vector2(this.CooldownBarWidth, this.CooldownBarHeight);
                }

                ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                    | ImGuiWindowFlags.NoDocking
                    | ImGuiWindowFlags.AlwaysAutoResize
                    | ImGuiWindowFlags.NoSavedSettings
                    | ImGuiWindowFlags.NoFocusOnAppearing
                    | ImGuiWindowFlags.NoNav
                    | ImGuiWindowFlags.NoMove;

                System.Numerics.Vector2 position =
                    Core.util.util.convert_Vector(Core.util.util.Convert_World_To_Screen_Coords(this.transform.position)) + (pos_offset ?? System.Numerics.Vector2.Zero);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, padding ?? new System.Numerics.Vector2(4));

                if (rounding != 0.0f)
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, rounding);
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, rounding);
                }

                // Set the transparent color
                this.cooldownColTransparent = ImGui.GetColorU32(new System.Numerics.Vector4(0, 0, 0, 0));

                // Disable the border
                ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);

                ImGui.SetNextWindowPos(position, ImGuiCond.Always, new System.Numerics.Vector2(0.5f));
                ImGui.Begin(uniqueId, window_flags);

                // Calculate Cooldown
                float cooldownRemaining = Math.Max(0, this.fireDelay - (Game_Time.total - this.lastFireTime));
                float cooldownRatio = cooldownRemaining / this.fireDelay;

                Imgui_Util.Progress_Bar_Stylised(
                    cooldownRatio,
                    display_size.Value,
                    4294944000,
                    this.cooldownColTransparent,
                    0f,
                    0f,
                    0.35f);

                if (cooldownRemaining > 0)
                {
                }

                ImGui.End();
                ImGui.PopStyleVar();

                if (rounding != 0.0f)
                {
                    ImGui.PopStyleVar(2);
                }
            }
        }

        public override void draw_imgui()
        {
            base.draw_imgui();

            float cooldownRemaining = Math.Max(0, this.fireDelay - (Game_Time.total - this.lastFireTime));
            if (cooldownRemaining > 0)
            {
                this.DisplayCooldownBar(null, new System.Numerics.Vector2(0, 20), new System.Numerics.Vector2(1), 5);
            }
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile testProjectile && !testProjectile.HasHit)
            {
                this.apply_damage(testProjectile.Damage);
                testProjectile.HasHit = true;
            }
            else
            {
                base.Hit(hit);
            }
        }
    }
}