using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.physics.material {

    public struct physics_material {

        public float friction  { get; set; }
        public float bounciness  { get; set; }

        public physics_material(Single dynamicFriction = 0.05f, Single bounciness = 0.05f) {

            this.friction = dynamicFriction;
            this.bounciness = bounciness;
        }
    }
}
