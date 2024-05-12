
namespace Core.world {

    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;

    public class Game_Object {

        public Transform            transform { get; set; } = new Transform();
        public Game_Object?         parent { get; private set; }
        public List<Game_Object>    children { get; } = new List<Game_Object>();
        public Collider?            collider { get; set; }

        /// <summary>
        /// Constructs a game object with the specified transform.
        /// </summary>
        /// <param name="transform">The transform of the game object.</param>
        public Game_Object(Transform transform) { this.transform = transform; Init(); }

        /// <summary>
        /// Constructs a game object with optional parameters for position, size, rotation, and mobility.
        /// </summary>
        /// <param name="position">The position of the game object.</param>
        /// <param name="size">The size of the game object.</param>
        /// <param name="rotation">The rotation of the game object.</param>
        /// <param name="mobility">The mobility type of the game object.</param>
        public Game_Object(Vector2? position = null, Vector2? size = null, Single rotation = 0, Mobility mobility = Mobility.DYNAMIC) {

            this.transform.position = position?? new Vector2();
            this.transform.size = size?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            Init();
        }

        // ======================= func =====================

        /// <summary>
        /// Sets the sprite of the game object.
        /// </summary>
        /// <param name="sprite">The sprite to assign to the game object.</param>
        /// <returns>The game object with the assigned sprite.</returns>
        public virtual Game_Object Set_Sprite(Sprite sprite) {

            this.sprite = sprite;
            this.sprite.transform = transform;  // let sprite access main transform
            return this;
        }

        /// <summary>
        /// Creates and sets a sprite using the provided texture for the game object.
        /// </summary>
        /// <param name="texture">The texture to use for creating the sprite.</param>
        /// <returns>The game object with the created sprite.</returns>
        public Game_Object Set_Sprite(Texture Texture) {

            this.sprite = new Sprite(Texture);
            this.sprite.transform = transform;  // let sprite access main transform
            return this;
        }

        /// <summary>
        /// Adds a collider to the game object.
        /// </summary>
        /// <param name="collider">The collider to add to the game object.</param>
        /// <returns>The game object with the added collider.</returns>
        public Game_Object Add_Collider(Collider collider) {

            this.collider = collider;
            this.collider.offset.mobility = this.transform.mobility;
            return this;
        }

        /// <summary>
        /// Sets the mobility type of the game object and updates associated components.
        /// </summary>
        /// <param name="mobility">The new mobility type to set for the game object.</param>
        /// <returns>The game object with the updated mobility type.</returns>
        public Game_Object Set_Mobility(Mobility mobility) {
            
            this.transform.mobility = mobility;
            if (this.sprite != null)
                this.sprite.set_mobility(mobility);
            if(this.collider != null)
                this.collider.offset.mobility = mobility;
            return this;
        }

        /// <summary>
        /// Handles the game object being hit by an external force or attack.
        /// </summary>
        /// <param name="hit">Data representing the hit event.</param>
        public virtual void Hit(hit_data hit) { }

        /// <summary>
        /// Updates the game object's state based on the elapsed time.
        /// </summary>
        /// <param name="delta_time">The time passed since the last update.</param>
        public virtual void Update(float delta_time) { }

        /// <summary>
        /// Draws the game object, rendering its associated sprite if available.
        /// </summary>
        public virtual void Draw() {
            
            if(sprite == null)
                return;

            sprite.Draw();
        }

        /// <summary>
        /// Adds a child game object to this game object.
        /// </summary>
        /// <param name="child">The child game object to add.</param>
        public void Add_Child(Game_Object child) {
            
            this.children.Add(child);
            child.parent = this;
            child.transform.parent = this.transform;
        }

        /// <summary>
        /// Removes a child game object from this game object.
        /// </summary>
        /// <param name="child">The child game object to remove.</param>
        public void Remove_Child(Game_Object child) {
            
            this.children.Remove(child);
            child.parent = null;
            child.transform.parent = null;
        }

        /// <summary>
        /// Checks if another game object is within a specified range of this game object.
        /// </summary>
        /// <param name="other">The other game object to check against.</param>
        /// <param name="range">The range within which to check for proximity.</param>
        /// <returns>True if the other game object is within the specified range, otherwise false.</returns>
        public bool Is_In_Range(Game_Object other, float range) {
        
            Vector2 distanceVector = this.transform.position - other.transform.position;
            float distance = distanceVector.Length;
            return distance <= range;
        }

        // =============================================== internal ==============================================

        /// <summary>
        /// Draws debug information for the game object, such as collider shapes.
        /// </summary>
        internal void Draw_Debug() {

            if(this.collider != null)
                this.debug_drawer?.draw_collision_shape(this.transform, this.collider, DebugColor.Red);
        }
        
        // =============================================== private ==============================================

        private Debug_Drawer? debug_drawer { get; set; }
        private Sprite? sprite;

        private void Init() {
            this.debug_drawer = new Debug_Drawer();
        }
    }

    public enum Mobility {

        STATIC = 0,
        MOVABLE = 1,
        DYNAMIC = 2,
    }
}