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
            var background = new Background(new Vector4(0.0f, 0.0f, 0.0f, 0.5f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);
            var titleText = new Text(windowSize / 2 + new Vector2(0, -150), "Powerup", Vector4.One, 3f);
            AddElement(titleText);

            float buttonY = titleText.Size.Y + ButtonPadding;
            fireRateBoostButton = CreateButton(windowSize / 2 + new Vector2(-100, buttonY - 150), "fireRateBoost", () => { SelectPowerup(fireRateBoost); }, fireRateBoost);
            AddElement(fireRateBoostButton);

            buttonY += ButtonHeight + ButtonPadding;
            healthIncreaseBoostButton = CreateButton(windowSize / 2 + new Vector2(-100, buttonY - 150), "healthIncreaseBoost", () => { SelectPowerup(healthIncreaseBoost); }, healthIncreaseBoost);
            AddElement(healthIncreaseBoostButton);

            buttonY += ButtonHeight + ButtonPadding;
            speedBoostButton = CreateButton(windowSize / 2 + new Vector2(-100, buttonY - 150), "speedBoost", () => { SelectPowerup(speedBoost); }, speedBoost);
            AddElement(speedBoostButton);

            buttonY += ButtonHeight + ButtonPadding;
            var backButton = CreateBackButton(windowSize / 2 + new Vector2(-100, buttonY - 150));
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
            var button = new Button(position, new Vector2(200, 50), text, () => { unlockDialog.Open(); upgradeDialog.Open(); selectedPowerup = powerUp; }, null, new Vector4(0.8f, 0.8f, 0.8f, 1), new Vector4(0.7f, 0.7f, 0.7f, 1), new Vector4(0.6f, 0.6f, 0.6f, 1), new Vector4(0, 0, 0, 1), new Vector4(0, 0, 0, 1), new Vector4(0, 0, 0, 1));
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
            return new Button(position, new Vector2(200, 50), "Back", () => Core.Game.Instance.play_state = Core.Play_State.PauseMenuSkillTree, null, new Vector4(0.2f, 0.7f, 0.2f, 1), new Vector4(0.0f, 0.8f, 0.1f, 1), new Vector4(0.1f, 0.5f, 0.1f, 1), Vector4.One, Vector4.One, Vector4.One);
        }

        private void SelectPowerup(PowerUp powerUp)
        {
            selectedPowerup = powerUp;
        }
    }
}
