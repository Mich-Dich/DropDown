using Core.util;
using ImGuiNET;
using System;
using System.Numerics;
using System.Reflection.Emit;

namespace Core.imgui {

    public class debug_data_viualizer {

        public debug_data_viualizer() {

        }

        public void draw() {

			ImGuiIOPtr io = ImGui.GetIO();

			ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration 
				| ImGuiWindowFlags.NoDocking 
				| ImGuiWindowFlags.AlwaysAutoResize 
				| ImGuiWindowFlags.NoSavedSettings 
				| ImGuiWindowFlags.NoFocusOnAppearing 
				| ImGuiWindowFlags.NoNav 
				| ImGuiWindowFlags.NoMove;

			ImGui.SetNextWindowBgAlpha(0.8f);
			ImGui.SetNextWindowPos(new Vector2(20, 20));
			ImGui.Begin("debug_data_viualizer", window_flags);

            ImGui.Text("Game general-debugger");
            ImGui.Separator();

            imgui_util.begin_default_table("general_debug_data");
            imgui_util.add_table_row("update time", $"{io.Framerate:F1} FPS ({(1000.0f/ io.Framerate):F2} ms)");
            imgui_util.add_table_spacing(3);

            imgui_util.add_table_row("chunks displayed", $"{debug_data.num_of_tiels_displayed}/{debug_data.num_of_tiels}");
            imgui_util.add_table_row("sprite draw calls", $"{debug_data.sprite_draw_calls_num}");
            imgui_util.add_table_row("playing animations", $"{debug_data.playing_animation_num}");
			imgui_util.add_table_spacing(3);
			        
            imgui_util.add_table_row("collision checks", $"{debug_data.collision_checks_num}");
            imgui_util.add_table_row("colidable objects", $"{debug_data.colidable_objects} [{debug_data.colidable_objects_static} static] [{debug_data.colidable_objects_dynamic} dynamic]");
            imgui_util.add_table_row("Collisions", $"{debug_data.collision_num}");

            imgui_util.end_default_table();


            ImGui.End();





		}

        private bool show_window = true;

    }
}
