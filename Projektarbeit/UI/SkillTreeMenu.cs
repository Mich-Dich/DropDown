namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;

    public class SkillTreeMenu : Menu
    {
        public SkillTreeMenu()
        {
            // Create background using the title background image for consistency
            var background = new Background("assets/textures/title/background.jpg");
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);

            // Create subtitle
            var subtitleText = new Text(windowSize / 2 + new Vector2(0, -150), "Choose Your Path", new Vector4(0.8f, 0.8f, 0.9f, 1.0f), 1.5f)
            {
                UseShadow = true,
                ShadowOffset = new Vector2(1, 1),
                ShadowColor = new Vector4(0, 0, 0, 0.5f)
            };
            AddElement(subtitleText);

            // Use the same button layout as the main menu
            float buttonSpacing = 20f;
            float buttonWidth = 280f;
            float buttonHeight = 60f;
            // Place the first button at a similar offset as the main menu (centered, below where the logo would be)
            float startY = windowSize.Y / 2 - buttonHeight * 2;

            var abilityButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY),
                new Vector2(buttonWidth, buttonHeight),
                "ABILITIES",
                () => NavigateToAbilities(),
                true, false
            );
            AddElement(abilityButton);

            var powerupButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + buttonHeight + buttonSpacing),
                new Vector2(buttonWidth, buttonHeight),
                "POWERUPS",
                () => NavigateToPowerups(),
                false, false
            );
            AddElement(powerupButton);

            var projectileButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + (buttonHeight + buttonSpacing) * 2),
                new Vector2(buttonWidth, buttonHeight),
                "PROJECTILES",
                () => NavigateToProjectiles(),
                false, false
            );
            AddElement(projectileButton);

            var backButton = CreateModernButton(
                new Vector2((windowSize.X - buttonWidth) / 2, startY + (buttonHeight + buttonSpacing) * 3),
                new Vector2(buttonWidth, buttonHeight),
                "BACK TO MENU",
                () => Core.Game.Instance.play_state = Core.Play_State.main_menu,
                false, false
            );
            AddElement(backButton);

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
                // Primary button - orange/gold
                normalColor = new Vector4(0.8f, 0.5f, 0.1f, 1.0f);
                hoverColor = new Vector4(0.9f, 0.6f, 0.2f, 1.0f);
                clickColor = new Vector4(0.7f, 0.4f, 0.0f, 1.0f);
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
                BorderRadius = 15.0f,
                UseGradient = true,
                UseShadow = true,
                ShadowOffset = 4.0f,
                AnimationSpeed = 0.25f,
                IsPrimary = isPrimary,
                IsDanger = isDanger
            };
        }

        private void NavigateToAbilities()
        {
            Core.Game.Instance.play_state = Core.Play_State.ability_skill_tree;
        }

        private void NavigateToPowerups()
        {
            Core.Game.Instance.play_state = Core.Play_State.powerup_skill_tree;
        }

        private void NavigateToProjectiles()
        {
            // Implement navigation to Projectiles menu
        }
    }
}