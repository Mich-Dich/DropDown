
namespace Core.render {

    using System.Numerics;
    using Core.util;
    using ImGuiNET;

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

            Imgui_Util.Begin_Table("general_DebugData");
            Imgui_Util.Add_Table_Row("update_internal time", $"{io.Framerate,6:0.00} FPS ({1000.0f / io.Framerate,6:0.00} ms)");

            Imgui_Util.Add_Table_Row("total work time", $"{DebugData.workTimeUpdate + DebugData.workTimeRender,6:0.00} ms");
            Imgui_Util.Add_Table_Row("update_internal time", $"{DebugData.workTimeUpdate, 6:0.00} ms");
            Imgui_Util.Add_Table_Row("render time", $"{DebugData.workTimeRender,6:0.00} ms");

            Imgui_Util.Add_Table_Spacing(3);

            Imgui_Util.Add_Table_Row("tiels displayed", $"{DebugData.numOfTielsDisplayed}/{DebugData.numOfTiels}");
            Imgui_Util.Add_Table_Row("sprite Draw calls", $"{DebugData.spriteDrawCallsNum}");
            Imgui_Util.Add_Table_Row("playing animations", $"{DebugData.playingAnimationNum}");
            Imgui_Util.Add_Table_Row("colidable objects", $"{DebugData.colidableObjectsStatic + DebugData.colidableObjectsDynamic} [{DebugData.colidableObjectsStatic} static] [{DebugData.colidableObjectsDynamic} dynamic]");
            
            Imgui_Util.Add_Table_Spacing(3);
            
            Imgui_Util.Add_Table_Row("debug_shapes", $"{DebugData.debug_lines + DebugData.debug_circle + DebugData.debug_rectangle}");
            Imgui_Util.Add_Table_Row(" - lines", $"{DebugData.debug_lines}");
            Imgui_Util.Add_Table_Row(" - circle", $"{DebugData.debug_circle}");
            Imgui_Util.Add_Table_Row(" - rectangle", $"{DebugData.debug_rectangle}");

            Imgui_Util.End_Table();

            ImGui.End();
        }

    }
}


/*
            debug_circle = 0;
            debug_rectangle = 0;
*/