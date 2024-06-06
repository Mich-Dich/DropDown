namespace Hell.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using Core.render;
    using Core.UI;
    using Core.util;

    public class MainHUD : Menu
    {
        private readonly ProgressBar healthBar;
        private readonly ProgressBar cooldownBar;
        private readonly Text scoreText;
        private readonly VerticalBox verticalBox;
        private readonly HorizontalBox statusEffectsBox;
        private float cooldownProgress;

        private Texture powerUpTexture;

        public MainHUD()
        {
            // Create a VerticalBox
            this.verticalBox = new VerticalBox(
                            new Vector2(15, Game.Instance.window.Size.Y - 150),
                            new Vector2(100, 200),
                            new Vector2(10, 10));

            this.statusEffectsBox = new HorizontalBox(
                            new Vector2(0, 0),
                            new Vector2(100, 30),
                            new Vector2(10, 10));

            // Create two ProgressBars
            this.healthBar = new ProgressBar(
                new Vector2(0, 0),
                new Vector2(250, 15),
                new Vector4(0.9f, 0.2f, 0.2f, 1),
                new Vector4(0, 0, 0, 1),
                () => Game.Instance.player.HealthRatio,
                0,
                Game.Instance.player.health_max,
                false);

            this.cooldownBar = new ProgressBar(
                new Vector2(0, 0),
                new Vector2(250, 15),
                new Vector4(0.2f, 0.2f, 0.9f, 1),
                new Vector4(0, 0, 0, 1),
                () => this.cooldownProgress,
                0,
                100,
                false);

            this.scoreText = new Text(new Vector2(0, 0), "Score");

            // Add ProgressBars to the VerticalBox
            this.verticalBox.AddElement(this.statusEffectsBox);
            this.verticalBox.AddElement(this.healthBar);
            this.verticalBox.AddElement(this.cooldownBar);
            this.verticalBox.AddElement(this.scoreText);

            // Add VerticalBox to the Menu
            this.AddElement(this.verticalBox);
        }

        public override void Render()
        {
            base.Render();

            var currentTime = Game_Time.total;
            this.cooldownProgress = (currentTime - Game.Instance.player.abilityLastUsedTime) / Game.Instance.player.Ability.Cooldown;
            this.cooldownProgress = Math.Clamp(this.cooldownProgress, 0.0f, 1.0f);

            this.scoreText.Content = $"Score: {Game.Instance.Score}";

            var player = Game.Instance.player;
            if (player.Ability.IconPath != null)
            {
                var abilityTexture = Resource_Manager.Get_Texture(player.Ability.IconPath);

                if (player.Ability.IsActive && abilityTexture != null)
                {
                    var statusEffectImage = this.statusEffectsBox.GetElementByTextureId(abilityTexture.Handle);
                    if (statusEffectImage == null)
                    {
                        statusEffectImage = new Image(this.statusEffectsBox.Position, new Vector2(30, 30), player.Ability.IconPath);
                        this.statusEffectsBox.AddElement(statusEffectImage);
                        this.abilityIcons.Add(player.Ability.IconPath);
                    }
                }
                else
                {
                    var statusEffectImage = this.statusEffectsBox.GetElementByTextureId(abilityTexture.Handle);
                    if (statusEffectImage != null)
                    {
                        this.statusEffectsBox.RemoveElement(statusEffectImage);
                        this.abilityIcons.Remove(player.Ability.IconPath);
                    }
                }
            }

            foreach (var powerUp in Game.Instance.player.ActivePowerUps)
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

            var elementsToRemove = new List<UIElement>();

            foreach (var child in this.statusEffectsBox.elements)
            {
                if (child is Image image && !Game.Instance.player.ActivePowerUps.Any(p => p.IconPath == image.TexturePath) && !this.abilityIcons.Contains(image.TexturePath))
                {
                    elementsToRemove.Add(child);
                }
            }

            foreach (var element in elementsToRemove)
            {
                this.statusEffectsBox.RemoveElement(element);
            }
        }

        private readonly List<string> abilityIcons = new ();
    }
}