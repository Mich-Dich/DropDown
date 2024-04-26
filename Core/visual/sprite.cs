using Core.game_objects;
using Core.physics.material;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.visual
{

    public class sprite : sprite_square {

        public sprite(Vector2 position, Vector2 size, Vector2 scale, Single rotation, Single mass, Vector2 velocity, physics_material physics_material, mobility mobility) 
            : base(position, size, scale, rotation, mass, velocity, physics_material, mobility) {}


    }
}
