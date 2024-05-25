namespace Core.defaults {
    using Core.util;
    using Core.world;

    public abstract class Ability
    {
        public float Duration { get; set; }
        public float LastUsedTime { get; set; }

        public abstract bool CanUse();
        public abstract void Use(Character character);
    }

    public class NoAbility : Ability
    {
        public override bool CanUse() => true;
        public override void Use(Character character) { /* Do nothing */ }
    }
}