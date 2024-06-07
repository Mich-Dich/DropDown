namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core;
    using Core.UI;

    public class GameOver : Menu
    {
        private readonly VerticalBox vbox;

        public GameOver()
        {
            var background = new Background(new Vector4(0, 0, 0, 0.5f));
            AddElement(background);

            var gameOverText = new Text(Vector2.Zero, "Game Over", new Vector4(0.9f, 0.2f, 0.2f, 1), new Vector2(100, 100));
            var restartButton = CreateRestartButton();

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            Vector2 vboxSize = new Vector2(200, 200);
            Vector2 vboxPosition = (windowSize - vboxSize) / 2;

            vbox = new VerticalBox(vboxPosition, vboxSize, new Vector2(10, 10));
            vbox.AddElement(gameOverText);
            vbox.AddElement(restartButton);

            AddElement(vbox);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreateRestartButton()
        {
            return new Button(
                Vector2.Zero,
                new Vector2(200, 50),
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
