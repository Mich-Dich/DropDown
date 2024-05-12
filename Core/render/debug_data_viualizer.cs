
namespace Core.render {

    using Core.util;
    using ImGuiNET;
    using System.Numerics;

    public sealed class debug_data_viualizer {

        public debug_data_viualizer() { }

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

            Imgui_Util.begin_default_table("general_debug_data");
            Imgui_Util.add_table_row("update time", $"{io.Framerate:F1} FPS ({(1000.0f/ io.Framerate):F2} ms)");
            Imgui_Util.add_table_row("total work time", $"{(debug_data.work_time_update + debug_data.work_time_render):F2} ms");
            Imgui_Util.add_table_row("update time", $"{debug_data.work_time_update:F2} ms");
            Imgui_Util.add_table_row("render time", $"{debug_data.work_time_render:F2} ms");

            Imgui_Util.add_table_spacing(3);

            Imgui_Util.add_table_row("tiels displayed", $"{debug_data.num_of_tiels_displayed}/{debug_data.num_of_tiels}");
            Imgui_Util.add_table_row("sprite draw calls", $"{debug_data.sprite_draw_calls_num}");
            Imgui_Util.add_table_row("playing animations", $"{debug_data.playing_animation_num}");
			Imgui_Util.add_table_spacing(3);
			        
            Imgui_Util.add_table_row("collision checks", $"{debug_data.collision_checks_num}");
            Imgui_Util.add_table_row("colidable objects", $"{debug_data.colidable_objects} [{debug_data.colidable_objects_static} static] [{debug_data.colidable_objects_dynamic} dynamic]");
            Imgui_Util.add_table_row("Collisions", $"{debug_data.collision_num}");

            Imgui_Util.end_default_table();


            ImGui.End();





		}

        //private bool show_window = true;

    }
}
