namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;

    public class MainMenu : Menu
    {
        public MainMenu()
        {
            var background = new Background(new Vector4(0.2f, 0.2f, 0.2f, 1));
            AddElement(background);

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);

            var titleText = new Text(windowSize / 2 + new Vector2(0, -150), "Projektarbeit", Vector4.One, 3f);
            AddElement(titleText);

            var playButton = CreatePlayButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + 10) - 150));
            AddElement(playButton);

            var exitButton = CreateExitButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + playButton.Size.Y + 20) -150));
            AddElement(exitButton);

            var xpButton = CreateXPButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + playButton.Size.Y + exitButton.Size.Y + 30) -150));
            AddElement(xpButton);

            var skillTreeButton = CreateSkillTreeButton((windowSize / 2) + new Vector2(-100, (titleText.Size.Y + playButton.Size.Y + exitButton.Size.Y + xpButton.Size.Y + 40) -150));
            AddElement(skillTreeButton);

            var profilePanel = new ProfilePanel(new Vector2(10, 10));
            AddElement(profilePanel);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreatePlayButton(Vector2 position)
        {
            return new Button(
                position,
                new Vector2(200, 50),
                "Play",
                () => Core.Game.Instance.StartGame(),
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }

        private Button CreateExitButton(Vector2 position)
        {
            return new Button(
                position,
                new Vector2(200, 50),
                "Exit",
                () => Environment.Exit(0),
                null,
                new Vector4(0.7f, 0.2f, 0.2f, 1),
                new Vector4(0.6f, 0.1f, 0.1f, 1),
                new Vector4(0.5f, 0.1f, 0.1f, 1),
                Vector4.One,
                Vector4.One,
                Vector4.One);
        }

        private Button CreateXPButton(Vector2 position)
        {
            return new Button(
                position,
                new Vector2(200, 50),
                "Increase XP",
                () => Core.Game.Instance.GameState.AddXP(1),
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
            return new Button(
                position,
                new Vector2(200, 50),
                "Skill Tree",
                () => Core.Game.Instance.play_state = Core.Play_State.skill_tree,
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