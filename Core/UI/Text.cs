using System;
using ImGuiNET;
using System.Numerics;
using Core.util;

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
        private Func<string> contentProvider;
        private string content;

        public string Content 
        { 
            get 
            { 
                if (contentProvider != null)
                {
                    content = contentProvider.Invoke();
                }
                return content; 
            } 
            set 
            { 
                content = value; 
                UpdateSize();
            } 
        }

        public Vector4 Color { get; set; }
        public float FontSize { get; set; }
        public TextAlign Alignment { get; set; }
        public bool UseShadow { get; set; } = true;
        public Vector4 ShadowColor { get; set; } = new Vector4(0, 0, 0, 0.5f);
        public Vector2 ShadowOffset { get; set; } = new Vector2(1, 1);
        public bool UseGradient { get; set; } = false;
        public Vector4 GradientColor { get; set; } = new Vector4(1, 1, 1, 1);
        public bool IsBold { get; set; } = false;
        public bool IsItalic { get; set; } = false;
        public float LetterSpacing { get; set; } = 0.0f;

        public Text(Vector2 position, Func<string> contentProvider, Vector4 color, float fontSize, Vector2 size)
            : base(position, size)
        {
            this.contentProvider = contentProvider;
            Color = color;
            FontSize = fontSize;
            Alignment = TextAlign.Center;
        }

        public Text(Vector2 position, string content, Vector4 color, float fontSize, TextAlign alignment = TextAlign.Center)
            : base(position, Vector2.Zero)
        {
            this.content = content;
            Color = color;
            FontSize = fontSize;
            Alignment = alignment;

            UpdateSize();
        }

        public Text(Vector2 position, Func<string> contentProvider, Vector4 color, float fontSize, TextAlign alignment = TextAlign.Center)
            : base(position, Vector2.Zero)
        {
            this.contentProvider = contentProvider;
            Color = color;
            FontSize = fontSize;
            Alignment = alignment;

            UpdateSize();
        }

        public Text(string content, float fontSize)
            : this(new Vector2(0, 0), content, new Vector4(1, 1, 1, 1), fontSize, TextAlign.Center) { }

        public Text(string content, Vector4 color, float fontSize)
            : this(new Vector2(0, 0), content, color, fontSize, TextAlign.Center) { }

        public Text(string content, Vector2 position, float fontSize)
            : this(position, content, new Vector4(1, 1, 1, 1), fontSize, TextAlign.Center) { }

        public Text(string content, Vector2 position, Vector4 color, float fontSize)
            : this(position, content, color, fontSize, TextAlign.Center) { }

        private void UpdateSize()
        {
            var textSize = ImGui.CalcTextSize(content);
            Size = new Vector2(textSize.X * FontSize, textSize.Y * FontSize + FontSize * 0.4f);
            Position = new Vector2(Position.X, Position.Y + Size.Y / 2);
        }

        public override void Render()
        {
            if (!IsActive)
                return;

            Vector2 windowPos;
            switch (Alignment)
            {
                case TextAlign.Left:
                    windowPos = new Vector2(Position.X, Position.Y - Size.Y / 2);
                    break;
                case TextAlign.Right:
                    windowPos = new Vector2(Position.X - Size.X, Position.Y - Size.Y / 2);
                    break;
                default: // Center
                    windowPos = new Vector2(Position.X - Size.X / 2, Position.Y - Size.Y / 2);
                    break;
            }
            ImGui.SetNextWindowPos(windowPos);

            var padding = new Vector2(10, 10);
            ImGui.SetNextWindowSize(Size + padding);

            ImGui.Begin($"##Text_{Content}_{GetHashCode()}", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            var draw_list = ImGui.GetWindowDrawList();
            var win_pos = ImGui.GetWindowPos();

            // Calculate text position
            var textSize = ImGui.CalcTextSize(Content);
            var textPos = win_pos + (Size - textSize) * 0.5f;

            // Select appropriate font based on size
            string fontKey = "default";
            if (FontSize >= 3.0f)
            {
                fontKey = "giant";
            }
            else if (FontSize >= 2.0f)
            {
                fontKey = IsBold ? "bold_big" : "regular_big";
            }
            else if (FontSize <= 0.8f)
            {
                fontKey = IsBold ? "small_bold" : "small";
            }
            else
            {
                fontKey = IsBold ? "bold" : "default";
            }

            // Apply font styling if fonts are available
            if (Imgui_Fonts.fonts.ContainsKey(fontKey))
            {
                ImGui.PushFont(Imgui_Fonts.fonts[fontKey]);
            }
            else if (IsBold && Imgui_Fonts.fonts.ContainsKey("bold"))
            {
                ImGui.PushFont(Imgui_Fonts.fonts["bold"]);
            }
            else if (IsItalic && Imgui_Fonts.fonts.ContainsKey("italic"))
            {
                ImGui.PushFont(Imgui_Fonts.fonts["italic"]);
            }

            // Draw shadow if enabled
            if (UseShadow)
            {
                var shadowPos = textPos + ShadowOffset;
                draw_list.AddText(shadowPos, ImGui.GetColorU32(ShadowColor), Content);
            }

            // Draw main text
            if (UseGradient)
            {
                // Draw gradient text by drawing each character with interpolated color
                var currentPos = textPos;
                for (int i = 0; i < Content.Length; i++)
                {
                    var charColor = new Vector4(
                        Color.X + (GradientColor.X - Color.X) * (i / (float)Content.Length),
                        Color.Y + (GradientColor.Y - Color.Y) * (i / (float)Content.Length),
                        Color.Z + (GradientColor.Z - Color.Z) * (i / (float)Content.Length),
                        Color.W + (GradientColor.W - Color.W) * (i / (float)Content.Length)
                    );
                    
                    var charStr = Content[i].ToString();
                    var charSize = ImGui.CalcTextSize(charStr);
                    draw_list.AddText(currentPos, ImGui.GetColorU32(charColor), charStr);
                    currentPos.X += charSize.X + LetterSpacing;
                }
            }
            else
            {
                draw_list.AddText(textPos, ImGui.GetColorU32(Color), Content);
            }

            // Pop font if we pushed one
            if (Imgui_Fonts.fonts.ContainsKey(fontKey) || 
                (IsBold && Imgui_Fonts.fonts.ContainsKey("bold")) || 
                (IsItalic && Imgui_Fonts.fonts.ContainsKey("italic")))
            {
                ImGui.PopFont();
            }

            ImGui.End();
        }
    }
}