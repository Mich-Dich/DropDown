namespace Hell.player.ability {
    using Core.defaults;
    using Core.world;
    public class ShieldBubbleAbility : Ability {
        public ShieldBubbleAbility() : base(5.0f) { }

        public override void Use(Character character) {
            Console.WriteLine("ShieldBubbleAbility used");
        }
    }
}