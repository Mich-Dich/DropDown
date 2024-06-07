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
        public float MaxValue { get; set; }
        public bool ShowPercentageText { get; set; }
        private static int counter = 0;
        private readonly string windowName;


        public ProgressBar(Vector2 position, Vector2 size, Vector4 fillColor, Vector4 backgroundColor, Func<float> valueProvider, float minValue, float maxValue, bool showPercentageText = false, float backgroundAlpha = 1.0f)
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
            float clampedValue = Math.Clamp(value, MinValue, MaxValue);
            float fraction = (clampedValue - MinValue) / (MaxValue - MinValue);

            if (ShowPercentageText)
            {
                ImGui.PushStyleColor(ImGuiCol.PlotHistogram, FillColor);
                ImGui.PushStyleColor(ImGuiCol.FrameBg, BackgroundColor);
                ImGui.ProgressBar(fraction, Size, $"{fraction * 100}%");
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();
            }
            else
            {
                uint color = ImGui.GetColorU32(FillColor);
                uint transparentColor = ImGui.GetColorU32(new System.Numerics.Vector4(0, 0, 0, 0));

                float progressBarWidth = Size.X * (MaxValue / 100f);
                Imgui_Util.Progress_Bar_Stylised(ValueProvider(), new System.Numerics.Vector2(progressBarWidth, Size.Y), color, transparentColor, 0.32f, 0.28f, 0.6f);
            }

            ImGui.End();
        }
    }
}