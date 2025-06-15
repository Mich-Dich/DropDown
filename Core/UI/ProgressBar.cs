using Core.util;
using ImGuiNET;
using System.Numerics;

namespace Core.UI
{
    public class ProgressBar : UIElement
    {
        public System.Numerics.Vector4 FillColor { get; set; }
        public System.Numerics.Vector4 BackgroundColor { get; set; }
        public Func<float> ValueProvider { get; set; }
        public float MinValue { get; set; }
        public Func<float> MaxValue { get; set; }
        public bool ShowPercentageText { get; set; }
        public Vector4 BorderColor { get; set; } = new Vector4(0,0,0,0.5f);
        public float BorderThickness { get; set; } = 2.0f;
        public string Label { get; set; } = string.Empty;
        public bool UseGradient { get; set; } = true;
        public Vector4 GradientColor { get; set; } = new Vector4(0.4f, 1.0f, 0.4f, 1.0f);
        private static int counter = 0;
        private readonly string windowName;


        public ProgressBar(Vector2 position, Vector2 size, Vector4 fillColor, Vector4 backgroundColor, Func<float> valueProvider, float minValue, float maxValue, bool showPercentageText = false, float backgroundAlpha = 1.0f)
            : base(position, size)
        {
            FillColor = fillColor;
            BackgroundColor = backgroundColor;
            ValueProvider = valueProvider;
            MinValue = minValue;
            MaxValue = () => maxValue;
            ShowPercentageText = showPercentageText;

            windowName = $"ProgressBar{counter++}";
        }

        public ProgressBar(Vector2 position, Vector2 size, Vector4 fillColor, Vector4 backgroundColor, Func<float> valueProvider, float minValue, Func<float> maxValue, bool showPercentageText = false, float backgroundAlpha = 1.0f)
            : base(position, size)
        {
            FillColor = fillColor;
            BackgroundColor = new Vector4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundAlpha);
            ValueProvider = valueProvider;
            MinValue = minValue;
            MaxValue = maxValue;
            ShowPercentageText = showPercentageText;

            windowName = $"ProgressBar{counter++}";
        }

        public override void Render()
        {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin(windowName, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            float value = ValueProvider();
            float clampedValue = Math.Clamp(value, MinValue, MaxValue());
            float fraction = (clampedValue - MinValue) / (MaxValue() - MinValue);

            var draw_list = ImGui.GetWindowDrawList();
            var win_pos = ImGui.GetWindowPos();
            var bar_pos = win_pos;
            var bar_size = Size;
            float radius = bar_size.Y / 2.0f;

            // Draw background with rounded corners
            draw_list.AddRectFilled(bar_pos, bar_pos + bar_size, ImGui.GetColorU32(BackgroundColor), radius);
            // Draw border
            if (BorderThickness > 0)
                draw_list.AddRect(bar_pos, bar_pos + bar_size, ImGui.GetColorU32(BorderColor), radius, 0, BorderThickness);

            // Draw fill (with optional gradient)
            var fill_size = new Vector2(bar_size.X * fraction, bar_size.Y);
            if (fraction > 0)
            {
                if (UseGradient)
                {
                    // Blend FillColor and GradientColor based on fraction for a simple horizontal gradient effect
                    var blendedColor = new Vector4(
                        FillColor.X * (1 - fraction) + GradientColor.X * fraction,
                        FillColor.Y * (1 - fraction) + GradientColor.Y * fraction,
                        FillColor.Z * (1 - fraction) + GradientColor.Z * fraction,
                        FillColor.W * (1 - fraction) + GradientColor.W * fraction
                    );
                    draw_list.AddRectFilled(bar_pos, bar_pos + fill_size, ImGui.GetColorU32(blendedColor), radius);
                }
                else
                {
                    draw_list.AddRectFilled(bar_pos, bar_pos + fill_size, ImGui.GetColorU32(FillColor), radius);
                }
            }

            // Draw label (centered)
            if (!string.IsNullOrEmpty(Label))
            {
                var text_size = ImGui.CalcTextSize(Label);
                var text_pos = bar_pos + new Vector2((bar_size.X - text_size.X) / 2, (bar_size.Y - text_size.Y) / 2);
                draw_list.AddText(text_pos, ImGui.GetColorU32(new Vector4(1,1,1,1)), Label);
            }

            ImGui.End();
        }
    }
}