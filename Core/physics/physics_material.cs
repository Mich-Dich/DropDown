namespace Core.physics {

    public struct physics_material {

        public float friction { get; set; }
        public float bounciness { get; set; }

        public physics_material(float dynamicFriction = 0.05f, float bounciness = 0.05f) {

            friction = dynamicFriction;
            this.bounciness = bounciness;
        }
    }
}
