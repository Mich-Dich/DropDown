namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;

    public class MainMenu : Menu
    {
        public MainMenu()
        {
            // Create background using the title background image
            var background = new Background("assets/textures/title/background.jpg");
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);

            // Use the actual logo aspect ratio (square: 819x819), scale to 320x320
            var logoSize = 400f;
            var logoPosition = new Vector2((windowSize.X - logoSize) / 2, windowSize.Y / 2 - logoSize - 60);
            var titleImage = new Image(
                logoPosition,
                new Vector2(logoSize, logoSize),
                "assets/textures/title/title.jpg"
            );
            AddElement(titleImage);

            // Place the first button 40px below the logo
            float buttonSpacing = 20f;
            float buttonWidth = 280f;
            float buttonHeight = 60f;
            float startY = logoPosition.Y + logoSize + 40f - windowSize.Y / 2;

            var playButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY),
                new Vector2(buttonWidth, buttonHeight),
                "PLAY GAME",
                () => Core.Game.Instance.StartGame(),
                true, false
            );
            AddElement(playButton);

            var skillTreeButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY + buttonHeight + buttonSpacing),
                new Vector2(buttonWidth, buttonHeight),
                "SKILL TREE",
                () => Core.Game.Instance.play_state = Core.Play_State.skill_tree,
                false, false
            );
            AddElement(skillTreeButton);

            var xpButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY + (buttonHeight + buttonSpacing) * 2),
                new Vector2(buttonWidth, buttonHeight),
                "ADD XP",
                () => Core.Game.Instance.GameState.AddXP(1),
                false, false
            );
            AddElement(xpButton);

            var exitButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, windowSize.Y / 2 + startY + (buttonHeight + buttonSpacing) * 3),
                new Vector2(buttonWidth, buttonHeight),
                "EXIT GAME",
                () => Environment.Exit(0),
                false, true
            );
            AddElement(exitButton);

            // Add version info at bottom
            var versionText = new Text(
                new Vector2(windowSize.X - 20, windowSize.Y - 20),
                "v1.0.0",
                new Vector4(0.6f, 0.6f, 0.7f, 0.8f),
                1.0f,
                TextAlign.Right
            )
            {
                UseShadow = true,
                ShadowOffset = new Vector2(1, 1)
            };
            AddElement(versionText);

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