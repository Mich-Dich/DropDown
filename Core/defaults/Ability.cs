namespace Core.defaults {
    using Core.world;
    using Core.util;

    public abstract class Ability
    {
        public float Cooldown { get; set; }
        public string IconPath { get; set; }
        public AbilityEffect? Effect { get; set; }
        public abstract void Use(Character character);
        public bool IsActive { get; set; } = false;

        public void AddEffectToCharacter(Character character)
        {
            character.Add_Child(this.Effect);
        }
    }
}
