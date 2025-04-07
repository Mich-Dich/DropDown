namespace DropDown.UI
{
    using Core.UI;
    using ImGuiNET;
    using System.Drawing;
    using System.Numerics;

    public class UI_main_menu : Menu
    {
        private Vector2 logoSize = new Vector2(250, 200);
        private bool Credentials_Popup = false;

        private Image backgroundImage;
        private Image logo;

        public UI_main_menu()
        {
            // Hintergrund und Logo nur **einmalig** laden
            Vector2 backgroundSize = new Vector2(1920 * 0.8f, 1080); // Startwerte (werden in Render ggf. aktualisiert)
            backgroundImage = new Image(Vector2.Zero, backgroundSize, "assets/textures/Logo/GreenBackground4.png");
            logo = new Image(Vector2.Zero, logoSize, "assets/textures/Logo/Drop_Down_Logo_Darkened.png");

            AddElement(backgroundImage);
            AddElement(logo);

            custom_UI_logic_bevor_elements = () => { };
            custom_UI_logic_after_elements = () => { };
        }

        public override void Render()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoBackground;

            //---------------GreenBackground (immer zuerst zeichnen)---------------------------------------
            Vector2 imageSize = new Vector2(io.DisplaySize.X * 0.8f, io.DisplaySize.Y);
            Vector2 imagePos = new Vector2(0, 0);
            backgroundImage.Size = imageSize;

            ImGui.SetNextWindowPos(imagePos, ImGuiCond.Always);
            ImGui.SetNextWindowSize(imageSize);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

            ImGui.Begin("LeftBackgroundImage", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoBringToFrontOnFocus); // wichtig!
            ImGui.Image((IntPtr)backgroundImage.TextureId, imageSize, new Vector2(0, 1), new Vector2(1, 0));
            ImGui.End();
            ImGui.PopStyleVar(3);

            //-------Logo-----------------------------------------------------------------------------------------
            ImGui.SetNextWindowBgAlpha(0f);
            ImGui.SetNextWindowPos(new Vector2(50, 10), ImGuiCond.Always);

            ImGui.Begin("HUD", window_flags);
            ImGui.SetCursorPos(Vector2.Zero);
            ImGui.Image((IntPtr)logo.TextureId, logoSize, new Vector2(0, 1), new Vector2(1, 0));
            ImGui.End();

            //---------Settings-Buttons-------------------------------------------------------------------------

            float buttonWidth = 160;
            float buttonHeight = 40;
            float padding = 20;
            float leftX = padding;
            float startY = io.DisplaySize.Y * 0.4f;

            ImGui.SetNextWindowPos(new Vector2(leftX, startY), ImGuiCond.Always);
            ImGui.Begin("MainMenuButtons", window_flags);

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.2f, 0.2f, 1f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 1f));

            if (ImGui.Button("Credentials", new Vector2(buttonWidth, buttonHeight)))
            {
                Credentials_Popup = true;
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + padding);

            if (ImGui.Button("Settings", new Vector2(buttonWidth, buttonHeight)))
            {
                // TODO: Open settings menu
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + padding);

            if (ImGui.Button("Quit", new Vector2(buttonWidth, buttonHeight)))
            {
                System.Environment.Exit(0);
            }

            ImGui.PopStyleColor(2);
            ImGui.End();

            //---------Credentials Popup-------------------------------------------------------------------------
            if (Credentials_Popup)
            {
                ImGui.OpenPopup("Credentials");
            }

            if (ImGui.BeginPopupModal("Credentials", ref Credentials_Popup, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("von Dominik Mally, Ruwen Buchmiller");

                if (ImGui.Button("Back"))
                {
                    Credentials_Popup = false;
                }

                ImGui.EndPopup();
            }
        }
    }
}
