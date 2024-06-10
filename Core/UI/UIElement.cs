namespace Core.UI
{
    public enum Align
    {
        Left,
        Center,
        Right
    }

    public abstract class UIElement
    {
        public System.Numerics.Vector2 Position { get; set; }
        public System.Numerics.Vector2 Size { get; set; }
        public bool IsActive { get; set; }
        public List<UIElement> Elements { get; set; }

        public UIElement(System.Numerics.Vector2 position, System.Numerics.Vector2 size)
        {
            Position = position;
            Size = size;
            IsActive = true;
            Elements = new List<UIElement>();
        }

        public void AddElement(UIElement element)
        {
            Elements.Add(element);
        }

        public virtual void Render()
        {
            foreach (var element in Elements)
            {
                element.Render();
            }
        }
    }
}