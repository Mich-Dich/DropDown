using ImGuiNET;
using System.Numerics;

namespace Core.UI
{
    public class Card : UIElement
    {
        public Vector4 BackgroundColor { get; set; } = new Vector4(0.1f, 0.1f, 0.15f, 0.9f);
        public Vector4 BorderColor { get; set; } = new Vector4(0.3f, 0.3f, 0.4f, 1.0f);
        public float BorderRadius { get; set; } = 10.0f;
        public float BorderThickness { get; set; } = 2.0f;
        public bool UseShadow { get; set; } = true;
        public float ShadowOffset { get; set; } = 3.0f;
        public bool UseGradient { get; set; } = true;
        public Vector4 GradientColor { get; set; } = new Vector4(0.15f, 0.15f, 0.2f, 0.9f);
        public string Title { get; set; } = "";
        public Vector4 TitleColor { get; set; } = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public float TitleFontSize { get; set; } = 1.5f;
        public bool ShowTitle { get; set; } = true;

        private readonly List<UIElement> elements = new();

        public Card(Vector2 position, Vector2 size) : base(position, size)
        {
        }

        public Card(Vector2 position, Vector2 size, string title) : this(position, size)
        {
            Title = title;
        }

        public new void AddElement(UIElement element)
        {
            elements.Add(element);
        }

        public void RemoveElement(UIElement element)
        {
            elements.Remove(element);
        }

        public override void Render()
        {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin($"Card_{Title}_{GetHashCode()}", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            var draw_list = ImGui.GetWindowDrawList();
            var win_pos = ImGui.GetWindowPos();
            var card_pos = win_pos;
            var card_size = Size;

            // Draw shadow
            if (UseShadow)
            {
                var shadowColor = new Vector4(0, 0, 0, 0.3f);
                var shadowPos = card_pos + new Vector2(ShadowOffset, ShadowOffset);
                draw_list.AddRectFilled(shadowPos, shadowPos + card_size, ImGui.GetColorU32(shadowColor), BorderRadius);
            }

            // Draw background
            if (UseGradient)
            {
                draw_list.AddRectFilledMultiColor(
                    card_pos, card_pos + card_size,
                    ImGui.GetColorU32(BackgroundColor),
                    ImGui.GetColorU32(BackgroundColor),
                    ImGui.GetColorU32(GradientColor),
                    ImGui.GetColorU32(GradientColor)
                );
            }
            else
            {
                draw_list.AddRectFilled(card_pos, card_pos + card_size, ImGui.GetColorU32(BackgroundColor), BorderRadius);
            }

            // Draw border
            if (BorderThickness > 0)
            {
                draw_list.AddRect(card_pos, card_pos + card_size, ImGui.GetColorU32(BorderColor), BorderRadius, 0, BorderThickness);
            }

            // Draw title if specified
            if (ShowTitle && !string.IsNullOrEmpty(Title))
            {
                var titleSize = ImGui.CalcTextSize(Title);
                // Center the title horizontally
                var titlePos = new Vector2(
                    card_pos.X + (card_size.X - titleSize.X) / 2,
                    card_pos.Y + 15
                );
                // Draw title background
                var titleBgColor = new Vector4(0, 0, 0, 0.3f);
                var titleBgSize = new Vector2(titleSize.X + 20, titleSize.Y + 10);
                draw_list.AddRectFilled(titlePos - new Vector2(5, 5), titlePos + titleBgSize, ImGui.GetColorU32(titleBgColor), 5.0f);
                // Draw title text
                draw_list.AddText(titlePos, ImGui.GetColorU32(TitleColor), Title);
            }

            // Render child elements
            foreach (var element in elements)
            {
                element.Render();
            }

            ImGui.End();
        }
    }
} 