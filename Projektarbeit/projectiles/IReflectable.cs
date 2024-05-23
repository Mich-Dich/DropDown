using OpenTK.Mathematics;

namespace Hell.weapon {
    public interface IReflectable {
        public bool Reflected { get;}
        public void Reflect(Vector2 position);
    }
}