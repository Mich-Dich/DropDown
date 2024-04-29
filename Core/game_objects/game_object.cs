using Core.physics;
using Core.physics.material;
using Core.visual;
using OpenTK.Mathematics;

namespace Core.game_objects {

    public abstract class game_object : ICollidable {

        //public float mass { get; set; }
        //public Vector2 velocity { get; set; }
        //public physics_material physics_material {  get; set; }

        public mobility mobility { get; set; } = mobility.DYNAMIC;    // conserning update method
        public transform transform { get; set; } = new transform();

        public sprite? sprite { get; set; }

        public game_object? parent { get; private set; }
        public List<game_object> children { get; } = new List<game_object>();

        //public List<game_object> childer {  get; set; }       // tODO: add capapility for children

        // ======================= func =====================

        public game_object() { }
        
        public game_object(Vector2 position, Vector2 size, Single rotation, mobility mobility = mobility.DYNAMIC) {

            transform.position = position;
            transform.size = size;
            transform.rotation = rotation;
            this.mobility = mobility;
        }

        public abstract void hit(hit_data hit);

        public void draw() {

            if(sprite == null)
                return;

        }

        public void add_child(game_object child) {
         
            this.children.Add(child);
            child.parent = this;
            child.transform.parent = this.transform;
        }

        public void remove_child(game_object child) {

            this.children.Remove(child);
            child.parent = null;
            child.transform.parent = null;
        }

        public bool is_in_range(game_object other, float range) {

            Vector2 distanceVector = this.transform.position - other.transform.position;
            float distance = distanceVector.Length;
            return distance <= range;
        }

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
