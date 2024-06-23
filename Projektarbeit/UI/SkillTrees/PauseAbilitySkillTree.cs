namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;
    using Core.defaults;
    using Projektarbeit.characters.player.abilities;

    public class PauseAbilitySkillTree : Menu
    {
        private Ability selectedAbility;
        private OmniFireAbility? omniFireAbility;
        private ShieldAbility? shieldAbility;
        private const float ButtonHeight = 50;
        private const float ButtonPadding = 10;

        private Button omniFireButton;
        private Button shieldButton;
        private Button testButton;

        private AbilityUnlockDialog unlockDialog;
        private AbilityUpgradeDialog upgradeDialog;

        public PauseAbilitySkillTree()
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

            var background = new Background(new Vector4(0f, 0f, 0f, 0.5f));
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
                () => { /* implementation */ },
                null
            );
            AddElement(testButton);

            buttonY += ButtonHeight + ButtonPadding;

            var backButton = CreateBackButton((windowSize / 2) + new Vector2(-100, buttonY - 150));
            AddElement(backButton);

            var profilePanel = new ProfilePanel(new Vector2(10, 10));
            AddElement(profilePanel);

            unlockDialog = new AbilityUnlockDialog();
            upgradeDialog = new AbilityUpgradeDialog();
        }

        public override void Render()
        {
            base.Render();

            // Update the color of the buttons
            UpdateButtonColor(omniFireButton, omniFireAbility);
            UpdateButtonColor(shieldButton, shieldAbility);
            UpdateButtonColor(testButton, null);

            // Display the dialog
            if (unlockDialog.IsOpen && selectedAbility.IsLocked)
            {
                unlockDialog.Render(selectedAbility);
            }
            else if (upgradeDialog.IsOpen && !selectedAbility.IsLocked)
            {
                upgradeDialog.Render(selectedAbility);
            }
        }

        private Button CreateButton(Vector2 position, string text, Action onClick, Ability ability)
        {
            var button = new Button(
                position, 
                new Vector2(200, 50), // Size
                text, 
                () => {
                    unlockDialog.Open();
                    upgradeDialog.Open();
                    this.selectedAbility = ability;
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
                    button.Color = new Vector4(0.4f, 0.8f, 0.4f, 1);
                }
                else if (ability != null && !ability.IsLocked)
                {
                    Console.WriteLine("Ability is not locked");
                    button.Color = new Vector4(0.8f, 0.8f, 0.4f, 1);
                }
                else {
                    button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1);
                    if (ability != null)
                    {
                        button.Label += $" (Cost to unlock: {ability.UnlockCost})";
                    }
                }
            }
            else
            {
                button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1);
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
                    button.Color = new Vector4(0.4f, 0.8f, 0.4f, 1);
                }
                else if (ability != null && !ability.IsLocked)
                {
                    button.Color = new Vector4(0.8f, 0.8f, 0.4f, 1);
                }
                else {
                    button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1);
                }
            }
            else
            {
                button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1);
            }
        }

        private Button CreateBackButton(Vector2 position)
        {
            return new Button(
                position,
                new Vector2(200, 50),
                "Back",
                () => Core.Game.Instance.play_state = Core.Play_State.PauseMenuSkillTree,
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
    }
}