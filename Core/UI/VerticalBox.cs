namespace Core.UI {
    using System.Collections.Generic;
    using System.Numerics;

    public class VerticalBox : UIElement {
        private List<UIElement> elements;
        private Vector2 padding;

        public VerticalBox(Vector2 position, Vector2 size, Vector2 padding) : base(position, size) {
            elements = new List<UIElement>();
            this.padding = padding;
        }

        public void AddElement(UIElement element) {
            elements.Add(element);
            OrganizeElements();
        }

        public void RemoveElement(UIElement element) {
            elements.Remove(element);
            OrganizeElements();
        }

        public UIElement GetElementByTextureId(IntPtr textureId) {
            foreach (var element in elements) {
                if (element is Image image && image.TextureId == textureId) {
                    return image;
                }
            }
            return null;
        }

        public override void Render() {
            foreach (var element in elements) {
                element.Render();
            }
        }

        private void OrganizeElements() {
            Vector2 currentPosition = Position;
            foreach (var element in elements) {
                element.Position = currentPosition;
                currentPosition.Y += element.Size.Y + padding.Y;
            }
        }
    }
}