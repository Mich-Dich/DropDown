
namespace Core.render {

    using Core.util;
    using ImGuiNET;
    using System.Numerics;

    public sealed class Debug_Data_Viualizer {

        public Debug_Data_Viualizer() { }

        public void Draw() {

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
			ImGui.Begin("Debug_Data_Viualizer", window_flags);

            ImGui.Text("Game general-debugger");
            ImGui.Separator();

            Imgui_Util.Begin_Table("general_debug_data");
            Imgui_Util.Add_Table_Row("update time", $"{io.Framerate:F1} FPS ({(1000.0f/ io.Framerate):F2} ms)");
            Imgui_Util.Add_Table_Row("total work time", $"{(debug_data.work_time_update + debug_data.work_time_render):F2} ms");
            Imgui_Util.Add_Table_Row("update time", $"{debug_data.work_time_update:F2} ms");
            Imgui_Util.Add_Table_Row("render time", $"{debug_data.work_time_render:F2} ms");

            Imgui_Util.Add_Table_Spacing(3);

            Imgui_Util.Add_Table_Row("tiels displayed", $"{debug_data.num_of_tiels_displayed}/{debug_data.num_of_tiels}");
            Imgui_Util.Add_Table_Row("sprite Draw calls", $"{debug_data.sprite_draw_calls_num}");
            Imgui_Util.Add_Table_Row("playing animations", $"{debug_data.playing_animation_num}");
			Imgui_Util.Add_Table_Spacing(3);
			        
            Imgui_Util.Add_Table_Row("collision checks", $"{debug_data.collision_checks_num}");
            Imgui_Util.Add_Table_Row("colidable objects", $"{debug_data.colidable_objects} [{debug_data.colidable_objects_static} static] [{debug_data.colidable_objects_dynamic} dynamic]");
            Imgui_Util.Add_Table_Row("Collisions", $"{debug_data.collision_num}");

            Imgui_Util.End_Table();

            ImGui.End();
		}

        //private bool show_window = true;

    }
}
