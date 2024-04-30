using Core.physics;
using Core.physics.material;
using Core.visual;
using OpenTK.Mathematics;

namespace Core.game_objects {

    public abstract class game_object {

        //public float mass { get; set; }
        //public Vector2 velocity { get; set; }
        //public physics_material physics_material {  get; set; }

        // world data
        public transform            transform { get; set; } = new transform();
        public game_object?         parent { get; private set; }
        public List<game_object>    children { get; } = new List<game_object>();

        // game_object data
        public sprite?              sprite { get; private set; }
        public collider?            collider { get; set; }

        // ======================= func =====================

        public game_object() { init(); }

        public game_object(transform transform) { this.transform = transform; init(); }
        
        public game_object(Vector2 position, Vector2 size, Single rotation, mobility mobility = mobility.DYNAMIC) {

            this.transform.position = position;
            this.transform.size = size;
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            init();
        }

        public void add_sprite(sprite sprite) {

            this.sprite = sprite;
            this.sprite.transform = transform;  // let sprite access main transform
        }

        public abstract void hit(hit_data hit);

        public void draw() {

            if(sprite == null)
                return;

            sprite.draw();

            //if(game.instance.draw_debug && this.collider != null)
            //    this.debugDrawer.DrawCollisionShape(this.Transform, this.CollisionManager, this.DebugColor);

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

        // =============================================== private ==============================================

        //private DebugDrawer debugDrawer;
        //private DebugColor DebugColor { get; set; } = DebugColor.Red;

        private void init() {

            //this.debugDrawer = new DebugDrawer();
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
