using Core.game_objects;
using Core.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Core.renderer;

namespace Core {

    public class map {
    
        public map() {

            init();
        }

        public map(shader shader) {
         
            _shader = shader;
            init();
        }

        public map(shader shader, List<tile_translation> positions) {

            _shader = shader;
            _positions = positions;
            init();
        }

        private sprite_square _core_sprite = new sprite_square(mobility.STATIC, Vector2.Zero, new Vector2(100), new Vector2(50, 50), 0);
        private shader _shader;

        public struct tile_translation {
            
            public Vector2 pos;
            public float rotation;

            public tile_translation(Vector2 pos, float rotation) {

                this.pos = pos;
                this.rotation = rotation;
            }
        }

        public void draw() {

            //_core_sprite.draw(_shader);

            for(int x = 0; x < _positions.Count; x++) {

                _core_sprite.set_position(_positions[x].pos * 100);
                _core_sprite.set_rotation(_positions[x].rotation);
                _core_sprite.draw(_shader);
            }

        }

        // ========================================== private ==========================================

        private void init() {

            _core_sprite.add_texture("textures/floor_000.png");

            Random random = new Random();
            double missing_time_rate = 0.2f;
            int width = 5;
            int height = 3;

            // Loop through the tiles and add them to the _positions list
            for(int x = -width; x < width +1; x++) {
                for(int y = -height; y < height +1; y++) {
                    
                    if(random.NextDouble() < missing_time_rate)    // Skip adding tiles at certain positions (e.g., missing tiles)
                        continue; // Skip adding this tile

                    float rotation = (float)utility.degree_to_radians(_rotations[random.Next(0,3)]);
                    _positions.Add(new tile_translation(new Vector2(x, y), rotation)); // Adjust rotation as needed
                }
            }
        }

        private List<tile_translation> _positions = new List<tile_translation>{};
        private readonly float[] _rotations = { 0, 90, 180 ,270 };

    }
}
