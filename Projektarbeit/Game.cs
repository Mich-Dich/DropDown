namespace Hell {
    using ImGuiNET;
    using Core.util;
    using Hell.player;
    using Hell.Levels;
    using Core.UI;
    using Hell.UI;

    internal class Game : Core.Game {
        private bool isGameOver = false;
        private Text testText;
        private Button testButton;
        private Input testInput;
        private Image testImage;
        private ProgressBar healthBar;
        private TestMenu testMenu;

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        // ========================================================= functions =========================================================
        protected override void Init() {
            Set_Update_Frequency(144.0f);
            this.activeMap = new MAP_base();
            this.player = new CH_player();
            this.playerController = new PC_main(player);
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
            testText = new Text("Test Text");
            testButton = new Button(
                            new System.Numerics.Vector2(0, 300), // position
                            new System.Numerics.Vector2(100, 50), // size
                            "Test Button", // label
                            () => Console.WriteLine("Button was clicked!"), // onClick
                            () => Console.WriteLine("Button was hovered!"), // onHover
                            new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 1.0f), // color
                            new System.Numerics.Vector4(0.3f, 0.3f, 0.3f, 1.0f), // hoverColor
                            new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 1.0f), // clickColor
                            new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f), // textColor
                            new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f), // hoverTextColor
                            new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f)  // clickTextColor
            );
        testInput = new Input(
            new System.Numerics.Vector2(200, 300), // position
            new System.Numerics.Vector2(200, 50), // size
            "Test Input", // initial text
            new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f), // color
            text => Console.WriteLine($"Input text changed to: {text}") // onTextChanged
        );

        testImage = new Image(new System.Numerics.Vector2(0, 400), "assets/textures/abilities/fireboost.png");

        healthBar = new ProgressBar(
            new System.Numerics.Vector2(0, 500), // position
            new System.Numerics.Vector2(250, 15), // size
            new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1.0f), // fill color
            new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 1.0f), // background color
            () => this.player.health, // value provider
            0, // min value
            100, // max value
            false // show percentage text
        );

        testMenu = new TestMenu();
#endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) {
            if (this.player.health <= 0) {
                isGameOver = true;
            }
        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) {
            ImGuiIOPtr io = ImGui.GetIO();

            if (isGameOver) {
                ImGui.GetBackgroundDrawList().AddRectFilled(new System.Numerics.Vector2(0, 0), io.DisplaySize, ImGui.GetColorU32(new System.Numerics.Vector4(0, 0, 0, 0.4f)));

                ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0));
                ImGui.SetNextWindowSize(io.DisplaySize);

                ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                    | ImGuiWindowFlags.NoDocking
                    | ImGuiWindowFlags.NoSavedSettings
                    | ImGuiWindowFlags.NoFocusOnAppearing
                    | ImGuiWindowFlags.NoBringToFrontOnFocus
                    | ImGuiWindowFlags.NoNav
                    | ImGuiWindowFlags.NoMove
                    | ImGuiWindowFlags.NoBackground;

                ImGui.Begin("Game Over", window_flags);

                ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f));

                ImGui.PushFont(io.Fonts.Fonts[2]);

                var textSize = ImGui.CalcTextSize("Game Over");

                ImGui.SetCursorPos(new System.Numerics.Vector2((io.DisplaySize.X - textSize.X) / 2, (io.DisplaySize.Y - textSize.Y) / 2));
                ImGui.Text("Game Over");

                ImGui.PopFont();
                ImGui.PopStyleColor();
                ImGui.End();
            } else {
                // HUD
                ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                    | ImGuiWindowFlags.NoDocking
                    | ImGuiWindowFlags.AlwaysAutoResize
                    | ImGuiWindowFlags.NoSavedSettings
                    | ImGuiWindowFlags.NoFocusOnAppearing
                    | ImGuiWindowFlags.NoNav
                    | ImGuiWindowFlags.NoMove
                    | ImGuiWindowFlags.NoBackground;

                ImGui.SetNextWindowBgAlpha(0f);
                ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, io.DisplaySize.Y - 10), ImGuiCond.Always, new System.Numerics.Vector2(0, 1));

                ImGui.Begin("HUD_TopLeft", window_flags);

                testMenu.Render();

                if (player.Ability.IconPath != null) {
                    var abilityTexture = Resource_Manager.Get_Texture(player.Ability.IconPath);

                    System.Numerics.Vector2 uv0 = new System.Numerics.Vector2(0, 1);
                    System.Numerics.Vector2 uv1 = new System.Numerics.Vector2(1, 0);

                    if (player.Ability.IsActive && abilityTexture != null) {
                        ImGui.Image(abilityTexture.Handle, new System.Numerics.Vector2(30, 30), uv0, uv1);
                    }
                }

                uint col_red = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1));
                uint transparentColor = ImGui.GetColorU32(new System.Numerics.Vector4(0, 0, 0, 0));
                uint col_blue = ImGui.GetColorU32(new System.Numerics.Vector4(0.2f, 0.2f, 0.9f, 1));

                float healthBarWidth = 250 * (this.player.health_max / 100f);
                Imgui_Util.Progress_Bar_Stylised(this.player.HealthRatio, new System.Numerics.Vector2(healthBarWidth, 15), col_red, transparentColor, 0.32f, 0.28f, 0.6f);

                var currentTime = Game_Time.total;
                float cooldownProgress = (currentTime - this.player.abilityLastUsedTime) / this.player.Ability.Cooldown;
                cooldownProgress = Math.Clamp(cooldownProgress, 0.0f, 1.0f);
                Imgui_Util.Progress_Bar_Stylised(cooldownProgress, new System.Numerics.Vector2(250, 15), col_blue, transparentColor, 0.32f, 0.28f, 0.6f);

                ImGui.Spacing();
                Imgui_Util.Title($"Score: {Game.Instance.Score}");
                ImGui.End();
            }
        }
    }
}