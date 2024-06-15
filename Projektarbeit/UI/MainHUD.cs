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
        }

        public void clearStatusEffects()
        {
            statusEffectsBox.elements.Clear();
        }

        private void InitializeUIElements()
        {
            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);
            Vector2 boxSize = new Vector2(100, 200);
            Vector2 padding = new Vector2(10, 10);

            float xPosition = (windowSize.X - boxSize.X) / 2;
            float yPosition = windowSize.Y - boxSize.Y - padding.Y;

            Align align = Align.Center;

            verticalBox = new VerticalBox(new Vector2(xPosition, yPosition), boxSize, padding, align);
            statusEffectsBox = new HorizontalBox(new Vector2(0, 0), new Vector2(100, 30), new Vector2(10, 10), Align.Center);
            healthBar = CreateProgressBar(() => Core.Game.Instance.player.HealthRatio, Core.Game.Instance.player.health_max, new Vector4(0.9f, 0.2f, 0.2f, 1));
            cooldownBar = CreateProgressBar(() => cooldownProgress, 100, new Vector4(0.2f, 0.2f, 0.9f, 1));

            verticalBox.AddElement(statusEffectsBox);
            verticalBox.AddElement(healthBar);
            verticalBox.AddElement(cooldownBar);

            AddElement(verticalBox);

            Vector2 scoreGoalBarSize = new Vector2(800, 20);
            Vector2 scoreGoalBarPosition = new Vector2((windowSize.X - boxSize.X - (scoreGoalBarSize.X / 2) ) / 2, padding.Y);
            scoreGoalBar = CreateProgressBar(() => Core.Game.Instance.get_active_map().ScoreRatio, Core.Game.Instance.get_active_map().scoreGoal, new Vector4(0.2f, 0.9f, 0.2f, 1), scoreGoalBarPosition, scoreGoalBarSize);

            AddElement(scoreGoalBar);
        }

        private ProgressBar CreateProgressBar(Func<float> progressFunc, float max, Vector4 color, Vector2 position, Vector2 size)
        {
            Vector4 backgroundColor = new Vector4(0.2f, 0.2f, 0.2f, 1);
            return new ProgressBar(position, size, color, backgroundColor, progressFunc, 0, max, false);
        }

        private ProgressBar CreateProgressBar(Func<float> progressFunc, float max, Vector4 color)
        {
            return new ProgressBar(new Vector2(0, 0), new Vector2(250, 15), color, new Vector4(0, 0, 0, 0), progressFunc, 0, max, false);
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
            if(Game.Instance.player.Ability != null)
            {
                var player = Core.Game.Instance.player;
                if (player.Ability.IconPath != null)
                {
                    var abilityTexture = Resource_Manager.Get_Texture(player.Ability.IconPath);
                    if (abilityTexture != null)
                    {
                        UpdateStatusEffectImage(player.Ability.IsActive, abilityTexture, player.Ability.IconPath);
                    }
                }            
            }
        }

        private void UpdatePowerUpIcons()
        {
            foreach (var powerUp in Core.Game.Instance.player.ActivePowerUps)
            {
                if (powerUp.IconPath != null)
                {
                    this.powerUpTexture = Resource_Manager.Get_Texture(powerUp.IconPath);
                    if (this.powerUpTexture != null)
                    {
                        var statusEffectImage = this.statusEffectsBox.GetElementByTextureId(this.powerUpTexture.Handle);
                        if (statusEffectImage == null)
                        {
                            statusEffectImage = new Image(this.statusEffectsBox.Position, new Vector2(30, 30), powerUp.IconPath);
                            this.statusEffectsBox.AddElement(statusEffectImage);
                        }
                    }
                }
            }
        }

        private void UpdateStatusEffectImage(bool condition, Texture texture, string path)
        {
            var statusEffectImage = statusEffectsBox.GetElementByTextureId(texture.Handle);
            if (condition)
            {
                if (statusEffectImage == null)
                {
                    statusEffectImage = new Image(statusEffectsBox.Position, new Vector2(30, 30), path);
                    statusEffectsBox.AddElement(statusEffectImage);
                    abilityIcons.Add(path);
                }
            }
            else if (statusEffectImage != null)
            {
                statusEffectsBox.RemoveElement(statusEffectImage);
                abilityIcons.Remove(path);
            }
        }

        private void RemoveUnusedIcons()
        {
            var elementsToRemove = statusEffectsBox.elements
                .Where(child => child is Image image && !Core.Game.Instance.player.ActivePowerUps.Any(p => p.IconPath == image.TexturePath) && !abilityIcons.Contains(image.TexturePath))
                .ToList();

            foreach (var element in elementsToRemove)
            {
                statusEffectsBox.RemoveElement(element);
            }
        }
    }
}