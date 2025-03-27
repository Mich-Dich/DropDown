namespace Core.UI {
    using Core.render;
    using ImGuiNET;
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

        public Button(System.Numerics.Vector2 position, System.Numerics.Vector2 size, string label, Action onClick, Action onHover, System.Numerics.Vector4 color, System.Numerics.Vector4 hoverColor, System.Numerics.Vector4 clickColor, System.Numerics.Vector4 textColor, System.Numerics.Vector4 hoverTextColor, System.Numerics.Vector4 clickTextColor)
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
            Size = size;
        }

        public void SetOnClick(Action onClick) {
            OnClick = onClick;
        }

        public void SetOnHover(Action onHover) {
            OnHover = onHover;
        }

        public override void Render() {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin(Label, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            System.Numerics.Vector4 currentColor = ImGui.IsItemActive() ? ClickColor : (ImGui.IsItemHovered() ? HoverColor : Color);
            System.Numerics.Vector4 currentTextColor = ImGui.IsItemActive() ? ClickTextColor : (ImGui.IsItemHovered() ? HoverTextColor : TextColor);

            ImGui.PushStyleColor(ImGuiCol.Button, currentColor);
            ImGui.PushStyleColor(ImGuiCol.Text, currentTextColor);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, BorderRadius);

            if (ImGui.Button(Label, Size)) {
                OnClick?.Invoke();
            } else if (ImGui.IsItemHovered()) {
                OnHover?.Invoke();
            }

            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar();

            ImGui.End();
        }
    }

    //public class image_button : UIElement {
    //
    //    public string Label { get; set; }
    //    public Action OnClick { get; set; }
    //    public Action OnHover { get; set; }
    //    public Texture DefaultTexture { get; set; }
    //    public Texture HoverTexture { get; set; }
    //    public Texture ActiveTexture { get; set; }
    //    public System.Numerics.Vector4 DefaultTint { get; set; } = System.Numerics.Vector4.One;
    //    public System.Numerics.Vector4 HoverTint { get; set; } = System.Numerics.Vector4.One;
    //    public System.Numerics.Vector4 ActiveTint { get; set; } = System.Numerics.Vector4.One;
    //    public System.Numerics.Vector4 TextColor { get; set; } = System.Numerics.Vector4.One;
    //    public System.Numerics.Vector4 TextHoverColor { get; set; } = System.Numerics.Vector4.One;
    //    public System.Numerics.Vector4 TextActiveColor { get; set; } = System.Numerics.Vector4.One;
    //    public float FrameRounding { get; set; }
    //    public System.Numerics.Vector2 UV0 { get; set; } = System.Numerics.Vector2.Zero;
    //    public System.Numerics.Vector2 UV1 { get; set; } = System.Numerics.Vector2.One;

    //    // Current state tracking
    //    private bool wasHovered;
    //    private bool wasActive;

    //    public image_button(System.Numerics.Vector2 position, System.Numerics.Vector2 size, Texture defaultTexture,
    //                     string label = "", Action onClick = null, Action onHover = null)
    //        : base(position, size) {

    //        DefaultTexture = defaultTexture;
    //        Label = label;
    //        OnClick = onClick;
    //        OnHover = onHover;
    //    }

    //    public override void Render() {

    //        if (!IsActive || DefaultTexture.Handle == IntPtr.Zero) return;

    //        ImGui.PushID(Label);
    //        ImGui.SetNextWindowPos(Position);
    //        ImGui.SetNextWindowSize(Size);
    //        ImGui.Begin("##ImageButtonWindow",
    //            ImGuiWindowFlags.NoBackground |
    //            ImGuiWindowFlags.NoDecoration |
    //            ImGuiWindowFlags.NoInputs);

    //        // Determine current state
    //        var currentTexture = DefaultTexture;
    //        var currentTint = DefaultTint;
    //        var currentTextColor = TextColor;

    //        if (wasActive) {

    //            currentTexture = ActiveTexture ?? DefaultTexture;
    //            currentTint = ActiveTint;
    //            currentTextColor = TextActiveColor;
            
    //        } else if (wasHovered) {

    //            currentTexture = HoverTexture ?? DefaultTexture;
    //            currentTint = HoverTint;
    //            currentTextColor = TextHoverColor;
    //        }

    //        // Render image button
    //        ImGui.SetCursorPos(System.Numerics.Vector2.Zero);
    //        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, FrameRounding);
    //        var pressed = ImGui.ImageButton(
    //            currentTexture.Handle,
    //            Size,
    //            UV0,
    //            UV1,
    //            0,
    //            System.Numerics.Vector4.Zero,
    //            currentTint
    //        );

    //        // Update state for next frame
    //        wasHovered = ImGui.IsItemHovered();
    //        wasActive = ImGui.IsItemActive();

    //        // Handle interactions
    //        if (pressed)
    //            OnClick?.Invoke();
            
    //        else if (wasHovered)
    //            OnHover?.Invoke();

    //        // Render text label
    //        if (!string.IsNullOrEmpty(Label)) {

    //            var textSize = ImGui.CalcTextSize(Label);
    //            var textPos = (Size - textSize) * 0.5f;
    //            ImGui.SetCursorPos(textPos);
    //            ImGui.TextColored(currentTextColor, Label);
    //        }

    //        ImGui.PopStyleVar();
    //        ImGui.End();
    //        ImGui.PopID();
    //    }
    //}
}