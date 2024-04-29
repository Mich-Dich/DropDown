
using Core.game_objects;

namespace Core.physics {

    public interface ICollidable {

        transform transform { get; set; }
        // CollisionManager CollisionManager { get; set; }
    }
}