namespace Hell.weapon
{
    public interface IProjectile
    {
        float Damage { get; }

        public bool HasHit { get; set; }

        bool FiredByPlayer { get; }
    }
}