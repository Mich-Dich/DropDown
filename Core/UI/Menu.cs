namespace Core.UI {

    using ImGuiNET;
    using System.Collections.Generic;

    public abstract class Menu {

        private List<UIElement> elements;
        public Action custom_UI_logic_bevor_elements { get; set; }
        public Action custom_UI_logic_after_elements { get; set; }

        public Menu() { elements = new List<UIElement>(); }

        public void AddElement(UIElement element) { elements.Add(element); }

        public virtual void Render() {
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

            custom_UI_logic_bevor_elements?.Invoke();

            foreach (var element in elements)
                element.Render();
             
            custom_UI_logic_after_elements?.Invoke();

            ImGui.End();
        }

    }
}