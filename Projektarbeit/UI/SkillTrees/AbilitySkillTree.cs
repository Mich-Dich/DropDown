namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;
    using Core.defaults;

    public class AbilitySkillTree : Menu
    {
        private Ability selectedAbility;
        private const float ButtonHeight = 50;
        private const float ButtonPadding = 10;

        public AbilitySkillTree()
        {
            var background = new Background(new Vector4(0.2f, 0.2f, 0.2f, 1));
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);

            var titleText = new Text(windowSize / 2 + new Vector2(0, -150), "Abilities", Vector4.One, 3f);
            AddElement(titleText);

            float buttonY = titleText.Size.Y + ButtonPadding;

            var omniFireButton = CreateButton(
                (windowSize / 2) + new Vector2(-100, buttonY - 150),
                "OmniFireAbility",
                () => { /*SelectAbility(omniFireAbility);*/ },
                null
            );
            AddElement(omniFireButton);

            buttonY += ButtonHeight + ButtonPadding;

            var shieldButton = CreateButton(
                (windowSize / 2) + new Vector2(-100, buttonY - 150),
                "ShieldAbility",
                () => { /*SelectAbility(shieldAbility);*/ },
                null
            );
            AddElement(shieldButton);

            buttonY += ButtonHeight + ButtonPadding;

            var testButton = CreateButton(
                (windowSize / 2) + new Vector2(-100, buttonY - 150),
                "TestAbility",
                () => { /*SelectAbility(testAbility);*/ },
                null
            );
            AddElement(testButton);

            buttonY += ButtonHeight + ButtonPadding;

            var backButton = CreateBackButton((windowSize / 2) + new Vector2(-100, buttonY - 150));
            AddElement(backButton);
        }

        public override void Render()
        {
            base.Render();

            // If an ability is selected, render its details
            if (selectedAbility != null)
            {
                RenderAbilityDetails();
            }
        }

        private Button CreateButton(Vector2 position, string text, Action onClick, Ability ability)
        {
            var button = new Button(
                position, 
                new Vector2(200, 50), // Size
                text, 
                () => {
                    if (ability != null)
                    {
                        if (ability.IsLocked)
                        {
                            // Open unlock dialog
                            OpenUnlockDialog(ability);
                        }
                        else
                        {
                            // Select ability
                            SelectAbility(ability);
                        }
                    }
                }, 
                null, // OnHover
                new Vector4(1, 1, 1, 1), // Color
                new Vector4(1, 1, 1, 1), // HoverColor
                new Vector4(1, 1, 1, 1), // ClickColor
                new Vector4(1, 1, 1, 1), // TextColor
                new Vector4(1, 1, 1, 1), // HoverTextColor
                new Vector4(1, 1, 1, 1)  // ClickTextColor
            );

            if (ability != null && ability.IsEquipped)
            {
                button.Color = new Vector4(0, 1, 0, 1); // Green for equipped
            }
            else if (ability != null && ability.IsUnlocked)
            {
                button.Color = new Vector4(1, 1, 0, 1); // Yellow for unlocked
            }
            else
            {
                button.Color = new Vector4(1, 0, 0, 1); // Red for locked
                if (ability != null)
                {
                    button.Label += $" (Cost to unlock: {ability.UnlockCost})";
                }
            }
            return button;
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

        private void RenderAbilityDetails()
        {
            // Create a backdrop
            var backdrop = new Background(new Vector4(0, 0, 0, 0.5f));
            AddElement(backdrop);

            // Create a window frame
            var popupWindow = new PopupWindow(new Vector2(200, 200), new Vector2(400, 400), new Vector4(1, 1, 1, 1));
            AddElement(popupWindow);

            // Add ability details to the window
            var abilityName = new Text(popupWindow.Position + new Vector2(20, 20), selectedAbility.Name, Vector4.One, 2f);
            AddElement(abilityName);

            var abilityDescription = new Text(abilityName.Position + new Vector2(0, 30), selectedAbility.Description, Vector4.One, 1f);
            AddElement(abilityDescription);

            var abilityLevel = new Text(abilityDescription.Position + new Vector2(0, 30), $"Level: {selectedAbility.Level}", Vector4.One, 1f);
            AddElement(abilityLevel);

            var upgradeButton = new Button(
                abilityLevel.Position + new Vector2(0, 50), 
                new Vector2(200, 50), // Size
                "Upgrade", 
                () => { /*selectedAbility.Upgrade();*/ }, 
                null, // OnHover
                new Vector4(1, 1, 1, 1), // Color
                new Vector4(1, 1, 1, 1), // HoverColor
                new Vector4(1, 1, 1, 1), // ClickColor
                new Vector4(1, 1, 1, 1), // TextColor
                new Vector4(1, 1, 1, 1), // HoverTextColor
                new Vector4(1, 1, 1, 1)  // ClickTextColor
            );
            AddElement(upgradeButton);

            var equipButton = new Button(
                upgradeButton.Position + new Vector2(0, 30), 
                new Vector2(200, 50), // Size
                "Equip", 
                () => Core.Game.Instance.player.Ability = selectedAbility, 
                null, // OnHover
                new Vector4(1, 1, 1, 1), // Color
                new Vector4(1, 1, 1, 1), // HoverColor
                new Vector4(1, 1, 1, 1), // ClickColor
                new Vector4(1, 1, 1, 1), // TextColor
                new Vector4(1, 1, 1, 1), // HoverTextColor
                new Vector4(1, 1, 1, 1)  // ClickTextColor
            );
            AddElement(equipButton);
        }

        private void OpenUnlockDialog(Ability ability)
        {
            // Create a backdrop
            var backdrop = new Background(new Vector4(0, 0, 0, 0.5f));
            AddElement(backdrop);

            // Create a window frame
            var popupWindow = new PopupWindow(new Vector2(200, 200), new Vector2(400, 400), new Vector4(1, 1, 1, 1));
            AddElement(popupWindow);

            // Add ability details to the window
            var abilityName = new Text(popupWindow.Position + new Vector2(20, 20), ability.Name, Vector4.One, 2f);
            AddElement(abilityName);

            var unlockText = new Text(abilityName.Position + new Vector2(0, 30), $"Unlock cost: {ability.UnlockCost}", Vector4.One, 1f);
            AddElement(unlockText);

            var unlockButton = new Button(
                unlockText.Position + new Vector2(0, 50), 
                new Vector2(200, 50), // Size
                "Unlock", 
                () => { /*ability.Unlock();*/ }, 
                null, // OnHover
                new Vector4(1, 1, 1, 1), // Color
                new Vector4(1, 1, 1, 1), // HoverColor
                new Vector4(1, 1, 1, 1), // ClickColor
                new Vector4(1, 1, 1, 1), // TextColor
                new Vector4(1, 1, 1, 1), // HoverTextColor
                new Vector4(1, 1, 1, 1)  // ClickTextColor
            );
            AddElement(unlockButton);
        }
    }
}