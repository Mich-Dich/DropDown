
namespace DropDown.player {

    using Core.Controllers.player;
    using Core.world;

    public class PC_empty : Player_Controller {

        public PC_empty()
            : base(null, null) { }

        protected override void Update(float deltaTime) { }

    }
}
