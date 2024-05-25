namespace Hell.player.ability {
    using Core.defaults;
    using Core.world;
    public class FireProjectileAbility : Ability {
        public FireProjectileAbility() : base(3.0f) { }

        public override void Use(Character character) {
            Console.WriteLine("FireProjectileAbility used");
        }
    }
}