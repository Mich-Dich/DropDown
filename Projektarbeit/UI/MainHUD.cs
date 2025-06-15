namespace Projektarbeit.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using Core;
    using Core.render;
    using Core.UI;
    using Core.util;

    public class MainHUD : Menu
    {
        private readonly List<string> abilityIcons = new();
        private ProgressBar healthBar;
        private ProgressBar cooldownBar;
        private VerticalBox verticalBox;
        private HorizontalBox statusEffectsBox;
        private float cooldownProgress;
        private ProgressBar scoreGoalBar;
        private Texture powerUpTexture;
        private Background topPanel;
        private Background leftPanel;
        private Background bottomPanel;
        private Background xpPanel;
        private Background healthPanel;
        private Background abilitiesPanel;

        public MainHUD()
        {
            InitializeUIElements();
        }

        public override void Render()
        {
            base.Render();

            UpdateCooldownProgress();
            UpdateAbilityIcons();
            UpdatePowerUpIcons();
            RemoveUnusedIcons();
            UpdateHealthBarLabel();
        }

        public void clearStatusEffects()
        {
            statusEffectsBox.elements.Clear();
        }

        private void InitializeUIElements()
        {
            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);
            Vector2 barPadding = new Vector2(12, 8);

            // XP Panel (top center)
            Vector2 xpPanelSize = new Vector2(windowSize.X * 0.45f, 48);
            Vector2 xpPanelPos = new Vector2((windowSize.X - xpPanelSize.X) / 2, 18);
            xpPanel = new Background(new Vector4(0.08f, 0.18f, 0.08f, 0.7f)) { Position = xpPanelPos, Size = xpPanelSize };
            AddElement(xpPanel);
            Vector2 xpBarSize = new Vector2(xpPanelSize.X - 2 * barPadding.X, 28);
            Vector2 xpBarPos = xpPanelPos + new Vector2(barPadding.X, 10);
            scoreGoalBar = new ProgressBar(
                xpBarPos, xpBarSize,
                new Vector4(0.2f, 0.8f, 0.4f, 1), new Vector4(0.18f, 0.18f, 0.18f, 1),
                () => Math.Clamp(Core.Game.Instance.get_active_map().ScoreRatio, 0, 1),
                0, 1, false
            )
            {
                BorderColor = new Vector4(0.1f, 0.3f, 0.1f, 1),
                BorderThickness = 2.0f,
                UseGradient = true,
                GradientColor = new Vector4(0.4f, 1.0f, 0.7f, 1),
                Label = $"XP: {Core.Game.Instance.Score} / {Core.Game.Instance.get_active_map().scoreGoal}"
            };
            AddElement(scoreGoalBar);

            // Health/Cooldown Panel (top left)
            Vector2 healthPanelSize = new Vector2(280, 80);
            Vector2 healthPanelPos = new Vector2(18, 18);
            healthPanel = new Background(new Vector4(0.18f, 0.08f, 0.08f, 0.7f)) { Position = healthPanelPos, Size = healthPanelSize };
            AddElement(healthPanel);
            Vector2 healthBarSize = new Vector2(healthPanelSize.X - 2 * barPadding.X, 28);
            Vector2 healthBarPos = healthPanelPos + new Vector2(barPadding.X, 10);
            healthBar = new ProgressBar(
                healthBarPos, healthBarSize,
                new Vector4(0.9f, 0.2f, 0.2f, 1), new Vector4(0.18f, 0.18f, 0.18f, 1),
                () => Core.Game.Instance.player.HealthRatio,
                0, 1, false
            )
            {
                BorderColor = new Vector4(0.3f, 0.1f, 0.1f, 1),
                BorderThickness = 2.0f,
                UseGradient = true,
                GradientColor = new Vector4(1.0f, 0.5f, 0.5f, 1),
                Label = "Health: 100 / 100" // Initial label, will be updated in UpdateHealthBarLabel
            };
            AddElement(healthBar);
            Vector2 cooldownBarSize = new Vector2(healthPanelSize.X - 2 * barPadding.X, 18);
            Vector2 cooldownBarPos = healthBarPos + new Vector2(0, healthBarSize.Y + 8);
            cooldownBar = new ProgressBar(
                cooldownBarPos, cooldownBarSize,
                new Vector4(0.2f, 0.2f, 0.9f, 1), new Vector4(0.18f, 0.18f, 0.18f, 1),
                () => CalculateCooldownRatio(),
                0, 1, false
            )
            {
                BorderColor = new Vector4(0.1f, 0.1f, 0.3f, 1),
                BorderThickness = 2.0f,
                UseGradient = true,
                GradientColor = new Vector4(0.5f, 0.5f, 1.0f, 1),
                Label = "Cooldown"
            };
            AddElement(cooldownBar);

            // Abilities/Powerups Panel (bottom center)
            Vector2 abilitiesPanelSize = new Vector2(windowSize.X * 0.35f, 64);
            Vector2 abilitiesPanelPos = new Vector2((windowSize.X - abilitiesPanelSize.X) / 2, windowSize.Y - 90);
            abilitiesPanel = new Background(new Vector4(0.08f, 0.08f, 0.18f, 0.7f)) { Position = abilitiesPanelPos, Size = abilitiesPanelSize };
            AddElement(abilitiesPanel);
            statusEffectsBox = new HorizontalBox(abilitiesPanelPos + new Vector2(16, 12), abilitiesPanelSize - new Vector2(32, 24), new Vector2(16, 0), Align.Center);
            AddElement(statusEffectsBox);
        }

        private float CalculateCooldownRatio()
        {
            var player = Core.Game.Instance.player;
            if (player.Ability != null && player.Ability.Cooldown > 0)
            {
                float elapsed = (float)(Game_Time.total - player.abilityLastUsedTime);
                float ratio = Math.Clamp(elapsed / player.Ability.Cooldown, 0, 1);
                return 1 - ratio; // 1 = ready, 0 = just used
            }
            return 1;
        }

        private void UpdateCooldownProgress()
        {
            if(Game.Instance.play_state == Play_State.LevelUp) { return; }
            if(Game.Instance.play_state == Play_State.InGameMenu) { return; }
            if(Game.Instance.play_state == Play_State.PauseMenuSkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PauseAbilitySkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PausePowerupSkillTree) { return; }

            var player = Core.Game.Instance.player;
                var equippedAbility = Core.Game.Instance.GameState.Abilities.FirstOrDefault(a => a.IsEquipped);

                if (equippedAbility != null)
                {
                    var currentTime = Game_Time.total;
                    cooldownProgress = (currentTime - player.abilityLastUsedTime) / player.Ability.Cooldown;
                    cooldownProgress = Math.Clamp(cooldownProgress, 0.0f, 1.0f);
                }
                else
                {
                    cooldownProgress = 0.0f;
                }
        }

        private void UpdateAbilityIcons()
        {
            var player = Core.Game.Instance.player;
            var equippedAbility = Core.Game.Instance.GameState.Abilities.FirstOrDefault(a => a.IsEquipped);

            if (equippedAbility != null && equippedAbility.IconPath != null)
            {
                var abilityTexture = Resource_Manager.Get_Texture(equippedAbility.IconPath);
                if (abilityTexture != null)
                {
                    // Check if the icon already exists
                    var existingIcon = statusEffectsBox.GetElementByTextureId(abilityTexture.Handle);
                    if (existingIcon == null)
                    {
                        // Add new icon with appropriate tint based on active state
                        var icon = new Image(statusEffectsBox.Position, new Vector2(48, 48), equippedAbility.IconPath)
                        {
                            TintColor = equippedAbility.IsActive ? new Vector4(1, 1, 1, 1) : new Vector4(0.5f, 0.5f, 0.5f, 0.7f)
                        };
                        statusEffectsBox.AddElement(icon);
                        abilityIcons.Add(equippedAbility.IconPath);
                    }
                    else if (existingIcon is Image icon)
                    {
                        // Update tint color based on active state
                        icon.TintColor = equippedAbility.IsActive ? new Vector4(1, 1, 1, 1) : new Vector4(0.5f, 0.5f, 0.5f, 0.7f);
                    }
                }
            }
        }

        private void UpdatePowerUpIcons()
        {
            var player = Core.Game.Instance.player;
            foreach (var powerUp in player.ActivePowerUps)
            {
                if (powerUp.IconPath != null)
                {
                    var powerUpTexture = Resource_Manager.Get_Texture(powerUp.IconPath);
                    if (powerUpTexture != null)
                    {
                        var existingIcon = statusEffectsBox.GetElementByTextureId(powerUpTexture.Handle);
                        if (existingIcon == null)
                        {
                            var icon = new Image(statusEffectsBox.Position, new Vector2(40, 40), powerUp.IconPath)
                            {
                                TintColor = new Vector4(1, 1, 1, 0.85f)
                            };
                            statusEffectsBox.AddElement(icon);
                        }
                    }
                }
            }
        }

        private void RemoveUnusedIcons()
        {
            var elementsToRemove = statusEffectsBox.elements
                .Where(child => child is Image image && 
                    !Core.Game.Instance.player.ActivePowerUps.Any(p => p.IconPath == image.TexturePath) && 
                    !abilityIcons.Contains(image.TexturePath))
                .ToList();

            foreach (var element in elementsToRemove)
            {
                statusEffectsBox.RemoveElement(element);
                if (element is Image image)
                {
                    abilityIcons.Remove(image.TexturePath);
                }
            }
        }

        private void UpdateHealthBarLabel()
        {
            var player = Core.Game.Instance.player;
            healthBar.Label = $"Health: {(int)player.health} / {(int)player.health_max}";
        }
    }
}