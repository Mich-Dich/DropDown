using Core.physics;
using Core.physics.material;
using OpenTK.Mathematics;

namespace Core.game_objects {

    public class game_object {

        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public Vector2 scale { get; set; }
        public float rotation { get; set; }
        public float mass { get; set; }
        public Vector2 velocity { get; set; }
        public physics_material physics_material {  get; set; }

        public mobility mobility { get; set; }      // conserning update method
        public primitive shape { get; set; }

        //public List<game_object> childer {  get; set; }       // tODO: add capapility for children

        // ======================= func =====================

        public game_object() { }
        
        public game_object(Vector2 position, Vector2 size, Vector2 scale, Single rotation, Single mass, Vector2 velocity, physics_material physics_material, mobility mobility) {

            this.position = position;
            this.size = size;
            this.scale = scale;
            this.rotation = rotation;
            this.mass = mass;
            this.velocity = velocity;
            this.physics_material = physics_material;
            this.mobility = mobility;
            this.shape = primitive.SQUARE;
        }

        public void hit(hit_data hit) { }

    }

    public enum mobility {

        STATIC = 0,
        MOVABLE = 1,
        DYNAMIC = 2,
    }

    public enum primitive {

        CIRCLE = 0,
        SQUARE = 1,
    }
}
