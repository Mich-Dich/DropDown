using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.physics.material {

    public struct physics_material {

        public float dynamicFriction  { get; set; }
        public float staticFriction  { get; set; }
        public float bounciness  { get; set; }
        public float frictionCombine  { get; set; }

        public physics_material(Single dynamicFriction = 0.05f, Single staticFriction = 0.05f, Single bounciness = 0.05f) {

            this.dynamicFriction = dynamicFriction;
            this.staticFriction = staticFriction;
            this.bounciness = bounciness;
            this.frictionCombine = this.dynamicFriction + this.staticFriction;
        }
    }
}
