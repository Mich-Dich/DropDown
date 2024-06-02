using ImGuiNET;
using System;
using System.Numerics;

namespace Core.UI {
    public class ProgressBar : UIElement {
        public Vector4 FillColor { get; set; }
        public Vector4 BackgroundColor { get; set; }
        public Func<float> ValueProvider { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public bool ShowPercentageText { get; set; }

        public ProgressBar(Vector2 position, Vector2 size, Vector4 fillColor, Vector4 backgroundColor, Func<float> valueProvider, float minValue, float maxValue, bool showPercentageText = false, float backgroundAlpha = 1.0f)
            : base(position, size) {
            FillColor = fillColor;
            BackgroundColor = new Vector4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundAlpha);
            ValueProvider = valueProvider;
            MinValue = minValue;
            MaxValue = maxValue;
            ShowPercentageText = showPercentageText;
        }

        public override void Render() {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin("ProgressBar", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, FillColor);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, BackgroundColor);

            float value = ValueProvider();
            float clampedValue = Math.Clamp(value, MinValue, MaxValue);
            float fraction = (clampedValue - MinValue) / (MaxValue - MinValue);

            if (ShowPercentageText) {
                ImGui.ProgressBar(fraction, Size, $"{fraction * 100}%");
            } else {
                ImGui.ProgressBar(fraction, Size, null);
            }

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();

            ImGui.End();
        }
    }
}