using Core.defaults;
using System.Numerics;
using ImGuiNET;

namespace Projektarbeit.UI.SkillTrees
{
    public class PowerupUnlockDialog
    {
        public bool IsOpen { get; private set; }

        public PowerupUnlockDialog()
        {
            IsOpen = false;
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void Render(PowerUp powerUp)
        {
            if (!IsOpen)
            {
                return;
            }

            Vector2 Size = new Vector2(400, 200);
            Vector2 Position = new Vector2(Core.Game.Instance.window.Size.X / 2 - Size.X / 2, Core.Game.Instance.window.Size.Y / 2 - Size.Y / 2);
            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin("PopupWindow", ImGuiWindowFlags.NoDecoration);

            Vector2 windowSize = ImGui.GetWindowSize();
            Vector2 nameSize = ImGui.CalcTextSize(powerUp.Name);
            Vector2 descSize = ImGui.CalcTextSize(powerUp.Description);

            ImGui.SetCursorPos(new Vector2((windowSize.X - nameSize.X) * 0.5f, 20));
            ImGui.Text(powerUp.Name);

            ImGui.SetCursorPos(new Vector2((windowSize.X - descSize.X) * 0.5f, nameSize.Y + 40));
            ImGui.Text(powerUp.Description);

            Vector2 unlockCostSize = ImGui.CalcTextSize($"Unlock Cost: {powerUp.UnlockCost}");
            ImGui.SetCursorPos(new Vector2((windowSize.X - unlockCostSize.X) * 0.5f, nameSize.Y + descSize.Y + 60));
            ImGui.Text($"Unlock Cost: {powerUp.UnlockCost}");

            ImGui.SetCursorPos(new Vector2(10, windowSize.Y - 60));
            if (ImGui.Button("<--", new Vector2(100, 50)))
            {
                Close();
            }

            Vector2 unlockSize = ImGui.CalcTextSize("Unlock");
            ImGui.SetCursorPos(new Vector2(windowSize.X - unlockSize.X - 70, windowSize.Y - 60));

            if (Core.Game.Instance.GameState.Currency < powerUp.UnlockCost)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
            }

            if (ImGui.Button("Unlock", new Vector2(100, 50)))
            {
                if (Core.Game.Instance.GameState.Currency >= powerUp.UnlockCost)
                {
                    powerUp.Unlock();
                }
            }

            if (Core.Game.Instance.GameState.Currency < powerUp.UnlockCost)
            {
                ImGui.PopStyleVar();
            }

            ImGui.End();
        }
    }
}