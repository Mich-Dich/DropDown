using ImGuiNET;

namespace Core.UI
{
    public enum TextAlign
    {
        Left,
        Center,
        Right
    }

    public class Text : UIElement
    {
        private string content;
        public string Content 
        { 
            get { return content; } 
            set 
            { 
                content = value; 
                UpdateSize();
            } 
        }
        public System.Numerics.Vector4 Color { get; set; }
        public float FontSize { get; set; }
        public TextAlign Alignment { get; set; }

        public Text(System.Numerics.Vector2 position, string content, System.Numerics.Vector4 color, float fontSize, TextAlign alignment = TextAlign.Center)
            : base(position, System.Numerics.Vector2.Zero)
        {
            this.content = content;
            Color = color;
            FontSize = fontSize;
            Alignment = alignment;

            UpdateSize();
        }

        public Text(string content, float fontSize)
            : this(new System.Numerics.Vector2(0, 0), content, new System.Numerics.Vector4(1, 1, 1, 1), fontSize, TextAlign.Center) { }

        public Text(string content, System.Numerics.Vector4 color, float fontSize)
            : this(new System.Numerics.Vector2(0, 0), content, color, fontSize, TextAlign.Center) { }

        public Text(string content, System.Numerics.Vector2 position, float fontSize)
            : this(position, content, new System.Numerics.Vector4(1, 1, 1, 1), fontSize, TextAlign.Center) { }

        public Text(string content, System.Numerics.Vector2 position, System.Numerics.Vector4 color, float fontSize)
            : this(position, content, color, fontSize, TextAlign.Center) { }

        private void UpdateSize()
        {
            var textSize = ImGui.CalcTextSize(content);
            Size = new System.Numerics.Vector2(textSize.X * FontSize, textSize.Y * FontSize + FontSize * 0.4f);
        }

        public override void Render()
        {
            if (!IsActive)
                return;

            System.Numerics.Vector2 windowPos;
            switch (Alignment)
            {
                case TextAlign.Left:
                    windowPos = new System.Numerics.Vector2(Position.X, Position.Y - Size.Y / 2);
                    break;
                case TextAlign.Right:
                    windowPos = new System.Numerics.Vector2(Position.X - Size.X, Position.Y - Size.Y / 2);
                    break;
                default: // Center
                    windowPos = new System.Numerics.Vector2(Position.X - Size.X / 2, Position.Y - Size.Y / 2);
                    break;
            }
            ImGui.SetNextWindowPos(windowPos);

            var padding = new System.Numerics.Vector2(10, 10);
            ImGui.SetNextWindowSize(Size + padding);

            ImGui.Begin("##Text" + Content, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            ImGui.SetWindowFontScale(FontSize);
            ImGui.TextColored(Color, Content);
            ImGui.SetWindowFontScale(1.0f);

            ImGui.End();
        }
    }
}