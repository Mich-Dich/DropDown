using Core.defaults;
using System.Numerics;
using ImGuiNET;

namespace Core.UI
{
    public class UnlockDialog
    {
        public bool IsOpen { get; private set; }

        public UnlockDialog()
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

        public void Render(Ability ability)
        {
            if (!IsOpen)
            {
                return;
            }

            Vector2 Size = new Vector2(400, 200);
            Vector2 Position = new Vector2((Game.Instance.window.Size.X / 2) - (Size.X / 2), (Game.Instance.window.Size.Y / 2) - (Size.Y / 2));
            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin("PopupWindow", ImGuiWindowFlags.NoDecoration);

            Vector2 windowSize = ImGui.GetWindowSize();
            Vector2 nameSize = ImGui.CalcTextSize(ability.Name);
            Vector2 descSize = ImGui.CalcTextSize(ability.Description);

            ImGui.SetCursorPos(new Vector2((windowSize.X - nameSize.X) * 0.5f, 20));
            ImGui.Text(ability.Name);

            ImGui.SetCursorPos(new Vector2((windowSize.X - descSize.X) * 0.5f, nameSize.Y + 40));
            ImGui.Text(ability.Description);

            Vector2 unlockCostSize = ImGui.CalcTextSize($"Unlock Cost: {ability.UnlockCost}");
            ImGui.SetCursorPos(new Vector2((windowSize.X - unlockCostSize.X) * 0.5f, nameSize.Y + descSize.Y + 60));
            ImGui.Text($"Unlock Cost: {ability.UnlockCost}");

            ImGui.SetCursorPos(new Vector2(10, windowSize.Y - 60));
            if (ImGui.Button("<--", new Vector2(100, 50)))
            {
                Close();
            }

            Vector2 unlockSize = ImGui.CalcTextSize("Unlock");
            ImGui.SetCursorPos(new Vector2(windowSize.X - unlockSize.X - 70, windowSize.Y - 60));
            
            if (Core.Game.Instance.GameState.Currency < ability.UnlockCost)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
            }

            if (ImGui.Button("Unlock", new Vector2(100, 50)))
            {
                if (Core.Game.Instance.GameState.Currency >= ability.UnlockCost)
                {
                    ability.Unlock();
                }
            }

            if (Core.Game.Instance.GameState.Currency < ability.UnlockCost)
            {
                ImGui.PopStyleVar();
            }

            ImGui.End();
        }
    }
}