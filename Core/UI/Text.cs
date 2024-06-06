
namespace Core.UI {

    using ImGuiNET;

    public class Text : UIElement {

        public string Content { get; set; }
        public System.Numerics.Vector4 Color { get; set; }

        public Text(System.Numerics.Vector2 position, string content, System.Numerics.Vector4 color, System.Numerics.Vector2 size)
            : base(position, size) {

            Content = content;
            Color = color;
            Size = size;
        }

        public Text(System.Numerics.Vector2 position, string content)
            : this(position, content, new System.Numerics.Vector4(1, 1, 1, 1), new System.Numerics.Vector2(100, 100)) { }

        public Text(string content)
            : this(new System.Numerics.Vector2(0, 0), content) { }

        public override void Render() {

            if (!IsActive)
                return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin("##Text" + Content, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            ImGui.TextColored(Color, Content);

            ImGui.End();
        }
    }
}