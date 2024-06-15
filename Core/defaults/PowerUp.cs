
using Box2DX.Collision;
using Box2DX.Dynamics;
using Core.physics;
using Core.util;
using Core.world;
using OpenTK.Mathematics;

namespace Core.defaults
{
    public abstract class PowerUp : Game_Object
    {

        private float live_time { get; set; } = 0f;
        public float Duration { get; set; }
        public float ActivationTime { get; set; }
        public bool IsActivated { get; set; } = false;
        public Action<Character> activation { get; set; }
        public Action<Character> deactivation { get; set; }
        public Action destruction { get; set; }
        public string IconPath { get; set; }
        public bool IsLocked { get; set; } = true;
        public int BaseUpgradeCost { get; set; }
        public float UpgradeMultiplier { get; set; }
        public bool IsEquipped { get; set; }
        public int UnlockCost { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public float SpeedBoost { get; set; }
        public float DurationBoost { get; set; }
        public const int MaxLevel = 8;

        public PowerUp(Vector2 position, Vector2 size, Sprite sprite) : base(position, size)
        {
            Set_Sprite(sprite);
            Game.Instance.get_active_map().Add_Game_Object(this);

            // Initialize the collider
            Add_Collider(new Collider(Collision_Shape.Circle, Collision_Type.bullet, null, 1f));

            collider.body = Game.Instance.get_active_map().CreatePhysicsBody(position, size, true);
            collider.body.SetUserData(this);
        }

        public override void Update(float deltaTime)
        {
            if (live_time > 0f)
            {
                if (Game_Time.total >= ActivationTime + Duration)
                {
                    destruction();
                    Game.Instance.get_active_map().Remove_Game_Object(this);
                }
            }
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object == Game.Instance.player)
            {
                if (!IsActivated)
                {
                    Game.Instance.player.add_power_up(this);
                    IsActivated = true;
                }

                ActivationTime = Game_Time.total;

                Game.Instance.get_active_map().Remove_Game_Object(this);
            }
        }

        public void Unlock()
        {
            IsLocked = false;
            Level = 1;
            Game.Instance.GameState.Currency -= UnlockCost;
            Game.Instance.GameState.PowerUps.Add(this);
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

        public void LoadFromSaveData(PowerUpSaveData saveData)
        {
            IsLocked = saveData.IsLocked;
            BaseUpgradeCost = saveData.BaseUpgradeCost;
            UpgradeMultiplier = saveData.UpgradeMultiplier;
            IsEquipped = saveData.IsEquipped;
            UnlockCost = saveData.UnlockCost;
            Name = saveData.Name;
            Description = saveData.Description;
            SpeedBoost = saveData.SpeedBoost;
            DurationBoost = saveData.DurationBoost;
            Duration = saveData.Duration;
            Level = saveData.Level;
        }

        public PowerUpSaveData ToSaveData()
        {
            return new PowerUpSaveData
            {
                IsLocked = this.IsLocked,
                BaseUpgradeCost = this.BaseUpgradeCost,
                UpgradeMultiplier = this.UpgradeMultiplier,
                IsEquipped = this.IsEquipped,
                UnlockCost = this.UnlockCost,
                Name = this.Name,
                Description = this.Description,
                Level = this.Level,
                SpeedBoost = this.SpeedBoost,
                DurationBoost = this.DurationBoost,
                Duration = this.Duration,
                PowerUpType = this.GetType().Name
            };
        }
    }
}