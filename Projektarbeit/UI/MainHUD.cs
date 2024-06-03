using Core.UI;
using System.Numerics;
using Core.util;
using Core.render;

namespace Hell.UI {
    public class MainHUD : Menu {
        private ProgressBar healthBar;
        private ProgressBar cooldownBar;
        private Text scoreText;
        private VerticalBox verticalBox;
        private HorizontalBox statusEffectsBox;
        private float cooldownProgress;

        private Texture powerUpTexture;

        public MainHUD() {
            // Create a VerticalBox
            verticalBox = new VerticalBox(
                            new Vector2(15, Game.Instance.window.Size.Y - 150),
                            new Vector2(100, 200), 
                            new Vector2(10, 10)
            );

            statusEffectsBox = new HorizontalBox(
                            new Vector2(0, 0),
                            new Vector2(100, 30), 
                            new Vector2(10, 10)
            );

            // Create two ProgressBars
            healthBar = new ProgressBar(
                new Vector2(0, 0), 
                new Vector2(250, 15), 
                new Vector4(0.9f, 0.2f, 0.2f, 1), 
                new Vector4(0, 0, 0, 1), 
                () => Game.Instance.player.HealthRatio, 
                0, 
                Game.Instance.player.health_max, 
                false
            );

            cooldownBar = new ProgressBar(
                new Vector2(0, 0), 
                new Vector2(250, 15), 
                new Vector4(0.2f, 0.2f, 0.9f, 1), 
                new Vector4(0, 0, 0, 1), 
                () => cooldownProgress, 
                0, 
                100, 
                false
            );

            scoreText = new Text(new Vector2(0, 0), "Score");

            // Add ProgressBars to the VerticalBox
            verticalBox.AddElement(statusEffectsBox);
            verticalBox.AddElement(healthBar);
            verticalBox.AddElement(cooldownBar);
            verticalBox.AddElement(scoreText);

            // Add VerticalBox to the Menu
            AddElement(verticalBox);
        }

        public override void Render() {
            base.Render();
            
            var currentTime = Game_Time.total;
            cooldownProgress = (currentTime - Game.Instance.player.abilityLastUsedTime) / Game.Instance.player.Ability.Cooldown;
            cooldownProgress = Math.Clamp(cooldownProgress, 0.0f, 1.0f);

            scoreText.Content = $"Score: {Game.Instance.Score}";

            var player = Game.Instance.player;
            if (player.Ability.IconPath != null) {
                var abilityTexture = Resource_Manager.Get_Texture(player.Ability.IconPath);

                if (player.Ability.IsActive && abilityTexture != null) {
                    var statusEffectImage = statusEffectsBox.GetElementByTextureId(abilityTexture.Handle);
                    if (statusEffectImage == null) {
                        statusEffectImage = new Image(statusEffectsBox.Position, new Vector2(30, 30), player.Ability.IconPath);
                        statusEffectsBox.AddElement(statusEffectImage);
                    }
                } else {
                    var statusEffectImage = statusEffectsBox.GetElementByTextureId(abilityTexture.Handle);
                    if (statusEffectImage != null) {
                        statusEffectsBox.RemoveElement(statusEffectImage);
                    }
                }
            }

            if(Game.Instance.player.ActivePowerUp != null)
            {
                if(Game.Instance.player.ActivePowerUp.IconPath != null)
                {
                    powerUpTexture = Resource_Manager.Get_Texture(Game.Instance.player.ActivePowerUp.IconPath);
                    if(powerUpTexture != null)
                    {
                        var statusEffectImage = statusEffectsBox.GetElementByTextureId(powerUpTexture.Handle);
                        if (statusEffectImage == null) {
                            statusEffectImage = new Image(statusEffectsBox.Position, new Vector2(30, 30), Game.Instance.player.ActivePowerUp.IconPath);
                            statusEffectsBox.AddElement(statusEffectImage);
                        }
                    }
                }
            }
            else {
                if(powerUpTexture != null)
                {
                    var statusEffectImage = statusEffectsBox.GetElementByTextureId(powerUpTexture.Handle);
                    if (statusEffectImage != null) {
                        statusEffectsBox.RemoveElement(statusEffectImage);
                    }
                }
            }
            
        }
    }
}