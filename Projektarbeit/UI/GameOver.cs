namespace Hell.UI
{
    using System.Numerics;
    using Core.UI;
    using Core.util;
    using Hell.Levels;
    using Core;

    public class GameOver : Menu
    {
        private readonly Text gameOverText;
        private readonly Button restartButton;
        private readonly Background background;
        private readonly VerticalBox vbox;

        public GameOver()
        {
            this.background = new Background(new Vector4(0, 0, 0, 0.5f));

            this.gameOverText = new Text(new Vector2(0, 0), "Game Over", new Vector4(0.9f, 0.2f, 0.2f, 1), new Vector2(100, 100));

            this.restartButton = new Button(
                new Vector2(0, 0),
                new Vector2(200, 50),
                "Restart",
                () => {
                    Game.Instance.StartGame();
                },
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                new Vector4(1, 1, 1, 1),
                new Vector4(1, 1, 1, 1),
                new Vector4(1, 1, 1, 1));

            Vector2 windowSize = new (Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            Vector2 vboxSize = new (200, 200);
            Vector2 vboxPosition = new ((windowSize.X - vboxSize.X) / 2, (windowSize.Y - vboxSize.Y) / 2);

            this.vbox = new VerticalBox(vboxPosition, vboxSize, new Vector2(10, 10));
            this.vbox.AddElement(this.gameOverText);
            this.vbox.AddElement(this.restartButton);

            this.AddElement(this.background);
            this.AddElement(this.vbox);
        }

        public override void Render()
        {
            base.Render();
        }
    }
}