using Core.game_objects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.visual {

    public class sprite : sprite_square {

        public sprite(mobility mobility, Vector2 position, Vector2 size, Vector2 scale, Single rotation) 
            : base(mobility, position, size, scale, rotation) {}


    }
}
