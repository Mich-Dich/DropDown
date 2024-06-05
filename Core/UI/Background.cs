using ImGuiNET;
using System.Numerics;
using Core.util;

namespace Core.UI {
    public class Background : UIElement {
        private Vector4 color;
        private readonly IntPtr textureId;

        // Constructor for color background
        public Background(Vector4 color) : base(Vector2.Zero, new System.Numerics.Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y)) {
            this.color = color;
            this.textureId = IntPtr.Zero;
        }

        // Constructor for image background
        public Background(string texturePath) : base(Vector2.Zero,  new System.Numerics.Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y)) {
            this.textureId = Resource_Manager.Get_Texture(texturePath).Handle;
            this.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        public override void Render() {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin("Background", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            if (textureId != IntPtr.Zero) {
                // If textureId is set, render image background
                ImGui.Image(textureId, Size, new Vector2(0, 0), new Vector2(1, 1), color);
            } else {
                // Otherwise, render color background
                ImGui.GetBackgroundDrawList().AddRectFilled(Position, Position + Size, ImGui.GetColorU32(color));
            }

            ImGui.End();
        }
    }
}