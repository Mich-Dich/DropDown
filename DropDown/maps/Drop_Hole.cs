
namespace DropDown.maps {

    using Core;
    using Core.physics;
    using Core.world;

    internal class Drop_Hole : Game_Object {

        public Action enter { get; set; }

        public override void Hit(hitData hit) {
            base.Hit(hit);

            if(hit.hit_object == Game.Instance.player)
                Game.Instance.set_active_map(new MAP_base());

        }

    }
}
