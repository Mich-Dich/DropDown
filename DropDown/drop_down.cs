
#define DISPLAY_DEBUG

namespace DropDown {

    using Core;
    using Core.util;
    using DropDown.player;
    using DropDown.UI;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public enum Game_State {
        
        main_menu = 0,
        Playing = 1,
        dead = 2,
        hub_area = 3,
    }


    public struct ranged_int {
        public int current;
        public int min;
        public int max;

        public ranged_int(Int32 current, Int32 min, Int32 max) { this.current = current; this.min = min; this.max = max; }
        public ranged_int(Int32 current, Int32 max) { this.current = current; this.min = current; this.max = max; }

        public int get_section(int divisions = 5) { return (current - min) / ((max - min) / divisions); }
        public void add_section(int divisions = 5) { current += ((max - min) / divisions); }
    }

    public struct ranged_float {
        public float current;
        public float min;
        public float max;

        public ranged_float(Single current, Single min, Single max) { this.current = current; this.min = min; this.max = max; }
        public ranged_float(Single current, Single max) { this.current = current; this.min = current; this.max = max; }

        public int get_section(int divisions = 5) { return (int) ((current - min) / ((max - min) / divisions)); }
        public void add_section(int divisions = 5) { current += ((max - min) / divisions); }
    }



    internal static class projectile_data {
        
        //              Type            Name                                    value range         Steps
        internal static ranged_int      knockback   = new( 1, 10 );         //  1 - 10              2
        internal static ranged_int      speed       = new( 1000, 3000 );    //  1000 - 3000         400
        internal static ranged_int      damage      = new( 25, 275 );                   //  25 - undefined      
        internal static ranged_float    lifespan    = new( 0.5f, 3);        //  0.5 - 3             0.5
        internal static ranged_float    cooldown    = new( 1.5f, 1.5f, 0.4f);     //  1.5 - 0.25          0.25
    }

    internal static class  player_data {

        internal static int speed = 3000;
    }






    internal class Drop_Down : Game {

        public Drop_Down(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        public int current_drop_level = 0; // current level the player is on (0 => Hub area, 1 => Dungon entrance)
        public UI_HUD ui_HUD { get; set; }
        public float buffer_zoom { get; set; } = 0.7f;

        private UI_main_menu ui_main_menu;
        private UI_death ui_death;
        private UI_hub ui_hub;

        MAP_start start_map;

        private CH_player CH_player;
        private Game_State play_state = Game_State.main_menu;
        public Sound battle_music;

        // ========================================================= functions =========================================================
        protected override void Init() {
            
            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            Set_Update_Frequency(144.0f);
            
            CH_player = new CH_player();
            this.player = CH_player;
            this.playerController = new PC_empty();

            start_map = new MAP_start();
            this.activeMap = start_map;

#if DEBUG
#if DISPLAY_DEBUG
            Show_Performance(true);
            showDebugData(true);
#endif
            this.playerController = new PC_hub(CH_player);
            play_state = Game_State.hub_area;
            this.camera.Set_Zoom(0.7f);
#else
            this.camera.Set_Zoom(0.7f);
#endif
            this.camera.Set_min_Max_Zoom(0.03f, 5f);
            this.camera.transform.position = new Vector2(-300, -300);

            ui_main_menu = new UI_main_menu();
            ui_HUD = new UI_HUD();
            ui_hub = new UI_hub(CH_player);
            ui_death = new UI_death(() => { Console.WriteLine($"Execute Function"); set_play_state(Game_State.hub_area); });

            battle_music = new Sound("assets/sounds/battle-sword.wav", 9);
        }

        protected override void Shutdown() { 
        
            battle_music.Stop();
        }

        protected override void Update(float deltaTime) { }

        protected override void Window_Resize() {

            base.Window_Resize();
            ui_HUD.screen_size_buffer = util.convert_Vector<System.Numerics.Vector2>(Game.Instance.window.ClientSize + new OpenTK.Mathematics.Vector2(3));
        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) {

            switch(play_state) {

                case Game_State.main_menu:
                    ui_main_menu.Render();
                    break;
                case Game_State.Playing:
                    ui_HUD.Render();
                    break;
                case Game_State.dead:
                    ui_death.Render();
                    break;
                case Game_State.hub_area:
                    ui_hub.Render();
                    break;
            }
        }

        public void set_play_state(Game_State new_play_state) {

            switch(new_play_state) {
                case Game_State.main_menu: {

                } break;

                case Game_State.Playing: {

                    this.playerController = new PC_Default(CH_player);
                    battle_music.Play();
                } break;
            
                case Game_State.dead: {

                } break;

                case Game_State.hub_area: {

                    CH_player.health = CH_player.health_max;
                    current_drop_level = 0;
                    ui_HUD.reset_blood_overlay();
                    this.playerController = new PC_hub(CH_player);
                    this.set_active_map(new MAP_start());
                } break;
            }
            play_state = new_play_state; 
        }

        public override void StartGame() {
            throw new NotImplementedException();
        }

    }
}
