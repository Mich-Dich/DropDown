namespace Core.UI {
    public class Menu {
        private List<UIElement> elements = new List<UIElement>();

        public void AddElement(UIElement element) {
            elements.Add(element);
        }

        public void RemoveElement(UIElement element) {
            elements.Remove(element);
        }

        public void Draw() {
            foreach (var element in elements) {
                element.Draw();
            }
        }
    }
}