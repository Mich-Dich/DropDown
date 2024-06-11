namespace Projektarbeit.characters.player.abilities
{
    using Core.defaults;
    using Core.world;

    public class NoAbility : Ability
    {
        public NoAbility()
        {
            Cooldown = 5.0f;
            Effect = null;

            Name = "No Ability";
            Description = "No Ability.";
            UnlockCost = 10;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 10;
        }

        public override void Use(Character character)
        {
            
        }
    }
}