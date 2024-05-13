namespace Core.world
{
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;

    public class Game_Object
    {
        public Transform transform { get; set; } = new Transform();

        public Game_Object? parent { get; private set; }

        public List<Game_Object> children { get; } = new List<Game_Object>();

        public Collider? collider { get; set; }

        public Game_Object(Transform transform)
        {
            this.transform = transform;
            this.Init();
        }

        public Game_Object(Vector2? position = null, Vector2? size = null, float rotation = 0, Mobility mobility = Mobility.DYNAMIC)
        {
            this.transform.position = position ?? default(Vector2);
            this.transform.size = size ?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            this.Init();
        }

        // ======================= func =====================
        public virtual Game_Object Set_Sprite(Sprite sprite)
        {
            this.sprite = sprite;
            this.sprite.transform = this.transform;  // let sprite access main transform
            return this;
        }

        public Game_Object Set_Sprite(Texture texture)
        {
            this.sprite = new Sprite(texture);
            this.sprite.transform = this.transform;  // let sprite access main transform
            return this;
        }

        public Game_Object Add_Collider(Collider collider)
        {
            this.collider = collider;
            this.collider.offset.mobility = this.transform.mobility;
            return this;
        }

        public Game_Object Set_Mobility(Mobility mobility)
        {
            this.transform.mobility = mobility;
            if (this.sprite != null)
            {
                this.sprite.Set_Mobility(mobility);
            }

            if (this.collider != null)
            {
                this.collider.offset.mobility = mobility;
            }

            return this;
        }

        public virtual void Hit(hitData hit)
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Draw()
        {
            if (this.sprite == null)
            {
                return;
            }

            this.sprite.Draw();
        }

        public void Add_Child(Game_Object child)
        {
            this.children.Add(child);
            child.parent = this;
            child.transform.parent = this.transform;
        }

        public void Remove_Child(Game_Object child)
        {
            this.children.Remove(child);
            child.parent = null;
            child.transform.parent = null;
        }

        public bool Is_In_Range(Game_Object other, float range)
        {
            Vector2 distanceVector = this.transform.position - other.transform.position;
            float distance = distanceVector.Length;
            return distance <= range;
        }

        // =============================================== internal ==============================================
        internal void Draw_Debug()
        {
            if (this.collider != null)
            {
                this.debug_drawer?.Draw_Collision_Shape(this.transform, this.collider, DebugColor.Red);
            }
        }

        // =============================================== private ==============================================
        private Debug_Drawer? debug_drawer { get; set; }

        private Sprite? sprite;

        private void Init()
        {
            this.debug_drawer = new Debug_Drawer();
        }
    }

    public enum Mobility
    {
        STATIC = 0,
        MOVABLE = 1,
        DYNAMIC = 2,
    }
}