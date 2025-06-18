namespace Projektarbeit.UI.SkillTrees
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
            omniFireAbility = Core.Game.Instance.GameState.Abilities.OfType<OmniFireAbility>().FirstOrDefault();
            if (omniFireAbility == null)
            {
                omniFireAbility = new OmniFireAbility();
                Core.Game.Instance.GameState.Abilities.Add(omniFireAbility);
            }

            shieldAbility = Core.Game.Instance.GameState.Abilities.OfType<ShieldAbility>().FirstOrDefault();
            if (shieldAbility == null)
            {
                shieldAbility = new ShieldAbility();
                Core.Game.Instance.GameState.Abilities.Add(shieldAbility);
            }

            // Create a semi-transparent overlay background
            var background = new Background(new Vector4(0f, 0f, 0f, 0.7f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);

            // Create a modern title with consistent styling
            var titleText = new Text(windowSize / 2 + new Vector2(0, -200), "ABILITIES", new Vector4(1.0f, 0.7f, 0.3f, 1.0f), 3.0f)
            {
                UseShadow = true,
                ShadowOffset = new Vector2(3, 3),
                ShadowColor = new Vector4(0, 0, 0, 0.7f),
                UseGradient = true,
                GradientColor = new Vector4(1.0f, 0.9f, 0.5f, 1.0f),
                IsBold = true,
                LetterSpacing = 3.0f
            };
            AddElement(titleText);

            // Create subtitle with consistent styling
            var subtitleText = new Text(windowSize / 2 + new Vector2(0, -150), "Manage your abilities", new Vector4(0.8f, 0.8f, 0.9f, 1.0f), 1.2f)
            {
                UseShadow = true,
                ShadowOffset = new Vector2(1, 1),
                ShadowColor = new Vector4(0, 0, 0, 0.5f)
            };
            AddElement(subtitleText);

            // Create main content card with consistent styling
            float cardWidth = 600f;
            float cardHeight = 400f;
            var mainCard = new Card(
                new Vector2((windowSize.X - cardWidth) / 2, windowSize.Y / 2 - cardHeight / 2),
                new Vector2(cardWidth, cardHeight),
                "Available Abilities"
            )
            {
                BackgroundColor = new Vector4(0.1f, 0.1f, 0.2f, 0.95f),
                GradientColor = new Vector4(0.15f, 0.1f, 0.25f, 0.95f),
                BorderColor = new Vector4(0.4f, 0.3f, 0.6f, 1.0f),
                BorderRadius = 15.0f,
                ShadowOffset = 5.0f
            };
            AddElement(mainCard);

            // Calculate button positions within the card
            float startY = windowSize.Y / 2 - cardHeight / 2 + 80f;
            float buttonWidth = 500f;

            omniFireButton = CreateAbilityButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY),
                new Vector2(buttonWidth, ButtonHeight),
                "OMNI FIRE",
                () => { SelectAbility(omniFireAbility); },
                omniFireAbility
            );
            AddElement(omniFireButton);

            shieldButton = CreateAbilityButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + ButtonHeight + ButtonPadding),
                new Vector2(buttonWidth, ButtonHeight),
                "SHIELD",
                () => { SelectAbility(shieldAbility); },
                shieldAbility
            );
            AddElement(shieldButton);

            var backButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + (ButtonHeight + ButtonPadding) * 2),
                new Vector2(buttonWidth, ButtonHeight),
                "BACK TO PAUSE MENU",
                () => Core.Game.Instance.play_state = Core.Play_State.InGameMenu,
                false, false
            );
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
                () =>
                {
                    unlockDialog.Open();
                    upgradeDialog.Open();
                    selectedAbility = ability;
                },
                () => { }, // OnHover
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
                else
                {
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
                else
                {
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
                () => { },
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }

        private Button CreateAbilityButton(Vector2 position, Vector2 size, string text, Action onClick, Ability ability)
        {
            Vector4 normalColor, hoverColor, clickColor, textColor, hoverTextColor, clickTextColor;

            if (!ability.IsLocked)
            {
                if (ability.IsEquipped)
                {
                    // Equipped ability - bright green
                    normalColor = new Vector4(0.2f, 0.8f, 0.3f, 1.0f);
                    hoverColor = new Vector4(0.3f, 0.9f, 0.4f, 1.0f);
                    clickColor = new Vector4(0.1f, 0.7f, 0.2f, 1.0f);
                }
                else
                {
                    // Unlocked but not equipped - blue
                    normalColor = new Vector4(0.2f, 0.4f, 0.8f, 1.0f);
                    hoverColor = new Vector4(0.3f, 0.5f, 0.9f, 1.0f);
                    clickColor = new Vector4(0.1f, 0.3f, 0.7f, 1.0f);
                }
            }
            else
            {
                // Locked ability - gray
                normalColor = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
                hoverColor = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
                clickColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            }

            textColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            hoverTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            clickTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            return new Button(
                position,
                size,
                text,
                onClick,
                () => { },
                normalColor,
                hoverColor,
                clickColor,
                textColor,
                hoverTextColor,
                clickTextColor
            )
            {
                BorderRadius = 12.0f,
                UseGradient = true,
                UseShadow = true,
                ShadowOffset = 3.0f,
                AnimationSpeed = 0.2f
            };
        }

        private Button CreateModernButton(Vector2 position, Vector2 size, string text, Action onClick, bool isPrimary = false, bool isDanger = false)
        {
            Vector4 normalColor, hoverColor, clickColor, textColor, hoverTextColor, clickTextColor;

            if (isPrimary)
            {
                normalColor = new Vector4(0.2f, 0.8f, 0.3f, 1.0f);
                hoverColor = new Vector4(0.3f, 0.9f, 0.4f, 1.0f);
                clickColor = new Vector4(0.1f, 0.7f, 0.2f, 1.0f);
            }
            else if (isDanger)
            {
                normalColor = new Vector4(0.8f, 0.2f, 0.2f, 1.0f);
                hoverColor = new Vector4(0.9f, 0.3f, 0.3f, 1.0f);
                clickColor = new Vector4(0.7f, 0.1f, 0.1f, 1.0f);
            }
            else
            {
                normalColor = new Vector4(0.4f, 0.2f, 0.6f, 1.0f);
                hoverColor = new Vector4(0.5f, 0.3f, 0.7f, 1.0f);
                clickColor = new Vector4(0.3f, 0.1f, 0.5f, 1.0f);
            }

            textColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            hoverTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            clickTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            return new Button(
                position,
                size,
                text,
                onClick,
                () => { },
                normalColor,
                hoverColor,
                clickColor,
                textColor,
                hoverTextColor,
                clickTextColor
            )
            {
                BorderRadius = 12.0f,
                UseGradient = true,
                UseShadow = true,
                ShadowOffset = 3.0f,
                AnimationSpeed = 0.2f,
                IsPrimary = isPrimary,
                IsDanger = isDanger
            };
        }

        private void SelectAbility(Ability ability)
        {
            selectedAbility = ability;

            if (ability.IsLocked)
            {
                unlockDialog.Open();
            }
            else
            {
                upgradeDialog.Open();
            }
        }
    }
}