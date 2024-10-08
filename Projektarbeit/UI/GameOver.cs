namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core;
    using Core.UI;
    using Projektarbeit.Levels;

    public class GameOver : Menu
    {
        public GameOver()
        {
            var background = new Background("assets/textures/background/YouDied.png");
            AddElement(background);

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            float fontSize = 8f;
            var buttonSize = new Vector2(200, 50);
            float padding = 10f;

            //var gameOverText = new Text(new Vector2(windowSize.X / 2, windowSize.Y / 2 - buttonSize.Y / 2 - padding - fontSize - 80), "Game Over", new Vector4(0.9f, 0.2f, 0.2f, 1), fontSize);
            //AddElement(gameOverText);

            var restartButton = CreateRestartButton(new Vector2((windowSize.X / 2) + 10, (windowSize.Y / 2 + buttonSize.Y / 2) + 15));
            AddElement(restartButton);

            var exitButton = CreateExitButton(new Vector2((windowSize.X / 2) + 10, (windowSize.Y / 2 + buttonSize.Y / 2) + 85));
            AddElement(exitButton);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreateRestartButton(Vector2 position)
        {
            var buttonSize = new Vector2(540, 50);
            return new Button(
                position - buttonSize / 2,
                buttonSize,
                "  ", // Button text
                () => Game.Instance.StartGame(),
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 0), // Visible color
                new Vector4(0.0f, 0.8f, 0.1f, 0), // Visible hover color
                new Vector4(0.1f, 0.5f, 0.1f, 0), // Visible click color
                Vector4.One, // Text color
                Vector4.One, // Hover text color
                Vector4.One, // Click text color
                true);
        }

        private Button CreateExitButton(Vector2 position)
        {
            var buttonSize = new Vector2(540, 50);
            return new Button(
                position - buttonSize / 2,
                buttonSize,
                " ", // Button text
                () => {
                    Game.Instance.play_state = Play_State.main_menu;
                    Game.Instance.set_active_map(new MAP_main_menu());
                },
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 0), // Visible color
                new Vector4(0.0f, 0.8f, 0.1f, 0), // Visible hover color
                new Vector4(0.1f, 0.5f, 0.1f, 0), // Visible click color
                Vector4.One, // Text color
                Vector4.One, // Hover text color
                Vector4.One, // Click text color
                true);
        }
    }
}