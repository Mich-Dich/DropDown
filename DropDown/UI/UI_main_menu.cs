
namespace DropDown.UI {

    using Core;
    using Core.render;
    using Core.UI;
    using Core.util;
    using ImGuiNET;
    using OpenTK.Mathematics;

    public class UI_main_menu : Menu {

        private Texture background;
        private Texture title;

        public UI_main_menu() {

            Vector2 window_size = Game.Instance.window.Size;
            background = Resource_Manager.Get_Texture("assets/textures/UI/main_menu_bg.png");
            title = Resource_Manager.Get_Texture("assets/textures/UI/title.png");

            this.AddElement(
                new ImageButton(
                    new System.Numerics.Vector2(280, 400),
                    new System.Numerics.Vector2(200, 57),
                    "##start_button",
                    () => { ((Drop_Down)Game.Instance).set_play_state(DropDown.Play_State.hub_area); },
                    () => { },
                    "assets/textures/UI/bu_start.png",
                    "assets/textures/UI/bu_start_hover.png",
                    "assets/textures/UI/bu_start_hover.png"
                ));

            this.AddElement(
                new ImageButton(
                    new System.Numerics.Vector2(250, window_size.Y - 390),
                    new System.Numerics.Vector2(200, 57),
                    "##settings",
                    () => { },
                    () => { },
                    "assets/textures/UI/bu_settings.png",
                    "assets/textures/UI/bu_settings_hover.png",
                    "assets/textures/UI/bu_settings_hover.png"
                ));

            this.AddElement(
                new ImageButton(
                    new System.Numerics.Vector2(225, window_size.Y - 270),
                    new System.Numerics.Vector2(200, 57),
                    "##credit",
                    () => { },
                    () => { },
                    "assets/textures/UI/bu_credit.png",
                    "assets/textures/UI/bu_credit_hover.png",
                    "assets/textures/UI/bu_credit_hover.png"
                ));

            this.AddElement(
                new ImageButton(
                    new System.Numerics.Vector2(200, window_size.Y - 150),
                    new System.Numerics.Vector2(200, 57),
                    "##exit_button",
                    () => { Game.Instance.exit_game(); },
                    () => { },
                    "assets/textures/UI/bu_exit.png",
                    "assets/textures/UI/bu_exit_hover.png",
                    "assets/textures/UI/bu_exit_hover.png"
                ));

        }

        public override void Render() {

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
            ImGui.SetNextWindowSize( util.convert_Vector<System.Numerics.Vector2>(Game.Instance.window.Size) + new System.Numerics.Vector2(20));
            ImGui.Begin("HUD", window_flags);

            System.Numerics.Vector2 pos = ImGui.GetCursorPos() + new System.Numerics.Vector2(10);
            ImGui.Image(background.Handle, new System.Numerics.Vector2(Game.Instance.window.Size.X * 0.4f, Game.Instance.window.Size.Y));

            ImGui.SetCursorPos(new System.Numerics.Vector2(100, 105));
            ImGui.Image(title.Handle, new System.Numerics.Vector2(900, 155));

            base.Render();

            ImGui.End();

        }


    }
}
