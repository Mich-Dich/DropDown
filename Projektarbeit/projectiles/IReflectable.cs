namespace Projektarbeit.projectiles
{
    using OpenTK.Mathematics;

    public interface IReflectable
    {
        float Damage { get; }

        public bool Reflected { get; }

        public void Reflect(Vector2 position);
    }
}