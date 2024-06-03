namespace Core.UI {
    using ImGuiNET;
    using System.Numerics;
    using Core.util;

    public class Image : UIElement {
        public IntPtr TextureId { get; set; }
        public Vector2 UV0 { get; set; } = new Vector2(0, 1);
        public Vector2 UV1 { get; set; } = new Vector2(1, 0);
        public Vector4 TintColor { get; set; }

        public Image(Vector2 position, string texturePath)
            : this(position, new Vector2(100, 100), Resource_Manager.Get_Texture(texturePath).Handle, new Vector2(0, 0), new Vector2(1, 1), new Vector4(1.0f, 1.0f, 1.0f, 1.0f)) { }

        public Image(Vector2 position, Vector2 size, string texturePath)
            : this(position, size, Resource_Manager.Get_Texture(texturePath).Handle, new Vector2(0, 1), new Vector2(1, 0), new Vector4(1.0f, 1.0f, 1.0f, 1.0f)) { }

        public Image(Vector2 position, Vector2 size, IntPtr textureId, Vector2 uv0, Vector2 uv1, Vector4 tintColor)
            : base(position, size) {
            TextureId = textureId;
            UV0 = uv0;
            UV1 = uv1;
            TintColor = tintColor;
        }

        public override void Render() {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin($"Image_{TextureId}", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            ImGui.Image(TextureId, Size, UV0, UV1, TintColor);

            ImGui.End();
        }
    }
}