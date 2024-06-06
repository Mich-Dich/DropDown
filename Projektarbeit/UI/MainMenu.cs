namespace Hell.UI
{
    using System.Numerics;
    using Core.UI;
    using Core.util;
    using Hell.Levels;

    public class MainMenu : Menu
    {
        private readonly Text titleText;
        private readonly Button playButton;
        private readonly Button exitButton;
        private readonly Background background;
        private readonly VerticalBox vbox;

        public MainMenu()
        {
            this.background = new Background(new Vector4(0.2f, 0.2f, 0.2f, 1));

            this.titleText = new Text(new Vector2(0, 0), "Projektarbeit", new Vector4(1, 1, 1, 1), new Vector2(100, 100));
            this.playButton = new Button(
                new Vector2(0, 0),
                new Vector2(200, 50),
                "Play",
                () => { Game.Instance.StartGame(); },
                null,
                new Vector4(0.2f, 0.7f, 0.2f, 1), // Normal color
                new Vector4(0.0f, 0.8f, 0.1f, 1), // Hover color
                new Vector4(0.1f, 0.5f, 0.1f, 1), // Click color
                new Vector4(1, 1, 1, 1),
                new Vector4(1, 1, 1, 1),
                new Vector4(1, 1, 1, 1));

            this.exitButton = new Button(
                new Vector2(0, 0),
                new Vector2(200, 50),
                "Exit",
                () => System.Environment.Exit(0),
                null,
                new Vector4(0.7f, 0.2f, 0.2f, 1),
                new Vector4(0.6f, 0.1f, 0.1f, 1),
                new Vector4(0.5f, 0.1f, 0.1f, 1),
                new Vector4(1, 1, 1, 1),
                new Vector4(1, 1, 1, 1),
                new Vector4(1, 1, 1, 1));

            Vector2 windowSize = new (Game.Instance.window.Size.X, Game.Instance.window.Size.Y);
            Vector2 vboxSize = new (200, 400);
            Vector2 vboxPosition = new ((windowSize.X - vboxSize.X) / 2, (windowSize.Y - vboxSize.Y) / 2);

            this.vbox = new VerticalBox(vboxPosition, vboxSize, new Vector2(10, 10));
            this.vbox.AddElement(this.titleText);
            this.vbox.AddElement(this.playButton);
            this.vbox.AddElement(this.exitButton);

            this.AddElement(this.background);
            this.AddElement(this.vbox);
        }

        public override void Render()
        {
            base.Render();
        }
    }
}