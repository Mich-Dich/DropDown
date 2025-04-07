namespace DropDown.UI {

    using Core;
    using Core.render;
    using Core.UI;
    using Core.util;
    using DropDown.player;
    using ImGuiNET;

    public class UI_death : Menu {

        private Texture death_icon;
        private CH_player player;
        private float animationTimer = 0f;
        private const float animationDuration = 2f; // 2 seconds
        private float currentScale = 200f; // Starting scale

        public UI_death() {
            death_icon = Resource_Manager.Get_Texture("assets/textures/death_icon.png");
            player = (CH_player)Game.Instance.player;
        }

        public override void Render() {

            if (animationTimer < animationDuration) {

                animationTimer += Game_Time.delta;
                float progress = Math.Min(animationTimer / animationDuration, 1f);
                currentScale = 200f + (100f * progress);
            }

            ImGuiIOPtr io = ImGui.GetIO();

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

            System.Numerics.Vector2 center = io.DisplaySize * 0.5f;
            ImGui.SetCursorPos(new System.Numerics.Vector2(center.X - currentScale * 0.5f, center.Y - 150));

            ImGui.BeginChild("PauseMenuContent", new System.Numerics.Vector2(currentScale, 300));
                ImGui.Image(death_icon.Handle, new System.Numerics.Vector2(currentScale, currentScale));
            ImGui.EndChild();

            ImGui.End();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar(2);
        }
        
        public void ResetAnimation() {
            animationTimer = 0f;
            currentScale = 200f;
        }
    }
}