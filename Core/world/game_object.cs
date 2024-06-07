namespace Core.world
{
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;

    public class Game_Object
    {

        public bool is_sensore { get; set; } = false;
        public Transform transform { get; set; } = new Transform();
        public Game_Object? parent { get; private set; }
        public List<Game_Object> children { get; } = new List<Game_Object>();
        public Collider? collider { get; set; }
        public float rotation_offset { get; set; } = 0;
        public bool IsRemoved { get; set; } = false;

        public Game_Object(Transform transform)
        {

            if (Game.Instance == null || Game.Instance.get_active_map() == null || Game.Instance.get_active_map().physicsWorld == null)
                throw new Exception("Game instance, active map, or physics world is not initialized");

            this.transform = transform;
            this.Init();
        }

        public Game_Object(Vector2? position = null, Vector2? size = null, float rotation = 0, Mobility mobility = Mobility.DYNAMIC)
        {

            if (Game.Instance == null || Game.Instance.get_active_map() == null || Game.Instance.get_active_map().physicsWorld == null)
                throw new Exception("Game instance, active map, or physics world is not initialized");

            this.transform.position = position ?? default(Vector2);
            this.transform.size = size ?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            this.Init();
        }

        // ---------------------------------------------------------------------------------------------------------------
        // default setters/getters
        // ---------------------------------------------------------------------------------------------------------------

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
                this.sprite.Set_Mobility(mobility);

            if (this.collider != null)
                this.collider.offset.mobility = mobility;

            return this;
        }

        public void rotate_to_move_dir()
        {

            Box2DX.Common.Vec2 movement_dir = collider.body.GetLinearVelocity();
            movement_dir.Normalize();
            float angleRadians = (float)System.Math.Atan2(movement_dir.X, movement_dir.Y);
            transform.rotation = -angleRadians + rotation_offset;
        }

        public void rotate_to_move_dir_smooth()
        {

            Box2DX.Common.Vec2 movement_dir = collider.body.GetLinearVelocity();
            movement_dir.Normalize();
            float target_angle = (float)System.Math.Atan2(-movement_dir.Y, movement_dir.X);

            float current_angle = -transform.rotation + rotation_offset;
            while (target_angle - current_angle > MathF.PI) target_angle -= 2 * MathF.PI;
            while (target_angle - current_angle < -MathF.PI) target_angle += 2 * MathF.PI;

            float new_angle = util.Lerp(current_angle, target_angle, 0.1f);
            transform.rotation = -new_angle + rotation_offset;
        }

        public void rotate_to_vector(Box2DX.Common.Vec2 dir)
        {

            float angleRadians = util.angle_from_vec(dir);
            transform.rotation = -angleRadians + rotation_offset;
        }

        public void rotate_to_vector(Vector2 dir)
        {

            float angleRadians = util.angle_from_vec(dir);
            transform.rotation = -angleRadians + rotation_offset;
        }

        public void rotate_to_vector_smooth(Vector2 dir)
        {

            dir.NormalizeFast();
            float target_angle = (float)System.Math.Atan2(-dir.Y, dir.X); // Invert Y-coordinate

            float current_angle = -transform.rotation + rotation_offset;
            while (target_angle - current_angle > MathF.PI) target_angle -= 2 * MathF.PI;
            while (target_angle - current_angle < -MathF.PI) target_angle += 2 * MathF.PI;

            float new_angle = util.Lerp(current_angle, target_angle, 0.1f);
            transform.rotation = -new_angle + rotation_offset;
        }

        public void rotate_to_vector_smooth(Box2DX.Common.Vec2 dir)
        {

            dir.Normalize();
            float target_angle = (float)System.Math.Atan2(-dir.Y, dir.X); // Invert Y-coordinate

            float current_angle = -transform.rotation + rotation_offset;
            while (target_angle - current_angle > MathF.PI) target_angle -= 2 * MathF.PI;
            while (target_angle - current_angle < -MathF.PI) target_angle += 2 * MathF.PI;

            float new_angle = util.Lerp(current_angle, target_angle, 0.1f);
            transform.rotation = -new_angle + rotation_offset;
        }

        // ---------------------------------------------------------------------------------------------------------------
        // interaction
        // ---------------------------------------------------------------------------------------------------------------

        public virtual void Hit(hitData hit) { }

        public virtual void Update(float deltaTime) { }

        public virtual void draw_imgui() { }

        public virtual void Draw()
        {

            if (this.sprite == null)
                return;

            this.sprite.Draw();
        }

        public void Add_Child(Game_Object child)
        {

            this.children.Add(child);
            child.parent = this;
            child.transform.parent = this.transform;
            Console.WriteLine("Current Child " + child.GetType().ToString());
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

        public void Update_position()
        {

            if (collider.body == null)
                return;

            Box2DX.Common.Vec2 pos = this.collider.body.GetPosition();
            this.transform.position = (pos.X, pos.Y);
        }

        // =============================================== internal ==============================================
        internal void Draw_Debug()
        {

            if (collider != null)
                debug_drawer?.Draw_Collision_Shape(this.transform, this.collider, Core.render.DebugColor.Red);
        }

        // =============================================== private ==============================================
        private Core.render.Debug_Drawer? debug_drawer { get; set; }
        public Sprite? sprite { get; private set; }

        private void Init() { debug_drawer = new Core.render.Debug_Drawer(); }
    }

    public enum Mobility
    {

        STATIC = 0,
        MOVABLE = 1,
        DYNAMIC = 2,
    }
}