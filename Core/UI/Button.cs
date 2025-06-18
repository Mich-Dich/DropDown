using ImGuiNET;
using System;

namespace Core.UI {

    public class Button : UIElement {
        public string Label { get; set; }
        public Action OnClick { get; set; }
        public Action OnHover { get; set; }
        public System.Numerics.Vector4 Color { get; set; }
        public System.Numerics.Vector4 HoverColor { get; set; }
        public System.Numerics.Vector4 ClickColor { get; set; }
        public System.Numerics.Vector4 TextColor { get; set; }
        public System.Numerics.Vector4 HoverTextColor { get; set; }
        public System.Numerics.Vector4 ClickTextColor { get; set; }
        public float BorderRadius { get; set; } = 8.0f;
        public bool TransparentHoverEffect { get; set; }
        public bool UseGradient { get; set; } = true;
        public bool UseShadow { get; set; } = true;
        public float ShadowOffset { get; set; } = 2.0f;
        public float AnimationSpeed { get; set; } = 0.15f;
        public string IconPath { get; set; }
        public bool IsPrimary { get; set; } = false;
        public bool IsDanger { get; set; } = false;

        private float hoverAnimation = 0.0f;
        private bool wasHovered = false;

        public Button(System.Numerics.Vector2 position, System.Numerics.Vector2 size, string label, Action onClick, Action onHover, System.Numerics.Vector4 color, System.Numerics.Vector4 hoverColor, System.Numerics.Vector4 clickColor, System.Numerics.Vector4 textColor, System.Numerics.Vector4 hoverTextColor, System.Numerics.Vector4 clickTextColor, bool transparentHoverEffect = false)
            : base(position, size) {

            Label = label;
            OnClick = onClick;
            OnHover = onHover;
            Color = color;
            HoverColor = hoverColor;
            ClickColor = clickColor;
            TextColor = textColor;
            HoverTextColor = hoverTextColor;
            ClickTextColor = clickTextColor;
            TransparentHoverEffect = transparentHoverEffect;
            Size = size;
        }

        public void SetOnClick(Action onClick) { OnClick = onClick; }

        public void SetOnHover(Action onHover) { OnHover = onHover; }

        public override void Render() {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin($"Button_{Label}_{GetHashCode()}", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            var draw_list = ImGui.GetWindowDrawList();
            var win_pos = ImGui.GetWindowPos();
            var button_pos = win_pos;
            var button_size = Size;

            // Check if mouse is over the button area
            var mouse_pos = ImGui.GetMousePos();
            bool isHovered = mouse_pos.X >= button_pos.X && mouse_pos.X <= button_pos.X + button_size.X &&
                           mouse_pos.Y >= button_pos.Y && mouse_pos.Y <= button_pos.Y + button_size.Y;

            // Update hover animation
            if (isHovered && !wasHovered)
            {
                OnHover?.Invoke();
            }
            wasHovered = isHovered;

            // Smooth hover animation
            float targetHover = isHovered ? 1.0f : 0.0f;
            hoverAnimation = Math.Min(1.0f, hoverAnimation + (targetHover - hoverAnimation) * AnimationSpeed);

            // Calculate colors with animation
            var currentColor = Color;
            var currentTextColor = TextColor;

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && isHovered)
            {
                currentColor = ClickColor;
                currentTextColor = ClickTextColor;
            }
            else if (isHovered)
            {
                currentColor = HoverColor;
                currentTextColor = HoverTextColor;
            }

            // Apply hover animation interpolation
            if (hoverAnimation > 0)
            {
                currentColor = new System.Numerics.Vector4(
                    Color.X + (HoverColor.X - Color.X) * hoverAnimation,
                    Color.Y + (HoverColor.Y - Color.Y) * hoverAnimation,
                    Color.Z + (HoverColor.Z - Color.Z) * hoverAnimation,
                    Color.W + (HoverColor.W - Color.W) * hoverAnimation
                );
                currentTextColor = new System.Numerics.Vector4(
                    TextColor.X + (HoverTextColor.X - TextColor.X) * hoverAnimation,
                    TextColor.Y + (HoverTextColor.Y - TextColor.Y) * hoverAnimation,
                    TextColor.Z + (HoverTextColor.Z - TextColor.Z) * hoverAnimation,
                    TextColor.W + (HoverTextColor.W - TextColor.W) * hoverAnimation
                );
            }

            // Draw shadow
            if (UseShadow && !isHovered)
            {
                var shadowColor = new System.Numerics.Vector4(0, 0, 0, 0.3f);
                var shadowPos = button_pos + new System.Numerics.Vector2(ShadowOffset, ShadowOffset);
                draw_list.AddRectFilled(shadowPos, shadowPos + button_size, ImGui.GetColorU32(shadowColor), BorderRadius);
            }

            // Draw button background with gradient
            if (UseGradient)
            {
                var gradientTop = new System.Numerics.Vector4(
                    currentColor.X * 1.2f, currentColor.Y * 1.2f, currentColor.Z * 1.2f, currentColor.W
                );
                var gradientBottom = new System.Numerics.Vector4(
                    currentColor.X * 0.8f, currentColor.Y * 0.8f, currentColor.Z * 0.8f, currentColor.W
                );

                // Draw gradient background
                draw_list.AddRectFilledMultiColor(
                    button_pos, button_pos + button_size,
                    ImGui.GetColorU32(gradientTop),
                    ImGui.GetColorU32(gradientTop),
                    ImGui.GetColorU32(gradientBottom),
                    ImGui.GetColorU32(gradientBottom)
                );
            }
            else
            {
                draw_list.AddRectFilled(button_pos, button_pos + button_size, ImGui.GetColorU32(currentColor), BorderRadius);
            }

            // Draw border
            var borderColor = new System.Numerics.Vector4(
                currentColor.X * 0.7f, currentColor.Y * 0.7f, currentColor.Z * 0.7f, currentColor.W
            );
            draw_list.AddRect(button_pos, button_pos + button_size, ImGui.GetColorU32(borderColor), BorderRadius, 0, 2.0f);

            // Draw highlight effect when hovered
            if (isHovered)
            {
                var highlightColor = new System.Numerics.Vector4(1, 1, 1, 0.1f);
                draw_list.AddRectFilled(button_pos, button_pos + button_size, ImGui.GetColorU32(highlightColor), BorderRadius);
            }

            // Calculate text position
            var textSize = ImGui.CalcTextSize(Label);
            var textPos = button_pos + (button_size - textSize) * 0.5f;

            // Draw text with shadow for better readability
            var textShadowColor = new System.Numerics.Vector4(0, 0, 0, 0.5f);
            draw_list.AddText(textPos + new System.Numerics.Vector2(1, 1), ImGui.GetColorU32(textShadowColor), Label);
            draw_list.AddText(textPos, ImGui.GetColorU32(currentTextColor), Label);

            // Handle click
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && isHovered)
            {
                OnClick?.Invoke();
            }

            ImGui.End();
        }
    }
}