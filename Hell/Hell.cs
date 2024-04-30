
namespace Hell {

    internal class Hell : Core.game {

        public Hell(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        // ========================================================= functions =========================================================
        protected override void init() {

            this.player_controller = new PC_default();
            set_update_frequency(144.0f);

            this.player = new player();
        }

        protected override void shutdown() { }

        protected override void update() { }

        protected override void render() {

        }

    }
}
