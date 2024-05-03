using Core.physics;
using Core.renderer;
using Core.visual;
using OpenTK.Mathematics;

namespace Core.game_objects {

    public /*abstract*/ class game_object {

        public transform transform { get; set; } = new transform();
        public game_object? parent { get; private set; }
        public List<game_object> children { get; } = new List<game_object>();
        public collider? collider { get; set; }

        public game_object(transform transform) { this.transform = transform; init(); }

        public game_object(Vector2? position = null, Vector2? size = null, Single rotation = 0, mobility mobility = mobility.DYNAMIC) {

            this.transform.position = position?? new Vector2();
            this.transform.size = size?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            init();
        }

        // ======================= func =====================

        public game_object set_sprite(sprite sprite) {
            
            this.sprite = sprite;
            this.sprite.transform = transform;  // let sprite access main transform
            return this;
        }

        public game_object set_sprite(Texture Texture) {
            
            this.sprite = new sprite(Texture);
            this.sprite.transform = transform;  // let sprite access main transform
            return this;
        }

        public game_object set_collider(collider collider) {
            
            this.collider = collider;
            return this;
        }

        public virtual void hit(hit_data hit) { }

        public void draw() {
            
            if(sprite == null)
                return;

            sprite.draw();

            if(game.instance.draw_debug && this.collider != null)
                this.debug_drawer?.draw_collision_shape(this.transform, this.collider.Value, DebugColor.Red);
            
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

        private debug_drawer? debug_drawer { get; set; }
        private sprite? sprite;
        private bool is_hit = false;

        private void init() {
            this.debug_drawer = new debug_drawer();
        }
    }

    public enum mobility {

        STATIC = 0,
        MOVABLE = 1,
        DYNAMIC = 2,
    }
}