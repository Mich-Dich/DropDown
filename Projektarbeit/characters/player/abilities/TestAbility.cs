namespace Hell.player.ability
{
    using Core.defaults;
    using Core.world;

    public class TestAbility : Ability
    {
        public TestAbility()
        {
            this.Cooldown = 5.0f;
            this.Effect = null;
        }

        public override void Use(Character character)
        {
            Console.WriteLine("Test ability used!");
        }
    }
}