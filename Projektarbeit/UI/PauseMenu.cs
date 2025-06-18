namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core;
    using Core.UI;
    using Projektarbeit.Levels;
    using Projektarbeit.characters.player;

    public class PauseMenu : Menu
    {
        public PauseMenu()
        {
            // Create a semi-transparent dark background with blur effect
            var background = new Background(new Vector4(0, 0, 0, 0.7f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);

            // Create a modern title with gradient and shadow
            var pauseText = new Text(new Vector2(windowSize.X / 2, windowSize.Y / 2 - 120), "GAME PAUSED", new Vector4(1.0f, 0.8f, 0.2f, 1.0f), 3.5f)
            {
                UseShadow = true,
                ShadowOffset = new Vector2(2, 2),
                ShadowColor = new Vector4(0, 0, 0, 0.8f),
                UseGradient = true,
                GradientColor = new Vector4(1.0f, 1.0f, 0.4f, 1.0f),
                IsBold = true,
                LetterSpacing = 2.0f
            };
            AddElement(pauseText);

            // Calculate button positions with better spacing
            float buttonSpacing = 20f;
            float startY = -30f;
            float buttonWidth = 250f;
            float buttonHeight = 55f;

            var continueButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY),
                new Vector2(buttonWidth, buttonHeight),
                "CONTINUE GAME",
                () => Game.Instance.play_state = Play_State.Playing,
                true, false
            );
            AddElement(continueButton);

            var menuButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY + buttonHeight + buttonSpacing),
                new Vector2(buttonWidth, buttonHeight),
                "MAIN MENU",
                () => {
                    Game.Instance.play_state = Play_State.main_menu;
                    Game.Instance.set_active_map(new MAP_main_menu());
                },
                false, false
            );
            AddElement(menuButton);

            var exitButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY + (buttonHeight + buttonSpacing) * 2),
                new Vector2(buttonWidth, buttonHeight),
                "EXIT GAME",
                () => Environment.Exit(0),
                false, true
            );
            AddElement(exitButton);

            var profilePanel = new ProfilePanel(new Vector2(10, 10));
            AddElement(profilePanel);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreateModernButton(Vector2 position, Vector2 size, string text, Action onClick, bool isPrimary = false, bool isDanger = false)
        {
            Vector4 normalColor, hoverColor, clickColor, textColor, hoverTextColor, clickTextColor;

            if (isPrimary)
            {
                // Primary button - bright green
                normalColor = new Vector4(0.2f, 0.8f, 0.3f, 1.0f);
                hoverColor = new Vector4(0.3f, 0.9f, 0.4f, 1.0f);
                clickColor = new Vector4(0.1f, 0.7f, 0.2f, 1.0f);
                textColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                hoverTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                clickTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else if (isDanger)
            {
                // Danger button - red
                normalColor = new Vector4(0.8f, 0.2f, 0.2f, 1.0f);
                hoverColor = new Vector4(0.9f, 0.3f, 0.3f, 1.0f);
                clickColor = new Vector4(0.7f, 0.1f, 0.1f, 1.0f);
                textColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                hoverTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                clickTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                // Standard button - blue
                normalColor = new Vector4(0.2f, 0.4f, 0.8f, 1.0f);
                hoverColor = new Vector4(0.3f, 0.5f, 0.9f, 1.0f);
                clickColor = new Vector4(0.1f, 0.3f, 0.7f, 1.0f);
                textColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                hoverTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                clickTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            }

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
                BorderRadius = 10.0f,
                UseGradient = true,
                UseShadow = true,
                ShadowOffset = 3.0f,
                AnimationSpeed = 0.2f,
                IsPrimary = isPrimary,
                IsDanger = isDanger
            };
        }
    }
}