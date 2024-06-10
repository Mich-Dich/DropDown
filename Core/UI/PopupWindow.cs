using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace Core.UI
{
    public class PopupWindow : UIElement
    {
        private readonly List<UIElement> elements = new();

        public Vector4 Color { get; set; }

        public PopupWindow(Vector2 position, Vector2 size) : base(position, size)
        {
        }

        public PopupWindow(Vector2 position, Vector2 size, Vector4 color) : this(position, size)
        {
            Color = color;
        }

        public new void AddElement(UIElement element)
        {
            elements.Add(element);
        }

        public override void Render()
        {
            if (!IsActive) return;

            ImGui.SetNextWindowPos(Position);
            ImGui.SetNextWindowSize(Size);
            ImGui.Begin("PopupWindow", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);

            foreach (var element in elements)
            {
                element.Render();
            }

            ImGui.End();
        }
    }
}