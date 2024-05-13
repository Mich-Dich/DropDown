
namespace Core.render {

    using Core.util;
    using ImGuiNET;
    using System.Numerics;

    public sealed class DebugDataViualizer {

        public DebugDataViualizer() { }

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
			ImGui.Begin("debugDataViualizer", window_flags);

            ImGui.Text("Game general-debugger");
            ImGui.Separator();

            Imgui_Util.Begin_Table("general_Debug_Data");
            Imgui_Util.Add_Table_Row("Update time", $"{io.Framerate:F1} FPS ({(1000.0f/ io.Framerate):F2} ms)");
            Imgui_Util.Add_Table_Row("total work time", $"{(Debug_Data.workTimeUpdate + Debug_Data.workTimeRender):F2} ms");
            Imgui_Util.Add_Table_Row("Update time", $"{Debug_Data.workTimeUpdate:F2} ms");
            Imgui_Util.Add_Table_Row("render time", $"{Debug_Data.workTimeRender:F2} ms");

            Imgui_Util.Add_Table_Spacing(3);

            Imgui_Util.Add_Table_Row("tiels displayed", $"{Debug_Data.numOfTiels_Displayed}/{Debug_Data.numOfTiels}");
            Imgui_Util.Add_Table_Row("sprite Draw calls", $"{Debug_Data.spriteDrawCallsNum}");
            Imgui_Util.Add_Table_Row("playing animations", $"{Debug_Data.playingAnimationNum}");
			Imgui_Util.Add_Table_Spacing(3);
			        
            Imgui_Util.Add_Table_Row("collision checks", $"{Debug_Data.collisionChecksNum}");
            Imgui_Util.Add_Table_Row("colidable objects", $"{Debug_Data.colidableObjects} [{Debug_Data.colidableObjects_Static} static] [{Debug_Data.colidableObjects_Dynamic} dynamic]");
            Imgui_Util.Add_Table_Row("Collisions", $"{Debug_Data.collisionNum}");

            Imgui_Util.End_Table();

            ImGui.End();
		}

        //private bool show_window = true;

    }
}
