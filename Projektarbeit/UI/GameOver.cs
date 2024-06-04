using Core.UI;
using System.Numerics;
using Core.util;

namespace Hell.UI {
    public class GameOver : Menu {
        private Text gameOverText;
        private Button restartButton;
        private Background background;
        private VerticalBox vbox;

        public GameOver() {
            background = new Background(new Vector4(0, 0, 0, 0.5f));

            gameOverText = new Text(new Vector2(0, 0), "Game Over", new Vector4(0.9f, 0.2f, 0.2f, 1), new Vector2(100, 100));

            restartButton = new Button(
                new Vector2(0, 0), 
                new Vector2(200, 50), 
                "Restart", 
                () => Game.Instance.Restart(), 
                null, 
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                new Vector4(1, 1, 1, 1), 
                new Vector4(1, 1, 1, 1), 
                new Vector4(1, 1, 1, 1)
            );

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            Vector2 vboxSize = new Vector2(200, 200);
            Vector2 vboxPosition = new Vector2((windowSize.X - vboxSize.X) / 2, (windowSize.Y - vboxSize.Y) / 2);
            vbox = new VerticalBox(vboxPosition, vboxSize, new Vector2(10, 10));
            vbox.AddElement(gameOverText);
            vbox.AddElement(restartButton);

            AddElement(background);
            AddElement(vbox);
        }

        public override void Render() {
            base.Render();
        }
    }
}