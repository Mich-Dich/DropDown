
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
        public int current_level { get; set; } = 0;
        private UI_main_menu main_menu;
        private CH_player CH_player;
        private float deathTimer = 0f;
        private bool timerActive = false;

        private Vector2 hole_center;
        private float hole_entry_timer = 0f;
        public bool is_entering_hole = false;
        private float initial_zoom_offset = 0;
        private Vector2 camera_min_max;

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
                        is_entering_hole = true;                                // miss use to prevent movement in death
                        deathTimer = 0f;
                        timerActive = true;
                        break;
                    }
                        
                    deathTimer += deltaTime;
                    if (deathTimer >= 1f) {                                     // After 3 seconds, switch to main menu
                        Console.WriteLine("DEATH -> finished timer");
                        set_play_state(Play_State.main_menu);
                        current_level = 0;
                        Game.Instance.StartGame();
                        timerActive = false;
                    }
                    break;

                default:

                    if (is_entering_hole) {

                        hole_entry_timer += deltaTime;
                        float t = MathHelper.Clamp(hole_entry_timer / 1f, 0f, 1f);
                        CH_player.transform.position = Vector2.Lerp(CH_player.transform.position, hole_center, t);       // move player to hole center
                        CH_player.sprite.transform.size = Vector2.Lerp(new Vector2(100), Vector2.One, t);                       // scale player when enter the hole

                        float newZoom = MathHelper.Lerp(initial_zoom_offset, 2f, t);
                        camera.Set_Zoom(newZoom);

                        if (hole_entry_timer >= 1f) {

                            is_entering_hole = false;
                            camera.zoom = initial_zoom_offset;
                            camera.Set_min_Max_Zoom(camera_min_max.X, camera_min_max.Y);
                            CH_player.transform.size = new Vector2(100);
                            set_active_map(new MAP_base());
                        }
                    }
                break;
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
            this.set_active_map(new MAP_start());
            is_entering_hole = false;
            this.play_state = Play_State.Playing;
            this.player.health = 100;
            this.player.IsDead = false;
            this.player.IsRemoved = false;
            this.Score = 0;
        }

        public void player_entered_hole(Vector2 holePosition) {

            Console.WriteLine("player_entered_hole");

            is_entering_hole = true;
            hole_center = holePosition;
            hole_entry_timer = 0f;
            initial_zoom_offset = camera.zoom;
            Console.WriteLine($"camera.zoom {camera.zoom}");

            camera_min_max = this.camera.get_zoom_min_max();
            camera.Set_min_Max_Zoom(0.5f, 30);
        }
    }
}
