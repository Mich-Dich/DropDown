namespace Core.util
{
    public class PowerUpSaveData
    {
        public bool IsLocked { get; set; }
        public int BaseUpgradeCost { get; set; }
        public float UpgradeMultiplier { get; set; }
        public bool IsEquipped { get; set; }
        public int UnlockCost { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public float SpeedBoost { get; set; }
        public float DurationBoost { get; set; }
        public float Duration { get; set; }
        public string PowerUpType { get; set; }
        public float FireDelayDecrease { get; set; } 
    }
}