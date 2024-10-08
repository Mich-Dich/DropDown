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
        public float BorderRadius { get; set; }
        public bool TransparentHoverEffect { get; set; }

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
            ImGui.Begin(Label, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            System.Numerics.Vector4 effectiveHoverColor = TransparentHoverEffect ? Color : HoverColor;

            System.Numerics.Vector4 currentColor;
            if (ImGui.IsItemActive()) {
                currentColor = ClickColor;
            } else if (ImGui.IsItemHovered()) {
                currentColor = TransparentHoverEffect ? effectiveHoverColor : HoverColor;
            } else {
                currentColor = Color;
            }

            System.Numerics.Vector4 currentTextColor = ImGui.IsItemActive() ? ClickTextColor : (ImGui.IsItemHovered() ? HoverTextColor : TextColor);

            ImGui.PushStyleColor(ImGuiCol.Button, currentColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, currentColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, ClickColor);
            ImGui.PushStyleColor(ImGuiCol.Text, currentTextColor);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, BorderRadius);

            if (ImGui.Button(Label, Size))
                OnClick?.Invoke();
            else if (ImGui.IsItemHovered())
                OnHover?.Invoke();

            ImGui.PopStyleColor(4);
            ImGui.PopStyleVar();

            ImGui.End();
        }
    }
}