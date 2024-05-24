namespace Core.defaults {
    using Core.util;
    using Core.world;

    public abstract class Ability {
        public float Cooldown { get; set; }
        public float LastUsedTime { get; set; }

        public Ability(float cooldown) {
            Cooldown = cooldown;
            LastUsedTime = -Cooldown;
        }

        public bool CanUse() {
            return Game_Time.total - LastUsedTime >= Cooldown;
        }

        public abstract void Use(Character character);
    }
}