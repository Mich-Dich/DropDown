namespace Hell.player.ability {
    using Core.defaults;
    using Core.world;

    public class TestAbility : Ability
    {
        public TestAbility() {
            Cooldown = 5.0f; // 5 seconds cooldown
        }

        public override void Use(Character character) {
            Console.WriteLine("Test ability used!");
        }
    }
}