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
        public float bounceCombine { get; set; }

    }
}
