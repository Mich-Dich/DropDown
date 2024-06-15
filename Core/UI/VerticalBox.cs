using System.Numerics;

namespace Core.UI
{
    public class VerticalBox : UIElement
    {
        public readonly List<UIElement> elements;
        public Align Alignment { get; set; }
        private Vector2 padding;

        public VerticalBox(Vector2 position, Vector2 size, Vector2 padding, Align align) : base(position, size)
        {
            elements = new List<UIElement>();
            this.padding = padding;
            this.Alignment = align;
        }

        public new void AddElement(UIElement element)
        {
            elements.Add(element);
            OrganizeElements();
        }

        public void RemoveElement(UIElement element)
        {
            elements.Remove(element);
            OrganizeElements();
        }

        public UIElement? GetElementByTextureId(IntPtr textureId)
        {
            foreach (var element in elements)
            {
                if (element is Image image && image.TextureId == textureId)
                {
                    return image;
                }
            }
            return null;
        }

        public override void Render()
        {
            foreach (var element in elements)
            {
                element.Render();
            }
        }

        private void OrganizeElements()
        {
            Vector2 currentPosition = Position;
            foreach (var element in elements)
            {
                switch (Alignment)
                {
                    case Align.Left:
                        element.Position = currentPosition;
                        break;
                    case Align.Center:
                        element.Position = new Vector2(Position.X + (Size.X - element.Size.X) / 2, currentPosition.Y);
                        break;
                    case Align.Right:
                        currentPosition.X = Position.X + Size.X - element.Size.X - padding.X;
                        element.Position = currentPosition;
                        break;
                    default:
                        element.Position = currentPosition;
                        break;
                }
                currentPosition.Y += element.Size.Y + padding.Y;
            }
        }
    }
}