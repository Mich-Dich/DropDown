
namespace DropDown.UI {

    using Core.UI;
    using ImGuiNET;

    public class UI_main_menu : Menu {

        public UI_main_menu() {

            //AddElement(new Background(new Vector4(0.2f, 0.7f, 0.2f, 1)));

            custom_UI_logic_bevor_elements = () => {


            };

            custom_UI_logic_after_elements = () => { 
            

            };
        
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
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, io.DisplaySize.Y - 10), ImGuiCond.Always, new System.Numerics.Vector2(0, 1));

            ImGui.Begin("HUD", window_flags);



            ImGui.Text("slkdjfghksjdfghlkjsdfg");



            ImGui.End();

        }


    }
}
