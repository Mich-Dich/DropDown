
namespace DropDown {

    using Core;
    using Core.render;
    using Core.util;
    using DropDown.player;
    using DropDown.UI;
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    internal enum Play_State {
        
        main_menu = 0,
        Playing = 1,
        dead = 2,
        hub_area = 3,
    }

    internal class Drop_Down : Core.Game {

        public Drop_Down(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        public UI_HUD HUD { get; set; }
        private UI_main_menu main_menu;

        private CH_player CH_player;
        private Play_State play_state = Play_State.main_menu;

        // ========================================================= functions =========================================================
        protected override void Init() {

            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            CH_player = new CH_player();
            this.playerController = new PC_Default(CH_player);
            this.player = CH_player;
#if true
            this.activeMap = new MAP_start();
#else
            //this.activeMap = new MAP_base();
#endif

            Set_Update_Frequency(144.0f);
            this.camera.Set_min_Max_Zoom(0.7f, 1.4f);
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
            this.camera.Set_Zoom(1.4f);

            main_menu = new UI_main_menu();
            HUD = new UI_HUD();
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

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
                case Play_State.hub_area:
            
                break;

            }
        }


        public void set_play_state(Play_State new_play_state) { play_state = new_play_state; }


    }
}
