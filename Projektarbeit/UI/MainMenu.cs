namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;

    public class MainMenu : Menu
    {
        private readonly VerticalBox vbox;

        public MainMenu()
        {
            var background = new Background(new Vector4(0.2f, 0.2f, 0.2f, 1));
            AddElement(background);

            var titleText = new Text(Vector2.Zero, "Projektarbeit", Vector4.One, new Vector2(100, 100));
            var playButton = CreatePlayButton();
            var exitButton = CreateExitButton();

            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);
            Vector2 vboxSize = new Vector2(200, 400);
            Vector2 vboxPosition = (windowSize - vboxSize) / 2;

            vbox = new VerticalBox(vboxPosition, vboxSize, new Vector2(10, 10));
            vbox.AddElement(titleText);
            vbox.AddElement(playButton);
            vbox.AddElement(exitButton);

            AddElement(vbox);
        }

        public override void Render()
        {
            base.Render();
        }

        private Button CreatePlayButton()
        {
            return new Button(
                Vector2.Zero,
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

        private Button CreateExitButton()
        {
            return new Button(
                Vector2.Zero,
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
    }
}
