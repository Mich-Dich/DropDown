using Core.world;

namespace Core.defaults
{
    public abstract class Ability
    {
        public float Cooldown { get; set; }
        public string IconPath { get; set; }
        public AbilityEffect? Effect { get; set; }
        public abstract void Use(Character character);
        public bool IsActive { get; set; } = false;
        public bool IsEquipped { get; set; }
        public bool IsUnlocked { get; set; }
        public int UnlockCost { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public bool IsLocked { get; set; }

        public void AddEffectToCharacter(Character character)
        {
            this.Effect.IsRemoved = false;
            character.Add_Child(this.Effect);
        }

        public void Unlock()
        {
            IsLocked = false;
        }
    }
}