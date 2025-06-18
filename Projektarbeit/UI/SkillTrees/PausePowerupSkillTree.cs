namespace Projektarbeit.UI.SkillTrees
{
    using System.Numerics;
    using System.Linq;
    using Core.UI;
    using Core.defaults;
    using Projektarbeit.characters.player.power_ups;

    public class PausePowerupSkillTree : Menu
    {
        private PowerUp selectedPowerup;
        private FireRateBoost? fireRateBoost;
        private HealthIncrease? healthIncreaseBoost;
        private SpeedBoost? speedBoost;
        private const float ButtonHeight = 50;
        private const float ButtonPadding = 10;

        private Button fireRateBoostButton;
        private Button healthIncreaseBoostButton;
        private Button speedBoostButton;

        private PowerupUnlockDialog unlockDialog;
        private PowerupUpgradeDialog upgradeDialog;

        public PausePowerupSkillTree()
        {
            InitializePowerUps();
            SetupUI();
        }

        private void InitializePowerUps()
        {
            fireRateBoost = InitializePowerUp<FireRateBoost>(new FireRateBoost(new OpenTK.Mathematics.Vector2(999, 999), fireDelayDecrease: 0.1f, duration: 4f));
            healthIncreaseBoost = InitializePowerUp<HealthIncrease>(new HealthIncrease(new OpenTK.Mathematics.Vector2(999, 999)));
            speedBoost = InitializePowerUp<SpeedBoost>(new SpeedBoost(new OpenTK.Mathematics.Vector2(999, 999), speedIncrease: 300f, duration: 3f));
        }

        private T InitializePowerUp<T>(T defaultPowerUp) where T : PowerUp
        {
            var powerUp = Core.Game.Instance.GameState.PowerUps.OfType<T>().FirstOrDefault();
            if (powerUp == null)
            {
                Core.Game.Instance.GameState.PowerUps.Add(defaultPowerUp);
                return defaultPowerUp;
            }
            return powerUp;
        }

        private void SetupUI()
        {
            // Create a semi-transparent overlay background
            var background = new Background(new Vector4(0.0f, 0.0f, 0.0f, 0.7f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);
            
            // Create a modern title with consistent styling
            var titleText = new Text(windowSize / 2 + new Vector2(0, -200), "POWERUPS", new Vector4(1.0f, 0.7f, 0.3f, 1.0f), 3.0f)
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
            var subtitleText = new Text(windowSize / 2 + new Vector2(0, -150), "Manage your powerups", new Vector4(0.8f, 0.8f, 0.9f, 1.0f), 1.2f)
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
                "Available PowerUps"
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

            fireRateBoostButton = CreatePowerUpButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY),
                new Vector2(buttonWidth, ButtonHeight),
                "FIRE RATE BOOST",
                () => { SelectPowerup(fireRateBoost); },
                fireRateBoost
            );
            AddElement(fireRateBoostButton);

            healthIncreaseBoostButton = CreatePowerUpButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + ButtonHeight + ButtonPadding),
                new Vector2(buttonWidth, ButtonHeight),
                "HEALTH INCREASE",
                () => { SelectPowerup(healthIncreaseBoost); },
                healthIncreaseBoost
            );
            AddElement(healthIncreaseBoostButton);

            speedBoostButton = CreatePowerUpButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + (ButtonHeight + ButtonPadding) * 2),
                new Vector2(buttonWidth, ButtonHeight),
                "SPEED BOOST",
                () => { SelectPowerup(speedBoost); },
                speedBoost
            );
            AddElement(speedBoostButton);

            var backButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + (ButtonHeight + ButtonPadding) * 3),
                new Vector2(buttonWidth, ButtonHeight),
                "BACK TO PAUSE MENU",
                () => Core.Game.Instance.play_state = Core.Play_State.InGameMenu,
                false, false
            );
            AddElement(backButton);

            var profilePanel = new ProfilePanel(new Vector2(10, 10));
            AddElement(profilePanel);

            unlockDialog = new PowerupUnlockDialog();
            upgradeDialog = new PowerupUpgradeDialog();
        }

        public override void Render()
        {
            base.Render();
            UpdateButtonColor(fireRateBoostButton, fireRateBoost);
            UpdateButtonColor(healthIncreaseBoostButton, healthIncreaseBoost);
            UpdateButtonColor(speedBoostButton, speedBoost);

            if (unlockDialog.IsOpen && selectedPowerup.IsLocked)
            {
                unlockDialog.Render(selectedPowerup);
            }
            else if (upgradeDialog.IsOpen && !selectedPowerup.IsLocked)
            {
                upgradeDialog.Render(selectedPowerup);
            }
        }

        private Button CreateButton(Vector2 position, string text, Action onClick, PowerUp powerUp)
        {
            var button = new Button(position, new Vector2(200, 50), text, () => { unlockDialog.Open(); upgradeDialog.Open(); selectedPowerup = powerUp; }, () => { }, new Vector4(0.8f, 0.8f, 0.8f, 1), new Vector4(0.7f, 0.7f, 0.7f, 1), new Vector4(0.6f, 0.6f, 0.6f, 1), new Vector4(0, 0, 0, 1), new Vector4(0, 0, 0, 1), new Vector4(0, 0, 0, 1));
            UpdateButtonLabelAndColor(button, powerUp);
            return button;
        }

        private void UpdateButtonColor(Button button, PowerUp powerUp)
        {
            if (Core.Game.Instance.GameState.PowerUps.Contains(powerUp))
            {
                if (powerUp != null && powerUp.IsEquipped)
                {
                    button.Color = new Vector4(0.4f, 0.8f, 0.4f, 1);
                }
                else if (powerUp != null && !powerUp.IsLocked)
                {
                    button.Color = new Vector4(0, 1, 0, 1);
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

        private void UpdateButtonLabelAndColor(Button button, PowerUp powerUp)
        {
            if (Core.Game.Instance.GameState.PowerUps.Contains(powerUp))
            {
                if (powerUp != null && !powerUp.IsLocked)
                {
                    button.Color = new Vector4(0.4f, 0.8f, 0.4f, 1);
                }
                else
                {
                    button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1);
                    if (powerUp != null)
                    {
                        button.Label += $" (Cost to unlock: {powerUp.UnlockCost})";
                    }
                }
            }
            else
            {
                button.Color = new Vector4(0.8f, 0.4f, 0.4f, 1);
                if (powerUp != null)
                {
                    button.Label += $" (Cost to unlock: {powerUp.UnlockCost})";
                }
            }
        }

        private Button CreateBackButton(Vector2 position)
        {
            return new Button(position, new Vector2(200, 50), "Back", () => Core.Game.Instance.play_state = Core.Play_State.PauseMenuSkillTree, () => { }, new Vector4(0.2f, 0.7f, 0.2f, 1), new Vector4(0.0f, 0.8f, 0.1f, 1), new Vector4(0.1f, 0.5f, 0.1f, 1), Vector4.One, Vector4.One, Vector4.One);
        }

        private Button CreatePowerUpButton(Vector2 position, Vector2 size, string text, Action onClick, PowerUp powerUp)
        {
            Vector4 normalColor, hoverColor, clickColor, textColor, hoverTextColor, clickTextColor;

            if (!powerUp.IsLocked)
            {
                if (powerUp.IsEquipped)
                {
                    // Equipped powerup - bright green
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
                // Locked powerup - gray
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

        private void SelectPowerup(PowerUp powerUp)
        {
            selectedPowerup = powerUp;

            if (powerUp.IsLocked)
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
