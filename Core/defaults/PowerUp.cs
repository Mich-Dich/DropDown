
using Box2DX.Collision;
using Box2DX.Dynamics;
using Core.physics;
using Core.util;
using Core.world;
using OpenTK.Mathematics;

namespace Core.defaults
{
    public abstract class PowerUp : Game_Object
    {

        private float live_time { get; set; } = 0f;     // 0f: infinit live_time
        public float Duration { get; set; } = 5f;
        public float ActivationTime { get; set; }
        public bool IsActivated { get; set; } = false;
        public Action<Character> activation { get; set; }
        public Action<Character> deactivation { get; set; }
        public Action destruction { get; set; }
        public string IconPath { get; set; }

        public PowerUp(Vector2 position, Vector2 size, Sprite sprite) : base(position, size)
        {
            Set_Sprite(sprite);
            Game.Instance.get_active_map().Add_Game_Object(this);

            // Initialize the collider
            Add_Collider(new Collider(Collision_Shape.Circle, Collision_Type.bullet, null, 1f));

            collider.body = Game.Instance.get_active_map().CreatePhysicsBody(position, size, true);
            collider.body.SetUserData(this);
        }

        public override void Update(float deltaTime)
        {
            if (live_time > 0f)
            {
                if (Game_Time.total >= ActivationTime + Duration)
                {
                    destruction();
                    Game.Instance.get_active_map().Remove_Game_Object(this);
                }
            }
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object == Game.Instance.player)
            {
                if (!IsActivated)
                {
                    Game.Instance.player.add_power_up(this);
                    IsActivated = true;
                }

                ActivationTime = Game_Time.total;

                Game.Instance.get_active_map().Remove_Game_Object(this);
            }
        }
    }

}