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
        }

        public override void Use(Character character)
        {
            Console.WriteLine("Test ability used!");
        }
    }
}