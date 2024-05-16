namespace Hell.enemy {

    using Core.Controllers.ai;
    using Core.world;

    public class TestEnemyController : AIC_simple {

        public TestEnemyController(Character character) : base(character) {
            // Initialize specific states for TestEnemyController
        }

        public override bool Exit() {
            // Override as needed for TestEnemy
            return base.Exit();
        }

        public override bool Enter() {
            // Override as needed for TestEnemy
            return base.Enter();
        }

        public override Type Execute() {
            // Override as needed for TestEnemy
            return base.Execute();
        }
    }
}