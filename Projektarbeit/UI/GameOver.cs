using Core.UI;
using System.Numerics;
using Core.util;

namespace Hell.UI {
    public class GameOver : Menu {
        private Text gameOverText;
        private Background background;

        public GameOver() {
            background = new Background(new Vector4(0, 0, 0, 0.5f));
            gameOverText = new Text(new Vector2(Game.Instance.window.Size.X / 2, Game.Instance.window.Size.Y / 2), "Game Over", new Vector4(0.9f, 0.2f, 0.2f, 1), new Vector2(100, 100));

            AddElement(background);
            AddElement(gameOverText);
        }

        public override void Render() {
            base.Render();
        }
    }
}