using Core.util;
using ImGuiNET;
using System.Numerics;

namespace Core.UI
{
    public class Background : UIElement
    {
        private Vector4 color;
        private readonly IntPtr textureId;
        public bool UseGradient { get; set; } = false;
        public Vector4 GradientColor { get; set; } = new Vector4(0.1f, 0.1f, 0.2f, 1.0f);
        public bool UsePattern { get; set; } = false;
        public float PatternScale { get; set; } = 50.0f;
        public bool UseBlur { get; set; } = false;
        public float BlurStrength { get; set; } = 0.5f;

        // Constructor for color background
        public Background(Vector4 color) : base(Vector2.Zero, new System.Numerics.Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y))
        {
            this.color = color;
            this.textureId = IntPtr.Zero;
        }

        // Constructor for image background
        public Background(string texturePath) : base(Vector2.Zero, new System.Numerics.Vector2(Game.Instance.window.Size.X, Game.Instance.window.Size.Y))
        {
            this.textureId = Resource_Manager.Get_Texture(texturePath).Handle;
            this.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        // Constructor for gradient background
        public Background(Vector4 color, Vector4 gradientColor) : this(color)
        {
            this.color = color;
            this.GradientColor = gradientColor;
            this.UseGradient = true;
        }

        public override void Render()
        {
            if (!IsActive)
                return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin($"Background_{GetHashCode()}", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNav);

            var draw_list = ImGui.GetWindowDrawList();
            var win_pos = ImGui.GetWindowPos();

            if (textureId != IntPtr.Zero)
            {
                // Draw texture background
                ImGui.Image(textureId, Size, new Vector2(0, 1), new Vector2(1, 0), color);
            }
            else if (UseGradient)
            {
                // Draw gradient background
                draw_list.AddRectFilledMultiColor(
                    win_pos, win_pos + Size,
                    ImGui.GetColorU32(color),
                    ImGui.GetColorU32(color),
                    ImGui.GetColorU32(GradientColor),
                    ImGui.GetColorU32(GradientColor)
                );

                // Add subtle pattern overlay
                if (UsePattern)
                {
                    DrawPattern(draw_list, win_pos);
                }
            }
            else
            {
                // Draw solid color background
                draw_list.AddRectFilled(win_pos, win_pos + Size, ImGui.GetColorU32(color));

                // Add subtle pattern overlay
                if (UsePattern)
                {
                    DrawPattern(draw_list, win_pos);
                }
            }

            ImGui.End();
        }

        private void DrawPattern(ImDrawListPtr draw_list, Vector2 pos)
        {
            var patternColor = new Vector4(1, 1, 1, 0.05f);
            var patternSize = PatternScale;

            for (float x = 0; x < Size.X; x += patternSize)
            {
                for (float y = 0; y < Size.Y; y += patternSize)
                {
                    var patternPos = pos + new Vector2(x, y);
                    draw_list.AddCircleFilled(patternPos, 1.0f, ImGui.GetColorU32(patternColor));
                }
            }
        }
    }
}