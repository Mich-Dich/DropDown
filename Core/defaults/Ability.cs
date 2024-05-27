namespace Core.defaults {
    using Core.world;

    public abstract class Ability
    {
        public float Cooldown { get; set; }
        public AbilityEffect? Effect { get; set; }
        public abstract void Use(Character character);

        public void AddEffectToCharacter(Character character)
        {
            character.Add_Child(this.Effect);
        }
    }
}