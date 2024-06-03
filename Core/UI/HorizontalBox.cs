namespace Core.UI {
    using System.Collections.Generic;
    using System.Numerics;

    public class HorizontalBox : UIElement {
        private List<UIElement> elements;
        private Vector2 padding;

        public HorizontalBox(Vector2 position, Vector2 size, Vector2 padding) : base(position, size) {
            elements = new List<UIElement>();
            this.padding = padding;
        }

        public void AddElement(UIElement element) {
            elements.Add(element);
            OrganizeElements();
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
                currentPosition.X += element.Size.X + padding.X;
            }
        }
    }
}