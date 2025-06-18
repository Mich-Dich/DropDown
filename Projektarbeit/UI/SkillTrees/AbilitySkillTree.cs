namespace Projektarbeit.UI.SkillTrees
{
    using System.Numerics;
    using Core.UI;
    using Core.defaults;
    using Projektarbeit.characters.player.abilities;

    public class AbilitySkillTree : Menu
    {
        private Ability selectedAbility;
        private OmniFireAbility? omniFireAbility;
        private ShieldAbility? shieldAbility;
        private const float ButtonHeight = 60;
        private const float ButtonPadding = 15;

        private Button omniFireButton;
        private Button shieldButton;
        private Button backButton;

        private AbilityUnlockDialog unlockDialog;
        private AbilityUpgradeDialog upgradeDialog;

        public AbilitySkillTree()
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

            // Create a modern gradient background with consistent styling
            var background = new Background("assets/textures/title/background.jpg");
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
            var subtitleText = new Text(windowSize / 2 + new Vector2(0, -150), "Unlock and upgrade your abilities", new Vector4(0.8f, 0.8f, 0.9f, 1.0f), 1.2f)
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

            backButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + (ButtonHeight + ButtonPadding) * 2),
                new Vector2(buttonWidth, ButtonHeight),
                "BACK TO SKILL TREE",
                () => Core.Game.Instance.play_state = Core.Play_State.skill_tree,
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

            if (unlockDialog.IsOpen)
                unlockDialog.Render(selectedAbility);

            if (upgradeDialog.IsOpen)
                upgradeDialog.Render(selectedAbility);
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
                null,
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
                null,
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