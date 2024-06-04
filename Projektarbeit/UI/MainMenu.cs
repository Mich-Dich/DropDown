using Core.UI;
using System.Numerics;
using Core.util;

namespace Hell.UI {
    public class MainMenu : Menu {
        private Text titleText;
        private Button playButton;
        private Button exitButton;
        private Background background;
        private VerticalBox vbox;

        public MainMenu() {
            background = new Background(new Vector4(0.2f, 0.2f, 0.2f, 1));

            titleText = new Text(new Vector2(0, 0), "Projektarbeit", new Vector4(1, 1, 1, 1), new Vector2(100, 100));

            playButton = new Button(
                new Vector2(0, 0), 
                new Vector2(200, 50), 
                "Play", 
                () => Game.Instance.gameState = Core.GameState.Playing, 
                null, 
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                new Vector4(1, 1, 1, 1), 
                new Vector4(1, 1, 1, 1), 
                new Vector4(1, 1, 1, 1)
            );
            exitButton = new Button(new Vector2(0, 0), new Vector2(200, 50), "Exit", () => System.Environment.Exit(0), null, new Vector4(0.7f, 0.2f, 0.2f, 1), new Vector4(0.6f, 0.1f, 0.1f, 1), new Vector4(0.5f, 0.1f, 0.1f, 1), new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1));

            Vector2 windowSize = new Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            Vector2 vboxSize = new Vector2(200, 400);
            Vector2 vboxPosition = new Vector2((windowSize.X - vboxSize.X) / 2, (windowSize.Y - vboxSize.Y) / 2);
            vbox = new VerticalBox(vboxPosition, vboxSize, new Vector2(10, 10));
            vbox.AddElement(titleText);
            vbox.AddElement(playButton);
            vbox.AddElement(exitButton);

            AddElement(background);
            AddElement(vbox);
        }

        public override void Render() {
            base.Render();
        }
    }
}