namespace Core.physics
{
    public struct Physics_Material
    {
        public float friction { get; set; }

        public float bounciness { get; set; }

        public Physics_Material(float dynamicFriction = 0.05f, float bounciness = 0.05f)
        {
            this.friction = dynamicFriction;
            this.bounciness = bounciness;
        }
    }
}
