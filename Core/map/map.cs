using Core.game_objects;
using Core.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Core {

    public class map {
    
        private sprite_square _core_sprite = new sprite_square(mobility.STATIC, Vector2.Zero, new Vector2(100), Vector2.One, 0);

        private List<Vector2i> _positions = new List<Vector2i>{

            new Vector2i(0, 0),
            new Vector2i(1, 0),
            new Vector2i(2, 0),
            new Vector2i(-2, 0),
            new Vector2i(-2, 1),
        };

        public void render() {

            for (int x = 0; x < _positions.Count; x++) {
                
            }

        }

    }
}
