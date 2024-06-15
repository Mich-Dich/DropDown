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
            var background = new Background(new Vector4(0, 0, 0, 0.5f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            float fontSize = 3f;
            var buttonSize = new Vector2(200, 50);
            float padding = 10f;

            var levelUpText = new Text(new Vector2(windowSize.X / 2, windowSize.Y / 2 - buttonSize.Y * 2 - padding - fontSize), "Level Up!!!", new Vector4(0.9f, 0.9f, 0.9f, 1), fontSize);
            AddElement(levelUpText);

            var continueButton = CreateContinueButton(new Vector2(windowSize.X / 2, windowSize.Y / 2 - buttonSize.Y));
            AddElement(continueButton);

            var skillTreeButton = CreateSkillTreeButton(new Vector2(windowSize.X / 2, windowSize.Y / 2));
            AddElement(skillTreeButton);

            var profilePanel = new ProfilePanel(new Vector2(10, 10));
            AddElement(profilePanel);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreateContinueButton(Vector2 position)
        {
            var buttonSize = new Vector2(200, 50);
            return new Button(
                new Vector2(position.X - buttonSize.X / 2, position.Y - buttonSize.Y / 2),
                buttonSize,
                "Continue",
                () => Game.Instance.play_state = Play_State.Playing,
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }

        private Button CreateSkillTreeButton(Vector2 position)
        {
            var buttonSize = new Vector2(200, 50);
            return new Button(
                new Vector2(position.X - buttonSize.X / 2, position.Y - buttonSize.Y / 2),
                buttonSize,
                "Skill Tree",
                () => { Game.Instance.play_state = Play_State.PauseMenuSkillTree; },
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }
    }
}