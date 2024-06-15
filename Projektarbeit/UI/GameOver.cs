namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core;
    using Core.UI;

    public class GameOver : Menu
    {
        public GameOver()
        {
            var background = new Background(new Vector4(0, 0, 0, 0.5f));
            AddElement(background);

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            float fontSize = 8f;
            var buttonSize = new Vector2(200, 50);
            float padding = 10f;

            var gameOverText = new Text(new Vector2(windowSize.X / 2, windowSize.Y / 2 - buttonSize.Y / 2 - padding - fontSize - 80), "Game Over", new Vector4(0.9f, 0.2f, 0.2f, 1), fontSize);
            AddElement(gameOverText);

            var restartButton = CreateRestartButton(new Vector2(windowSize.X / 2, windowSize.Y / 2 + buttonSize.Y / 2));
            AddElement(restartButton);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreateRestartButton(Vector2 position)
        {
            var buttonSize = new Vector2(200, 50);
            return new Button(
                new Vector2(position.X - buttonSize.X / 2, position.Y - buttonSize.Y / 2),
                buttonSize,
                "Restart",
                () => Game.Instance.StartGame(),
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