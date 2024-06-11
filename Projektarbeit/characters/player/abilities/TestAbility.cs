namespace Projektarbeit.characters.player.abilities
{
    using Core.defaults;
    using Core.world;

    public class TestAbility : Ability
    {
        public TestAbility()
        {
            Cooldown = 5.0f;
            Effect = null;

            Name = "Test";
            Description = "Test ability.";
            UnlockCost = 10;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 10;
        }

        public override void Use(Character character)
        {
            Console.WriteLine("Test ability used!");
        }
    }
}