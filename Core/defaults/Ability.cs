using Core.util;
using Core.world;
using System.Timers;

namespace Core.defaults
{
    [Serializable]
    public abstract class Ability
    {
        public float Cooldown { get; set; }
        public string IconPath { get; set; }
        public AbilityEffect? Effect { get; set; }
        public abstract void Use(Character character);
        public bool IsActive { get; set; } = false;
        public bool IsEquipped { get; set; }
        public int UnlockCost { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public bool IsLocked { get; set; } = true;
        public int BaseUpgradeCost { get; set; }
        public float UpgradeMultiplier { get; set; }

        public System.Timers.Timer timer;

        public Ability()
        {
            Core.Game.Instance.GameStateChanged += OnGameStateChanged;
        }

        public void AddEffectToCharacter(Character character)
        {
            this.Effect.IsRemoved = false;
            character.Add_Child(this.Effect);
        }

        public void Unlock()
        {
            IsLocked = false;
            Level = 1;
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
                UnlockCost = (int)(BaseUpgradeCost * Math.Pow(2, Level) * Math.Log10(Level + 2));
                GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
            }
            else
            {
                // Display a message to the user that they don't have enough currency
            }
        }

        public void ToggleEquip()
        {
            // Find the ability in the game state
            var gameStateAbility = Game.Instance.GameState.Abilities.FirstOrDefault(a => a.Name == this.Name);
            if (gameStateAbility != null)
            {
                if (gameStateAbility.IsEquipped)
                {
                    // Unequip the ability
                    gameStateAbility.IsEquipped = false;
                    if (Game.Instance.player.Ability == gameStateAbility)
                    {
                        Game.Instance.player.Ability = null;
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
                    gameStateAbility.IsEquipped = true;
                    Game.Instance.player.Ability = gameStateAbility;
                }

                GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
            }
        }

        public void LoadFromSaveData(AbilitySaveData saveData)
        {
            IsLocked = saveData.IsLocked;
            BaseUpgradeCost = saveData.BaseUpgradeCost;
            UpgradeMultiplier = saveData.UpgradeMultiplier;
            IsEquipped = saveData.IsEquipped;
            UnlockCost = saveData.UnlockCost;
            Name = saveData.Name;
            Description = saveData.Description;
            Level = saveData.Level;
        }

        public AbilitySaveData ToSaveData()
        {
            return new AbilitySaveData
            {
                IsLocked = this.IsLocked,
                BaseUpgradeCost = this.BaseUpgradeCost,
                UpgradeMultiplier = this.UpgradeMultiplier,
                IsEquipped = this.IsEquipped,
                UnlockCost = this.UnlockCost,
                Name = this.Name,
                Description = this.Description,
                Level = this.Level,
                AbilityType = this.GetType().Name
            };
        }

        private void OnGameStateChanged(object sender, GameStateChangedEventArgs e)
        {
            if (timer == null) return;

            if (e.NewState == Core.Play_State.InGameMenu)
            {
                timer.Stop();
            }
            else if (e.OldState == Core.Play_State.InGameMenu)
            {
                timer.Start();
            }
            else if (e.OldState == Core.Play_State.LevelUp)
            {
                timer.Start();
            }
            else if (e.NewState == Core.Play_State.LevelUp)
            {
                timer.Stop();
            }
            else if(e.NewState == Core.Play_State.PauseMenuSkillTree)
            {
                timer.Stop();
            }
            else if(e.OldState == Core.Play_State.PauseMenuSkillTree)
            {
                timer.Start();
            }
            else if(e.NewState == Core.Play_State.PauseAbilitySkillTree)
            {
                timer.Stop();
            }
            else if(e.OldState == Core.Play_State.PauseAbilitySkillTree)
            {
                timer.Start();
            }
            else if(e.NewState == Core.Play_State.PausePowerupSkillTree)
            {
                timer.Stop();
            }
            else if(e.OldState == Core.Play_State.PausePowerupSkillTree)
            {
                timer.Start();
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
            Core.Game.Instance.GameStateChanged -= OnGameStateChanged;
        }
    }
}