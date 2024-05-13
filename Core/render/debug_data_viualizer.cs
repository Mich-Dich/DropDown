namespace Core.render
{
    using System.Numerics;
    using Core.util;
    using ImGuiNET;

    public sealed class DebugDataViualizer
    {
        public DebugDataViualizer()
        {
        }

        public void Draw()
        {
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
            Imgui_Util.Add_Table_Row("Update time", $"{io.Framerate:F1} FPS ({1000.0f / io.Framerate:F2} ms)");
            Imgui_Util.Add_Table_Row("total work time", $"{DebugData.workTimeUpdate + DebugData.workTimeRender:F2} ms");
            Imgui_Util.Add_Table_Row("Update time", $"{DebugData.workTimeUpdate:F2} ms");
            Imgui_Util.Add_Table_Row("render time", $"{DebugData.workTimeRender:F2} ms");

            Imgui_Util.Add_Table_Spacing(3);

            Imgui_Util.Add_Table_Row("tiels displayed", $"{DebugData.numOfTielsDisplayed}/{DebugData.numOfTiels}");
            Imgui_Util.Add_Table_Row("sprite Draw calls", $"{DebugData.spriteDrawCallsNum}");
            Imgui_Util.Add_Table_Row("playing animations", $"{DebugData.playingAnimationNum}");
            Imgui_Util.Add_Table_Spacing(3);

            Imgui_Util.Add_Table_Row("collision checks", $"{DebugData.collisionChecksNum}");
            Imgui_Util.Add_Table_Row("colidable objects", $"{DebugData.colidableObjects} [{DebugData.colidableObjectsStatic} static] [{DebugData.colidableObjectsDynamic} dynamic]");
            Imgui_Util.Add_Table_Row("Collisions", $"{DebugData.collisionNum}");

            Imgui_Util.End_Table();

            ImGui.End();
        }

        // private bool show_window = true;
    }
}
