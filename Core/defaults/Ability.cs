namespace Core.defaults {
    using Core.world;

    public abstract class Ability
    {
        public float Cooldown { get; set; }

        public abstract void Use(Character character);
    }
}