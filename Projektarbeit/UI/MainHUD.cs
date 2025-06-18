namespace Projektarbeit.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using Core;
    using Core.render;
    using Core.UI;
    using Core.util;
    using Projektarbeit.Levels;

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
        private ProgressBar waveProgressBar;
        private Background wavePanel;

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

            // Update wave bar label and progress every frame
            int waveNum = Wave.currentWave + 1;
            var currentWave = Wave.GetCurrentWave();
            float currentProgress = currentWave?.WaveProgress ?? 0f;
            
            // Create a more informative label
            string waveLabel = $"Wave {waveNum}";
            if (currentWave != null)
            {
                waveLabel += $" - {currentWave.EnemiesDefeated}/{currentWave.TotalEnemiesInWave} enemies";
            }
            
            waveProgressBar.Label = waveLabel;
            waveProgressBar.ValueProvider = () => currentWave?.WaveProgress ?? 0f;
            
            // Debug output for progress bar (only show occasionally to avoid spam)
            if (Game_Time.total % 2.0f < 0.016f) // Show roughly once per 2 seconds
            {
                if (currentWave != null)
                {
                    Console.WriteLine($"[UI] Wave {waveNum} Progress: {currentWave.EnemiesDefeated}/{currentWave.TotalEnemiesInWave} = {currentProgress * 100:F1}%");
                }
            }
        }

        public void clearStatusEffects()
        {
            statusEffectsBox.elements.Clear();
        }

        private void InitializeUIElements()
        {
            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);
            float hudPanelWidth = windowSize.X * 0.38f;
            float hudPanelHeight = 90f;
            float padding = 14f;
            float iconSize = 56f;
            float barWidth = hudPanelWidth - iconSize - 3 * padding;
            float healthBarHeight = 28f;
            float cooldownBarHeight = 18f;
            float barSpacing = 8f;

            // Wave Panel (top center)
            Vector2 wavePanelSize = new Vector2(windowSize.X * 0.45f, 48);
            Vector2 wavePanelPos = new Vector2((windowSize.X - wavePanelSize.X) / 2, 18);
            wavePanel = new Background(new Vector4(0.08f, 0.18f, 0.08f, 0.7f)) { Position = wavePanelPos, Size = wavePanelSize };
            AddElement(wavePanel);
            Vector2 waveBarSize = new Vector2(wavePanelSize.X - 2 * padding, 28);
            Vector2 waveBarPos = wavePanelPos + new Vector2(padding, 10);
            waveProgressBar = new ProgressBar(
                waveBarPos, waveBarSize,
                new Vector4(0.2f, 0.8f, 0.4f, 1), new Vector4(0.18f, 0.18f, 0.18f, 1),
                () => 0f, // Will be updated in Render()
                0, 1, false
            )
            {
                BorderColor = new Vector4(0.1f, 0.3f, 0.1f, 1),
                BorderThickness = 2.0f,
                UseGradient = true,
                GradientColor = new Vector4(0.4f, 1.0f, 0.7f, 1),
                Label = "Wave 1"
            };
            AddElement(waveProgressBar);

            // Unified HUD Panel (bottom center)
            Vector2 hudPanelPos = new Vector2((windowSize.X - hudPanelWidth) / 2, windowSize.Y - hudPanelHeight - 32);
            bottomPanel = new Background(new Vector4(0.08f, 0.08f, 0.18f, 0.82f))
            {
                Position = hudPanelPos,
                Size = new Vector2(hudPanelWidth, hudPanelHeight)
            };
            AddElement(bottomPanel);

            // Ability icon (left in panel)
            Vector2 iconPos = hudPanelPos + new Vector2(padding, (hudPanelHeight - iconSize) / 2);
            var equippedAbility = Core.Game.Instance.GameState.Abilities.FirstOrDefault(a => a.IsEquipped);
            if (equippedAbility != null && equippedAbility.IconPath != null)
            {
                var abilityTexture = Resource_Manager.Get_Texture(equippedAbility.IconPath);
                if (abilityTexture != null)
                {
                    var icon = new Image(iconPos, new Vector2(iconSize, iconSize), equippedAbility.IconPath)
                    {
                        TintColor = equippedAbility.IsActive ? new Vector4(1, 1, 1, 1) : new Vector4(0.5f, 0.5f, 0.5f, 0.7f)
                    };
                    AddElement(icon);
                }
            }

            // Health bar (stacked above cooldown bar, right of icon)
            Vector2 healthBarPos = hudPanelPos + new Vector2(iconSize + 2 * padding, padding);
            healthBar = new ProgressBar(
                healthBarPos, new Vector2(barWidth, healthBarHeight),
                new Vector4(0.9f, 0.2f, 0.2f, 1), new Vector4(0.18f, 0.18f, 0.18f, 1),
                () => Core.Game.Instance.player.HealthRatio,
                0, 1, false
            )
            {
                BorderColor = new Vector4(0.3f, 0.1f, 0.1f, 1),
                BorderThickness = 3.0f,
                UseGradient = true,
                GradientColor = new Vector4(1.0f, 0.5f, 0.5f, 1),
                Label = "Health: 100 / 100"
            };
            AddElement(healthBar);

            // Cooldown bar (below health bar, right of icon)
            Vector2 cooldownBarPos = healthBarPos + new Vector2(0, healthBarHeight + barSpacing);
            cooldownBar = new ProgressBar(
                cooldownBarPos, new Vector2(barWidth, cooldownBarHeight),
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

            // Ensure statusEffectsBox is always initialized to avoid null reference
            if (statusEffectsBox == null)
            {
                statusEffectsBox = new HorizontalBox(new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 0), Align.Center);
            }
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