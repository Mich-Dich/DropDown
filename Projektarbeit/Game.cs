
namespace Projektarbeit {

    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;
    using Projektarbeit.Levels;

    internal class Game : Core.Game {

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }


        // ========================================================= functions =========================================================
        protected override void Init() {

            Set_Update_Frequency(144.0f);
            this.player = new CH_player();
            this.playerController = new PC_main(player);
            this.activeMap = new MAP_base();
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) { }

    }
}
