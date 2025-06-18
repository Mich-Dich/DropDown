using ImGuiNET;

namespace Core.UI
{
    public abstract class Menu
    {

        private List<UIElement> elements;
        public Action custom_UI_logic_bevor_elements { get; set; }
        public Action custom_UI_logic_after_elements { get; set; }

        public Menu() { elements = new List<UIElement>(); }

        public void AddElement(UIElement element) { elements.Add(element); }

        public virtual void Render()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            // Use a single full-screen window for the entire menu to prevent input capture issues
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoInputs; // This prevents the window from capturing input

            ImGui.SetNextWindowBgAlpha(0f);
            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero, ImGuiCond.Always);
            ImGui.SetNextWindowSize(io.DisplaySize, ImGuiCond.Always);

            ImGui.Begin("MenuContainer", window_flags);

            custom_UI_logic_bevor_elements?.Invoke();

            foreach (var element in elements)
                element.Render();

            custom_UI_logic_after_elements?.Invoke();

            ImGui.End();
        }

    }
}