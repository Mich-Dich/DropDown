using Core.defaults;
using System.Numerics;
using ImGuiNET;
using Core.world;
using Projektarbeit.characters.player.abilities;

namespace Core.UI
{
    public class UpgradeDialog
    {
        public bool IsOpen { get; private set; }

        public UpgradeDialog()
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

            Vector2 levelSize = ImGui.CalcTextSize($"Level: {ability.Level}");
            ImGui.SetCursorPos(new Vector2((windowSize.X - levelSize.X) * 0.5f, nameSize.Y + descSize.Y + 60));
            ImGui.Text($"Level: {ability.Level}");

            Vector2 upgradeCostSize = ImGui.CalcTextSize($"Upgrade Cost: {ability.UnlockCost}");
            ImGui.SetCursorPos(new Vector2((windowSize.X - upgradeCostSize.X) * 0.5f, nameSize.Y + descSize.Y + levelSize.Y + 80));
            ImGui.Text($"Upgrade Cost: {ability.UnlockCost}");

            ImGui.SetCursorPos(new Vector2(10, windowSize.Y - 60));
            if (ImGui.Button("<--", new Vector2(100, 50)))
            {
                Close();
            }

            Vector2 upgradeSize = ImGui.CalcTextSize("Upgrade");
            ImGui.SetCursorPos(new Vector2(windowSize.X - upgradeSize.X - 172, windowSize.Y - 60));
            
            if (Core.Game.Instance.GameState.Currency < ability.UnlockCost)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
            }

            if (ImGui.Button("Upgrade", new Vector2(100, 50)))
            {
                ability.Upgrade();
                int index = Core.Game.Instance.GameState.Abilities.IndexOf(ability);
                if (index != -1)
                {
                    Core.Game.Instance.GameState.Abilities[index] = ability;
                    GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
                }
            }

            if (Core.Game.Instance.GameState.Currency < ability.UnlockCost)
            {
                ImGui.PopStyleVar();
            }

            Vector2 equipSize = ImGui.CalcTextSize(ability.IsEquipped ? "Unequip" : "Equip");
            ImGui.SetCursorPos(new Vector2(290, windowSize.Y - 60));
            
            if (ImGui.Button(ability.IsEquipped ? "Unequip" : "Equip", new Vector2(100, 50)))
            {
                ability.ToggleEquip();
            }

            ImGui.End();
        }
    }
}