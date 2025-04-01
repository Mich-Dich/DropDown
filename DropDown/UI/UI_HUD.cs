
namespace DropDown.UI {

    using Core;
    using Core.render;
    using Core.UI;
    using Core.util;
    using DropDown.player;
    using ImGuiNET;

    public class UI_HUD : Menu {

        public System.Numerics.Vector2 screen_size_buffer;
        public bool display_pause_menu = false;

        private Texture image_blood_overlay;
        private Texture image_hud_box;
        private Texture image_hud_box_selected;
        private CH_player player;
        private uint col_red = 0;
        private uint col_blue = 0;
        private uint col_black = 0;

        private System.Numerics.Vector2 icon_box_size = new System.Numerics.Vector2(45);
        private System.Numerics.Vector2 icon_offset = new System.Numerics.Vector2(10);

        public UI_HUD() {
            
            // HUD
            image_blood_overlay = Resource_Manager.Get_Texture("assets/textures/BloodOverlay.png");
            image_hud_box = Resource_Manager.Get_Texture("assets/textures/box.png");
            image_hud_box_selected = Resource_Manager.Get_Texture("assets/textures/box_selected.png");

            col_red = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1));
            col_blue = ImGui.GetColorU32(new System.Numerics.Vector4(0.2f, 0.2f, 0.8f, 1));
            col_black = ImGui.GetColorU32(new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 1f));

            player = (CH_player)Game.Instance.player;
        }

        public override void Render() {

            display_blood_overlay(Game_Time.delta);

            //ImGui.ShowStyleEditor();

            // HUD
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove;

            ImGui.PushStyleColor(ImGuiCol.WindowBg, col_black);
            ImGui.SetNextWindowBgAlpha(1f);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(-2, Game.Instance.window.Size.Y - 38), ImGuiCond.Always, new System.Numerics.Vector2(0, 1));
            ImGui.Begin("HUD_BotomLeft", window_flags);
            {

                Imgui_Util.Progress_Bar_Stylised(player.health / player.health_max, new System.Numerics.Vector2(250, 15), col_red, col_black, 0.35f, 0.28f, 0.6f);
                ImGui.Spacing();
                Imgui_Util.Progress_Bar_Stylised(1f, new System.Numerics.Vector2(210, 12), col_blue, col_black, 0.24f, 0.28f, 0.6f);
                ImGui.Spacing();
                Imgui_Util.Progress_Bar_Stylised(1f, new System.Numerics.Vector2(210, 12), col_blue, col_black, 0.24f, 0.28f, 0.6f);

                ImGui.Spacing();
                Imgui_Util.Title($"Level {player.level}");
                ImGui.SameLine();
                Imgui_Util.Shift_Cursor_Pos(15, 0);
                Imgui_Util.Title($"{player.XP_current}/{player.XP_needed}");
            }
            ImGui.End();
            ImGui.PopStyleColor();


            // Fullscreen background
            if (display_pause_menu) {

                ((Game)Game.Instance).Set_Game_Paused(true);

                //Console.WriteLine($"displaying pause menu");
                ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);
                ImGui.SetNextWindowSize(io.DisplaySize);

                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0, 0));
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
                ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0.7f)));

                ImGui.Begin("PauseMenuBackground",
                    ImGuiWindowFlags.NoDecoration |
                    ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoSavedSettings |
                    ImGuiWindowFlags.NoBringToFrontOnFocus);

                // Main menu content
                System.Numerics.Vector2 center = io.DisplaySize * 0.5f;
                ImGui.SetCursorPos(new System.Numerics.Vector2(center.X - 100, center.Y - 150));

                ImGui.BeginChild("PauseMenuContent", new System.Numerics.Vector2(200, 300));
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 1f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.3f, 0.3f, 0.3f, 1f));

                    ImGui.SetCursorPosX(20);
                    if (ImGui.Button("Resume", new System.Numerics.Vector2(160, 40)))
                        display_pause_menu = false;

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 20);
                    ImGui.SetCursorPosX(20);
                    if (ImGui.Button("Settings", new System.Numerics.Vector2(160, 40))) {

                        // TODO: Open settings menu
                    }

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 20);
                    ImGui.SetCursorPosX(20);
                    if (ImGui.Button("Quit to Menu", new System.Numerics.Vector2(160, 40)))
                    {
                        
                        ((Drop_Down)Game.Instance).set_play_state(Play_State.main_menu);
                        Game.Instance.set_active_map(new MAP_start());
                        display_pause_menu = false;
                        ((Game)Game.Instance).Set_Game_Paused(false);
                    }

                    ImGui.PopStyleColor(2);
                }
                ImGui.EndChild();

                ImGui.End();
                ImGui.PopStyleColor();
                ImGui.PopStyleVar(2);
            } else
            {
                ((Game)Game.Instance).Set_Game_Paused(false);
            }


            //ImGui.PushStyleColor(ImGuiCol.WindowBg, col_black);
            //ImGui.SetNextWindowBgAlpha(1f);
            //ImGui.SetNextWindowPos(new System.Numerics.Vector2((Game.Instance.window.Size.X / 2), Game.Instance.window.Size.Y - 38), ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 1));
            //ImGui.Begin("HUD_bottom_middle", window_flags);
            //{

            //    const int selected = 0;

            //    System.Numerics.Vector2 cursor_pos = ImGui.GetCursorPos();

            //    for(int x = 0; x < 10; x++) {

            //        cursor_pos = ImGui.GetCursorPos();
            //        ImGui.Image((x == selected) ? image_hud_box_selected.Handle : image_hud_box.Handle, icon_box_size);

            //        if(x == 0) {
            //            ImGui.SetCursorPos(cursor_pos);
            //            Imgui_Util.Shift_Cursor_Pos(icon_offset.X, icon_offset.Y);
            //            ImGui.Image(Resource_Manager.Get_Texture("assets/textures/weapon.png").Handle, icon_box_size - (icon_offset * 2));
            //        }

            //        if(x == 1) {
            //            ImGui.SetCursorPos(cursor_pos);
            //            Imgui_Util.Shift_Cursor_Pos(icon_offset.X, icon_offset.Y);
            //            ImGui.Image(Resource_Manager.Get_Texture("assets/textures/bow.png").Handle, icon_box_size - (icon_offset * 2));
            //        }

            //        if(x == 4) {
            //            ImGui.SetCursorPos(cursor_pos);
            //            Imgui_Util.Shift_Cursor_Pos(icon_offset.X, icon_offset.Y);
            //            ImGui.Image(Resource_Manager.Get_Texture("assets/textures/torch.png").Handle, icon_box_size - (icon_offset * 2));
            //        }

            //        ImGui.SetCursorPos(cursor_pos);
            //        Imgui_Util.Shift_Cursor_Pos(icon_box_size.X + 10, 0);
            //    }
            //}
            //ImGui.End();
            //ImGui.PopStyleColor();

        }


        // blood overlay
        private float _boold_overlay_decreace_amout = 1.5f;
        private float _boold_overlay_intencity = 0.0f;

        public void flash_blood_overlay() { _boold_overlay_intencity += 0.3f; }

        private void display_blood_overlay(float deltaTime) {

            float lower_limit = 1-((player.health/ player.health_max) * 2);
            if(lower_limit <= 0 && _boold_overlay_intencity <= 0)
                return;

            if(_boold_overlay_intencity > lower_limit)
                _boold_overlay_intencity -= deltaTime * _boold_overlay_decreace_amout;

            ImGuiWindowFlags window_flags_overlay = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove;

            ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, System.Numerics.Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, _boold_overlay_intencity);

            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);
            ImGui.SetNextWindowSize(screen_size_buffer);
            ImGui.Begin("Screen_overlay", window_flags_overlay);
            ImGui.Image(image_blood_overlay.Handle, screen_size_buffer);
            ImGui.End();

            ImGui.PopStyleVar(3);
            ImGui.PopStyleColor();

        }

    }
}
