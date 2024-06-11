
namespace Core.UI {

    using Core.render;
    using Core.util;
    using ImGuiNET;

    public class ImageButton : UIElement {

        public string Label { get; set; }
        public Action OnClick { get; set; }
        public Action OnHover { get; set; }
        public float BorderRadius { get; set; }

        private nint selected_image;

        private Texture bu_normal;
        private Texture bu_hovered;
        private Texture bu_clicked;

        public ImageButton(System.Numerics.Vector2 position, System.Numerics.Vector2 size, string label, Action? onClick, Action? onHover, string bu_normal_img_path, string? bu_hovered_img_path, string? bu_clicked_img_path)
            : base(position, size) {

            Label = label;
            Size = size;

            if(onClick != null)
                OnClick = onClick;
            if(onHover != null)
                OnHover = onHover;

            bu_normal = Resource_Manager.Get_Texture(bu_normal_img_path);
            bu_hovered = (bu_hovered_img_path != null) ? Resource_Manager.Get_Texture(bu_hovered_img_path) : bu_normal;
            bu_clicked = (bu_clicked_img_path != null) ? Resource_Manager.Get_Texture(bu_clicked_img_path) : bu_normal;
            selected_image = bu_normal.Handle;
        }

        public void force_OnClick(Action onClick) {

            OnClick = onClick;
            selected_image = bu_clicked.Handle;
        }

        public void force_OnHover(Action onHover) {

            OnHover = onHover;
            selected_image = bu_hovered.Handle;
        }

        public override void Render() {
            if(!IsActive) return;

            ImGui.SetCursorPos(Position);

            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));

            if(ImGui.ImageButton(Label, selected_image, Size))
                OnClick?.Invoke();

            ImGui.PopStyleColor(3);

            if(ImGui.IsItemHovered())
                OnHover?.Invoke();

            selected_image = ImGui.IsItemHovered() ?
                ImGui.IsItemClicked() ? bu_clicked.Handle : bu_hovered.Handle
                : bu_normal.Handle;
        }
    }
}