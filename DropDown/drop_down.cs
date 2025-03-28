
namespace DropDown {

    using Core;
    using Core.util;
    using DropDown.player;
    using DropDown.UI;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    internal class Drop_Down : Core.Game {

        public Drop_Down(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        public UI_HUD HUD { get; set; }
        public int current_level { get; set; } = -1;
        private UI_main_menu main_menu;
        private CH_player CH_player;
        private float deathTimer = 0f;
        private bool timerActive = false;

        // ========================================================= functions =========================================================
        protected override void Init() {

            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            CH_player = new CH_player();
            this.playerController = new PC_Default(CH_player);
            this.player = CH_player;
            this.activeMap = new MAP_start();

            Set_Update_Frequency(144.0f);
            this.camera.Set_min_Max_Zoom(0.7f, 1.4f);
#if DEBUG
            set_show_performance(true);
            show_debug_data(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
            this.camera.Set_Zoom(0.04f);

            main_menu = new UI_main_menu();
            HUD = new UI_HUD();
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) {

            switch(play_state) {

                case Play_State.dead:
                    if (!timerActive) {
                        Console.WriteLine("DEATH -> starting timer");
                        deathTimer = 0f;
                        timerActive = true;
                        break;
                    }
                        
                    deathTimer += deltaTime;
                    if (deathTimer >= 1f) {                                     // After 3 seconds, switch to main menu
                        Console.WriteLine("DEATH -> finished timer");
                        set_play_state(Play_State.main_menu);
                        this.activeMap = new MAP_start();               // TODO: causes ERROR
                        timerActive = false;
                    }
                    break;
                    break; // TODO: implement timer for 3sec and then set map to main_menu
                default: break;
            }
        }

        protected override void Window_Resize() {

            base.Window_Resize();
            HUD.screen_size_buffer = util.convert_Vector(Game.Instance.window.ClientSize + new Vector2(3));
        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) {

            switch(play_state) {

                case Play_State.main_menu:
                    main_menu.Render();
                break;
                case Play_State.Playing:
                    HUD.Render();
                break;
                case Play_State.dead:
            
                break;
            }
        }

        public void set_play_state(Play_State new_play_state) { play_state = new_play_state; }

        public override void StartGame() {
            throw new NotImplementedException();
        }
    }
}
