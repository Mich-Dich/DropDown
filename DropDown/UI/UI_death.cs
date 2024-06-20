
namespace DropDown.UI {

    using Core;
    using Core.render;
    using Core.UI;
    using Core.util;
    using ImGuiNET;

    internal class UI_death : Menu {

        public Action on_timer_done;
        private float timer_start_time;
        private bool timer_running = false;
#if DEBUG
        private float duration = 5;
#else
        private float duration = 1;
#endif
        private Texture message;

        public UI_death(Action timer_done_func) {

            message = Resource_Manager.Get_Texture("assets/textures/UI/game_over.png");
            on_timer_done = timer_done_func; 
        }

        public override void Render() {

            if (!timer_running) {

                timer_running = true;
                timer_start_time = Game_Time.total;
            }

            else if(Game_Time.total >= timer_start_time + duration) {

                timer_running = false;
                Console.WriteLine($"Timer DONE"); 
                on_timer_done?.Invoke();
            }



            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoBackground;

            ImGui.SetNextWindowBgAlpha(0f);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(-10), ImGuiCond.Always, new System.Numerics.Vector2(0, 0));
            ImGui.SetNextWindowSize(util.convert_Vector<System.Numerics.Vector2>(Game.Instance.window.Size) + new System.Numerics.Vector2(20));
            ImGui.Begin("ui_HUD", window_flags);

            System.Numerics.Vector2 message_size = new System.Numerics.Vector2(600, 182);
            ImGui.SetCursorPos( (util.convert_Vector<System.Numerics.Vector2>(Game.Instance.window.Size) / 2) - (message_size/2));
            ImGui.Image(message.Handle, message_size);

            //base.Render();

            ImGui.End();

        }

    }
}
