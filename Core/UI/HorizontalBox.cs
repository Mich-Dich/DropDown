using System.Numerics;

namespace Core.UI
{
    public class HorizontalBox : UIElement
    {
        public List<UIElement> elements;
        public Align Alignment { get; set; }
        private Vector2 padding;

        public HorizontalBox(Vector2 position, Vector2 size, Vector2 padding, Align align) : base(position, size)
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
            float totalWidth = elements.Sum(e => e.Size.X + padding.X) - padding.X;
            Vector2 startPosition = Position;

            switch (Alignment)
            {
                case Align.Center:
                    startPosition.X += (Size.X - totalWidth) / 2;
                    break;
                case Align.Right:
                    startPosition.X += Size.X - totalWidth;
                    break;
            }

            foreach (var element in elements)
            {
                element.Position = new Vector2(startPosition.X, Position.Y + (Size.Y - element.Size.Y) / 2);
                startPosition.X += element.Size.X + padding.X;
            }
        }
    }
}