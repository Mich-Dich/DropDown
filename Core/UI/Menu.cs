namespace Core.UI {
    using System.Collections.Generic;
    using System.Numerics;

    public class Menu {
        private List<UIElement> elements;

        public Menu() {
            elements = new List<UIElement>();
        }

        public void AddElement(UIElement element) {
            elements.Add(element);
        }

        public void Render() {
            foreach (var element in elements) {
                element.Render();
            }
        }
    }
}
