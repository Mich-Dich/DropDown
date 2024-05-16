namespace Hell.enemy {
    using Core.Controllers.ai;
    using Core.world;
    using OpenTK.Mathematics;

    public class TestEnemyController : AIC_simple {

        public TestEnemyController() : base(false) {
            float radius = 500f;

            Vector2 center = new Vector2(0, 0);

            for (int i = 0; i < 6; i++) {
                TestEnemy enemy = new TestEnemy();

                float angle = i * MathHelper.TwoPi / 6;

                enemy.transform.position = center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

                characters.Add(enemy);
            }

            AddCharactersToMap();

            Set_Statup_State(typeof(EnterScreen));
        }

        public override bool Exit() {
            return base.Exit();
        }

        public override bool Enter() {
            return base.Enter();
        }

        public override Type Execute() {
            return base.Execute();
        }
    }
}