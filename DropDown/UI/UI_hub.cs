
namespace DropDown.UI {
    using Core;
    using Core.render;
    using Core.UI;
    using Core.util;
    using Core.world;
    using DropDown.player;
    using ImGuiNET;

    internal class UI_hub : Menu {

        private bool display_upgades = true;
        private Texture background;

        private Texture PB_tile;
        private Texture PB_plus;
        private Texture PB_plus_hover;

        private Texture title_speed;
        private Texture title_lifetime;
        private Texture title_knockback;
        private Texture title_damage;
        private Texture title_cooldown;
        private CH_player player;

        public UI_hub(CH_player player) {

            this.player = player;
            background = Resource_Manager.Get_Texture("assets/textures/UI/main_menu_bg.png");

            PB_tile = Resource_Manager.Get_Texture("assets/textures/UI/progress_bar_item.png");
            PB_plus = Resource_Manager.Get_Texture("assets/textures/UI/progress_bar_pluss_bu.png");
            PB_plus_hover = Resource_Manager.Get_Texture("assets/textures/UI/progress_bar_pluss_bu_hover.png");

            title_speed = Resource_Manager.Get_Texture("assets/textures/UI/title_speed.png");
            title_lifetime = Resource_Manager.Get_Texture("assets/textures/UI/title_lifetime.png");
            title_knockback = Resource_Manager.Get_Texture("assets/textures/UI/title_knockback.png");
            title_damage = Resource_Manager.Get_Texture("assets/textures/UI/title_damage.png");
            title_cooldown = Resource_Manager.Get_Texture("assets/textures/UI/title_cooldown.png");
        }


        // input variables
        private void display_progressbar(
            System.Numerics.Vector2 start_position,  
            int texture_handle,
            string name, 
            Action onClick,
            int progressbar_value,
            float padding = 3.0f) {


            System.Numerics.Vector2 message_size = new System.Numerics.Vector2(250, 40);
            System.Numerics.Vector2 tile_size = new System.Numerics.Vector2(message_size.Y);
            const int max_tiles = 5;

            ImGui.SetCursorPos(start_position);
            ImGui.Image(texture_handle, message_size);

            for (int x = 0; x < max_tiles; x++) {

                if (x < progressbar_value) {

                    ImGui.SetCursorPosY(start_position.Y);
                    ImGui.SetCursorPosX(start_position.X + message_size.X + padding + (x * (tile_size.X + padding)));
                    ImGui.Image(PB_tile.Handle, tile_size);
                }
            }

            ImGui.SetCursorPosY(start_position.Y);
            ImGui.SetCursorPosX(start_position.X + message_size.X + padding + (max_tiles * (tile_size.X + padding)));

            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(0));

            if(ImGui.ImageButton(name, PB_plus.Handle, tile_size)) {
                if(player.has_free_AB_point()) {

                    Console.WriteLine($"Invoking Function");
                    onClick.Invoke();
                    player.use_AB_point();
                }
            }

            ImGui.PopStyleVar();
            ImGui.PopStyleColor(3);
        }


        public override void Render() {

            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoBackground;

            ImGui.SetNextWindowBgAlpha(0f);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(-10), ImGuiCond.Always, new System.Numerics.Vector2(0, 0));
            ImGui.SetNextWindowSize(util.convert_Vector<System.Numerics.Vector2>(Game.Instance.window.Size) + new System.Numerics.Vector2(20));
            ImGui.Begin("ui_HUD", window_flags);

            if(display_upgades) {

                System.Numerics.Vector2 pos = ImGui.GetCursorPos() + new System.Numerics.Vector2(10);
                ImGui.Image(background.Handle, new System.Numerics.Vector2(Game.Instance.window.Size.X * 0.4f, Game.Instance.window.Size.Y));

                display_progressbar(
                    new System.Numerics.Vector2(100, 150),
                    title_speed.Handle,
                    "speed",
                    () => { projectile_data.speed.add_section(); },
                    projectile_data.speed.get_section()
                    );

                display_progressbar(
                    new System.Numerics.Vector2(100, 200),
                    title_lifetime.Handle,
                    "lifetime",
                    () => { projectile_data.lifespan.add_section(); },
                    projectile_data.lifespan.get_section()
                    );
      
                display_progressbar(
                    new System.Numerics.Vector2(100, 250),
                    title_knockback.Handle,
                    "knockback",
                    () => { projectile_data.knockback.add_section(); },
                    projectile_data.knockback.get_section()
                    );

                display_progressbar(
                    new System.Numerics.Vector2(100, 300),
                    title_damage.Handle,
                    "damage",
                    () => { projectile_data.damage.add_section(); },
                    projectile_data.damage.get_section()
                    );

                display_progressbar(
                    new System.Numerics.Vector2(100, 350),
                    title_cooldown.Handle,
                    "cooldown",
                    () => { projectile_data.cooldown.add_section(); },
                    projectile_data.cooldown.get_section()
                    );


            }

            ImGui.End();
        }



    }
}
