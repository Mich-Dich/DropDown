namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;
    using Core.defaults;
    using Projektarbeit.characters.player.abilities;
    using ImGuiNET;
    using Core.world;
    using System.Drawing;

    public class AbilitySkillTree : Menu
    {
        private Ability selectedAbility;
        private OmniFireAbility omniFireAbility;
        private ShieldAbility shieldAbility;
        private TestAbility testAbility;
        private const float ButtonHeight = 50;
        private const float ButtonPadding = 10;
        private bool displayDialog = false;

        private Button omniFireButton;
        private Button shieldButton;
        private Button testButton;

        public AbilitySkillTree()
        {
            omniFireAbility = Game.Instance.GameState.Abilities.OfType<OmniFireAbility>().FirstOrDefault();
            if (omniFireAbility == null)
            {
                omniFireAbility = new OmniFireAbility();
                Game.Instance.GameState.Abilities.Add(omniFireAbility);
            }

            shieldAbility = Game.Instance.GameState.Abilities.OfType<ShieldAbility>().FirstOrDefault();
            if (shieldAbility == null)
            {
                shieldAbility = new ShieldAbility();
                Game.Instance.GameState.Abilities.Add(shieldAbility);
            }

            testAbility = Game.Instance.GameState.Abilities.OfType<TestAbility>().FirstOrDefault();
            if (testAbility == null)
            {
                testAbility = new TestAbility();
                Game.Instance.GameState.Abilities.Add(testAbility);
            }

            var background = new Background(new Vector4(0.2f, 0.2f, 0.2f, 1));
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);

            var titleText = new Text(windowSize / 2 + new Vector2(0, -150), "Abilities", Vector4.One, 3f);
            AddElement(titleText);

            float buttonY = titleText.Size.Y + ButtonPadding;

            omniFireButton = CreateButton(
                (windowSize / 2) + new Vector2(-100, buttonY - 150),
                "OmniFireAbility",
                () => { SelectAbility(omniFireAbility); },
                omniFireAbility
            );
            AddElement(omniFireButton);

            buttonY += ButtonHeight + ButtonPadding;

            shieldButton = CreateButton(
                (windowSize / 2) + new Vector2(-100, buttonY - 150),
                "ShieldAbility",
                () => { SelectAbility(shieldAbility); },
                shieldAbility
            );
            AddElement(shieldButton);

            buttonY += ButtonHeight + ButtonPadding;

            testButton = CreateButton(
                (windowSize / 2) + new Vector2(-100, buttonY - 150),
                "TestAbility",
                () => { SelectAbility(testAbility); },
                testAbility
            );
            AddElement(testButton);

            buttonY += ButtonHeight + ButtonPadding;

            var backButton = CreateBackButton((windowSize / 2) + new Vector2(-100, buttonY - 150));
            AddElement(backButton);

            var profilePanel = new ProfilePanel(new Vector2(10, 10));
            AddElement(profilePanel);
        }

        public override void Render()
        {
            base.Render();

            // Update the color of the buttons
            UpdateButtonColor(omniFireButton, omniFireAbility);
            UpdateButtonColor(shieldButton, shieldAbility);
            UpdateButtonColor(testButton, testAbility);

            if(displayDialog && selectedAbility.IsLocked)
            {
                DisplayUnlockDialog(selectedAbility);
            }
            else if(displayDialog && !selectedAbility.IsLocked)
            {
                DisplayUpgradeDialog(selectedAbility);
            }
        }

        private Button CreateButton(Vector2 position, string text, Action onClick, Ability ability)
        {
            var button = new Button(
                position, 
                new Vector2(200, 50), // Size
                text, 
                () => {
                    this.selectedAbility = ability;
                    displayDialog = true;
                }, 
                null, // OnHover
                new Vector4(0.8f, 0.8f, 0.8f, 1), // Color
                new Vector4(0.7f, 0.7f, 0.7f, 1), // HoverColor
                new Vector4(0.6f, 0.6f, 0.6f, 1), // ClickColor
                new Vector4(0, 0, 0, 1), // TextColor
                new Vector4(0, 0, 0, 1), // HoverTextColor
                new Vector4(0, 0, 0, 1)  // ClickTextColor
            );

            // In the CreateButton method
            if (Core.Game.Instance.GameState.Abilities.Contains(ability))
            {
                if (ability != null && ability.IsEquipped)
                {
                    button.Color = new Vector4(0.4f, 0.8f, 0.4f, 1); // Subtle green for equipped
                }
                else if (ability != null && !ability.IsLocked)
                {
                    Console.WriteLine("Ability is not locked");
                    button.Color = new Vector4(0.8f, 0.8f, 0.4f, 1); // Subtle yellow for unlocked
                }
                else {
                    button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1); // Subtle red for locked
                    if (ability != null)
                    {
                        button.Label += $" (Cost to unlock: {ability.UnlockCost})";
                    }
                }
            }
            else
            {
                button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1); // Subtle red for locked
                if (ability != null)
                {
                    button.Label += $" (Cost to unlock: {ability.UnlockCost})";
                }
            }
            return button;
        }

        private void UpdateButtonColor(Button button, Ability ability)
        {
            if (Core.Game.Instance.GameState.Abilities.Contains(ability))
            {
                if (ability != null && ability.IsEquipped)
                {
                    button.Color = new Vector4(0.4f, 0.8f, 0.4f, 1); // Subtle green for equipped
                }
                else if (ability != null && !ability.IsLocked)
                {
                    button.Color = new Vector4(0.8f, 0.8f, 0.4f, 1); // Subtle yellow for unlocked
                }
                else {
                    button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1); // Subtle red for locked
                }
            }
            else
            {
                button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1); // Subtle red for locked
            }
        }

        private Button CreateBackButton(Vector2 position)
        {
            return new Button(
                position,
                new Vector2(200, 50),
                "Back",
                () => Core.Game.Instance.play_state = Core.Play_State.skill_tree,
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }

        private void SelectAbility(Ability ability)
        {
            selectedAbility = ability;
        }

        private void DisplayUpgradeDialog(Ability ability)
        {
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
                displayDialog = false;
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

            Vector2 equipSize = ImGui.CalcTextSize("Equip/Unequip");
            ImGui.SetCursorPos(new Vector2(windowSize.X - equipSize.X - 20, windowSize.Y - 60));
            if (ImGui.Button("Equip/Unequip", new Vector2(100, 50)))
            {
                // Logic for when the "Equip/Unequip" button is clicked
            }

            ImGui.End();
        }

        private void DisplayUnlockDialog(Ability ability)
        {
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
                displayDialog = false;
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