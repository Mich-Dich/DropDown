﻿
namespace DropDown {

    using Core.render;
    using Core.util;
    using DropDown.player;
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    internal class Drop_Down : Core.Game {

        public Drop_Down(String title, Int32 initalWindowWidth, Int32 initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) {
        
        }

        //private game_object test_cursor_object;
        //private game_object test_cursor_object_2;
        private CH_player CH_player;

        // ========================================================= functions =========================================================
        protected override void Init() {
            
            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            Set_Update_Frequency(144.0f);
            Show_Debug_Data(true);

            CH_player = new CH_player();
            this.playerController = new PC_Default(CH_player);
            this.player = CH_player;
            this.activeMap = new Base_Map();
            this.camera.Set_min_Max_Zoom(0.05f, 10.9f);

            //test_cursor_object = new game_object(new Vector2(150, 150)).set_sprite(resource_manager.get_texture("assets/defaults/default_grid_bright.png"));
            //this.activeMap.add_game_object(test_cursor_object);

            //test_cursor_object_2 = new game_object(new Vector2(150, 150)).set_sprite(resource_manager.get_texture("assets/defaults/default_grid_bright.png"));
            //this.activeMap.add_game_object(test_cursor_object_2);
            
            //this.activeMap.add_sprite(new sprite(new Vector2(600, 200), new Vector2(500, 500)).Add_Animation("assets/textures/explosion", true, false, 60, true));
            //this.activeMap.add_sprite(new sprite(new Vector2(-400, -200), new Vector2(300, 300)).Add_Animation("assets/textures/FX_explosion/animation_explosion.png", 8, 6, true, false, 60, true));
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) {

            //test_cursor_object.transform.position = this.camera.get_uper_left_screen_corner_in_world_coordinates();
            //test_cursor_object_2.transform.position = this.camera.get_lower_right_screen_corner_in_world_coordinates();
            
            //Console.WriteLine($"resultin pos: {test_cursor_object.transform.position}");
        }

        protected override void Window_Resize() { this.camera.Set_Zoom(((float)this.window.Size.X / 3500.0f) + this.camera.zoom_offset); }

        protected override void Render(float deltaTime) { }
        
        protected override void Render_Imgui(float deltaTime) {

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

            ImGui.SetNextWindowBgAlpha(1f);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(-2, this.window.Size.Y-38), ImGuiCond.Always, new System.Numerics.Vector2(0,1));
            ImGui.Begin("HUD_BotomLeft", window_flags);

            uint col_red = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1));
            uint col_blue = ImGui.GetColorU32(new System.Numerics.Vector4(0.2f, 0.2f, 0.8f, 1));
            uint col_black = ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 1f));

            Imgui_Util.Progress_Bar_Stylised(CH_player.health / CH_player.health_max, new System.Numerics.Vector2(250, 15), col_red, col_black, 0.32f, 0.28f, 0.6f);
            ImGui.Spacing();
            Imgui_Util.Progress_Bar_Stylised(1f, new System.Numerics.Vector2(250, 15), col_blue, col_black, 0.32f, 0.28f, 0.6f);

            ImGui.Spacing();
            Imgui_Util.Title("{weapon image}");
            ImGui.SameLine();
            Imgui_Util.Title("10/10");

            ImGui.End();

        }

    }
}
