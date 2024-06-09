
namespace Core.defaults {

    using Core.Controllers.player;
    using Core.world;

    public class PC_empty : Player_Controller {

        public PC_empty(Character character)
                : base(character, null) { }

        protected override void Update(float deltaTime) { }

    }
}
