namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class FireRateBoost : PowerUp
    {
        private float originalFireDelay;
        private float originalAbilityCooldown;
        private float abilityCooldownReduction;
        
        // Cache sound for power-up collection
        private static readonly Sound powerUpSound = Resource_Manager.Get_Sound("assets/sounds/whoosh-flame01short.wav");

        public FireRateBoost(Vector2 position, float fireDelayDecrease, float duration)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/firerate_increaser.png")))
        {
            IconPath = "assets/textures/power-ups/firerate_increaser.png";
            activation = ActivatePowerUp;
            deactivation = DeactivatePowerUp;
            destruction = () => { };

            Name = "FireRateBoost";
            Description = "Decreases the fire delay.";
            UnlockCost = 20;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 20;

            FireDelayDecrease = fireDelayDecrease;
            Duration = duration;
            abilityCooldownReduction = 0.3f; // 30% ability cooldown reduction
        }

        public override void Upgrade()
        {
            base.Upgrade();

            FireDelayDecrease += 0.15f; // Increased from 0.1f to 0.15f for more impact
            abilityCooldownReduction += 0.1f; // Increase ability cooldown reduction per level
            Duration = Level % 2 != 0 ? Duration + 1 : Duration;

            LogUpgradeDetails();
            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }

        private void ActivatePowerUp(Character target)
        {
            target.ActivePowerUps.Add(this);
            
            // Play power-up collection sound
            _ = powerUpSound.Play();
            
            LogActivationDetails();

            if (target != null)
            {
                // Reduce fire delay
                originalFireDelay = target.fireDelay;
                float oldFireDelay = target.fireDelay;
                target.fireDelay = Math.Max(0.05f, target.fireDelay - FireDelayDecrease); // Minimum fire delay of 0.05s
                Console.WriteLine($"FireRateBoost: Fire delay {oldFireDelay} -> {target.fireDelay} (-{FireDelayDecrease})");
                
                // Reduce ability cooldown if player has an ability
                if (target.Ability != null)
                {
                    originalAbilityCooldown = target.Ability.Cooldown;
                    float cooldownReduction = originalAbilityCooldown * abilityCooldownReduction;
                    target.Ability.Cooldown = Math.Max(0.5f, target.Ability.Cooldown - cooldownReduction); // Minimum ability cooldown of 0.5s
                    Console.WriteLine($"FireRateBoost: Ability cooldown {originalAbilityCooldown} -> {target.Ability.Cooldown} (-{cooldownReduction:F2})");
                }
                
                Console.WriteLine($"FireRateBoost activated for {Duration} seconds");
            }
            else
            {
                Console.WriteLine("FireRateBoost: Target is null");
            }
        }

        private void DeactivatePowerUp(Character target)
        {
            target.ActivePowerUps.Remove(this);

            if (target != null)
            {
                // Restore fire delay
                float oldFireDelay = target.fireDelay;
                target.fireDelay = originalFireDelay;
                Console.WriteLine($"FireRateBoost: Fire delay restored {oldFireDelay} -> {target.fireDelay}");
                
                // Restore ability cooldown if player has an ability
                if (target.Ability != null)
                {
                    float oldAbilityCooldown = target.Ability.Cooldown;
                    target.Ability.Cooldown = originalAbilityCooldown;
                    Console.WriteLine($"FireRateBoost: Ability cooldown restored {oldAbilityCooldown} -> {target.Ability.Cooldown}");
                }
                
                Console.WriteLine("FireRateBoost deactivated");
            }
            else
            {
                Console.WriteLine("FireRateBoost: Target is null during deactivation");
            }
        }

        private void LogUpgradeDetails()
        {
            Console.WriteLine($"FireRateBoost upgraded to level {Level}");
            Console.WriteLine($"FireRateBoost: {FireDelayDecrease} activated for {Duration} seconds");
        }

        private void LogActivationDetails()
        {
            Console.WriteLine("FireRateBoost activated");
            Console.WriteLine($"FireRateBoost: {FireDelayDecrease} activated for {Duration} seconds");
        }
    }
}
