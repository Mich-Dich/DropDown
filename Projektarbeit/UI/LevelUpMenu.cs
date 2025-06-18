namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core;
    using Core.UI;
    using Projektarbeit.Levels;
    using Projektarbeit.characters.player;

    public class LevelUpMenu : Menu
    {
        public LevelUpMenu()
        {
            // Create a semi-transparent dark background
            var background = new Background(new Vector4(0, 0, 0, 0.8f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);

            // Create a modern title with gradient and shadow
            var levelUpText = new Text(new Vector2(windowSize.X / 2, windowSize.Y / 2 - 120), "LEVEL UP!", new Vector4(1.0f, 0.9f, 0.2f, 1.0f), 4.0f)
            {
                UseShadow = true,
                ShadowOffset = new Vector2(3, 3),
                ShadowColor = new Vector4(0, 0, 0, 0.8f),
                UseGradient = true,
                GradientColor = new Vector4(1.0f, 1.0f, 0.5f, 1.0f),
                IsBold = true,
                LetterSpacing = 3.0f
            };
            AddElement(levelUpText);

            // Create subtitle
            var subtitleText = new Text(new Vector2(windowSize.X / 2, windowSize.Y / 2 - 80), "Choose your next upgrade", new Vector4(0.9f, 0.9f, 0.9f, 1.0f), 1.8f)
            {
                UseShadow = true,
                ShadowOffset = new Vector2(1, 1),
                ShadowColor = new Vector4(0, 0, 0, 0.6f)
            };
            AddElement(subtitleText);

            // Calculate button positions with better spacing
            float buttonSpacing = 25f;
            float startY = -20f;
            float buttonWidth = 280f;
            float buttonHeight = 60f;

            var continueButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY),
                new Vector2(buttonWidth, buttonHeight),
                "CONTINUE GAME",
                () => Game.Instance.play_state = Play_State.Playing,
                true, false
            );
            AddElement(continueButton);

            var skillTreeButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY + buttonHeight + buttonSpacing),
                new Vector2(buttonWidth, buttonHeight),
                "SKILL TREE",
                () => { Game.Instance.play_state = Play_State.PauseMenuSkillTree; },
                false, false
            );
            AddElement(skillTreeButton);

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
                // Standard button - purple
                normalColor = new Vector4(0.4f, 0.2f, 0.6f, 1.0f);
                hoverColor = new Vector4(0.5f, 0.3f, 0.7f, 1.0f);
                clickColor = new Vector4(0.3f, 0.1f, 0.5f, 1.0f);
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
                BorderRadius = 12.0f,
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