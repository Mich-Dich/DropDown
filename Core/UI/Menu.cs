namespace Core.UI {
    using System.Collections.Generic;
    using System.Numerics;
    using ImGuiNET;

    public abstract class Menu {
        private List<UIElement> elements;

        public Menu() {
            elements = new List<UIElement>();
        }

        public void AddElement(UIElement element) {
            elements.Add(element);
        }

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

            foreach (var element in elements) {
                element.Render();
            }

            ImGui.End();
        }
    }
}