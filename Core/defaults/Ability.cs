using Core.world;
using System;

namespace Core.defaults
{
    [Serializable]
    public abstract class Ability
    {
        public float Cooldown { get; set; }
        public string IconPath { get; set; }
        public AbilityEffect? Effect { get; set; }
        public abstract void Use(Character character);
        public bool IsActive { get; set; } = true;
        public bool IsEquipped { get; set; }
        public int UnlockCost { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public bool IsLocked { get; set; } = true;
        public int BaseUpgradeCost { get; set; }
        public float UpgradeMultiplier { get; set; }

        public void AddEffectToCharacter(Character character)
        {
            this.Effect.IsRemoved = false;
            character.Add_Child(this.Effect);
        }

        public void Unlock()
        {
            IsLocked = false;
            Game.Instance.GameState.Currency -= UnlockCost;
            Game.Instance.GameState.Abilities.Add(this);
            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }

        public virtual void Upgrade()
        {
            if (Game.Instance.GameState.Currency >= UnlockCost)
            {
                Level++;
                Game.Instance.GameState.Currency -= UnlockCost;
                UnlockCost = (int)(BaseUpgradeCost * Math.Pow(Level, UpgradeMultiplier) * Math.Log10(Level + 2));
                GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
            }
            else
            {
                // Display a message to the user that they don't have enough currency
            }
        }

        public void ToggleEquip()
        {
            if (IsEquipped)
            {
                // Unequip the ability
                IsEquipped = false;
                if (Game.Instance.player.Ability == this)
                {
                    
                }
            }
            else
            {
                // Unequip all other abilities
                foreach (var ability in Game.Instance.GameState.Abilities)
                {
                    if (ability.IsEquipped)
                    {
                        ability.IsEquipped = false;
                    }
                }

                // Equip this ability
                IsEquipped = true;
                Game.Instance.player.Ability = this;
            }

            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }
    }
}