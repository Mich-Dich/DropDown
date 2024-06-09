
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

        public UIElement(System.Numerics.Vector2 position, System.Numerics.Vector2 size)
        {
            Position = position;
            Size = size;
            IsActive = true;
        }

        public abstract void Render();
    }
}