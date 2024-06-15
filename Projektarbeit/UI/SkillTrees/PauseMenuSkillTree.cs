namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;

    public class PauseMenuSkillTree : Menu
    {
        public PauseMenuSkillTree()
        {
            var background = new Background(new Vector4(0, 0, 0, 0.5f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);

            var titleText = new Text(windowSize / 2 + new Vector2(0, -150), "Skill Tree", Vector4.One, 3f);
            AddElement(titleText);

            var abilityButton = CreateButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + 10) -150), "Abilities", () => NavigateToAbilities());
            AddElement(abilityButton);

            var powerupButton = CreateButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + abilityButton.Size.Y + 20) -150), "Powerups", () => NavigateToPowerups());
            AddElement(powerupButton);

            var projectileButton = CreateButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + abilityButton.Size.Y + powerupButton.Size.Y + 30) -150), "Projectiles", () => NavigateToProjectiles());
            AddElement(projectileButton);

            var backButton = CreateBackButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + abilityButton.Size.Y + powerupButton.Size.Y + projectileButton.Size.Y + 40) -150));
            AddElement(backButton);

            var profilePanel = new ProfilePanel(new Vector2(10, 10));
            AddElement(profilePanel);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreateButton(Vector2 position, string text, Action onClick)
        {
            return new Button(
                position,
                new Vector2(200, 50),
                text,
                onClick,
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }

        private Button CreateBackButton(Vector2 position)
        {
            return new Button(
                position,
                new Vector2(200, 50),
                "Back",
                () => Core.Game.Instance.play_state = Core.Play_State.LevelUp,
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }

        private void NavigateToAbilities()
        {
            // Set the game state to the AbilitySkillTree menu
            Core.Game.Instance.play_state = Core.Play_State.PauseAbilitySkillTree;
        }

        private void NavigateToPowerups()
        {
            // Implement navigation to Powerups menu
            Core.Game.Instance.play_state = Core.Play_State.PausePowerupSkillTree;
        }

        private void NavigateToProjectiles()
        {
            // Implement navigation to Projectiles menu
        }
    }
}