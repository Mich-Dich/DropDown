namespace Core.UI {
    using ImGuiNET;
    using System;

    public class Input : UIElement {
        public string Text { get; set; }
        public System.Numerics.Vector4 Color { get; set; }
        public Action<string> OnTextChanged { get; set; }

        public Input(System.Numerics.Vector2 position, System.Numerics.Vector2 size, string text, System.Numerics.Vector4 color, Action<string> onTextChanged)
            : base(position, size) {
            Text = text;
            Color = color;
            OnTextChanged = onTextChanged;
        }

        public Input(System.Numerics.Vector2 position, string text)
            : this(position, new System.Numerics.Vector2(100, 20), text, new System.Numerics.Vector4(1, 1, 1, 1), null) { }

        public Input(string text)
            : this(new System.Numerics.Vector2(0, 0), text) { }

        public override void Render() {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin("##Input" + Text, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            ImGui.PushStyleColor(ImGuiCol.Text, Color);

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(Text + '\0');
            if (ImGui.InputText("##InputField", inputBytes, 100, ImGuiInputTextFlags.AutoSelectAll)) {
                string newText = System.Text.Encoding.UTF8.GetString(inputBytes).TrimEnd('\0');
                if (newText != Text) {
                    Text = newText;
                    OnTextChanged?.Invoke(Text);
                }
            }

            ImGui.PopStyleColor();

            ImGui.End();
        }
    }
}