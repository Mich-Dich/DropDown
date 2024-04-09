using Core.game_objects;
using Core.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Core.renderer;
using Core.manager;

namespace Core {

    public class map {
    
        public map() {

            init();
        }

        public map(shader shader) {
         
            _shader = shader;
            init();
        }

        public map(shader shader, List<tile_data> positions) {

            _shader = shader;
            _tile_data = positions;
            init();
        }

        public Vector2 tile_size { get; set; } = new Vector2(100, 100);

        public struct tile_data {

            public Vector2 pos;
            public float rotation;
            public int texture_slot { get; set; }

            public tile_data(Vector2 pos, float rotation, int texture_slot) {

                this.pos = pos;
                this.rotation = rotation;
                this.texture_slot = texture_slot;
            }
        }

        public void draw() {

            for(int x = 0; x < _tile_data.Count; x++) 
                _core_sprite.draw(_shader, _tile_modle_matrix[x], _tile_data[x].texture_slot);
        }

        public map generate_square(int width, int height) {

            Random random = new Random();
            double missing_time_rate = 0f;

            float offset_x = (((float)width - 1) / 2) * tile_size.X;
            float offset_y = (((float)height - 1) / 2) * tile_size.Y;

            // Loop through the tiles and add them to the _positions list
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {

                    if(random.NextDouble() < missing_time_rate)    // Skip adding tiles at certain positions (e.g., missing tiles)
                        continue;

                    float rotation = (float)utility.degree_to_radians(_rotations[random.Next(0,3)]);
                    _tile_data.Add(new tile_data(new Vector2(x, y), rotation, random.Next(0, 5)));
                    
                    Matrix4 trans = Matrix4.CreateTranslation(x * tile_size.X - offset_x, y * tile_size.Y - offset_y, 0);
                    Matrix4 sca = Matrix4.CreateScale(tile_size.X / 2, tile_size.Y / 2, 0);
                    Matrix4 rot = Matrix4.CreateRotationZ(rotation);
                    _tile_modle_matrix.Add(rot * sca * trans);
                }
            }

            return this;
        }

        // ========================================== private ==========================================

        private sprite_square _core_sprite = new sprite_square(mobility.STATIC, Vector2.Zero, new Vector2(100), new Vector2(50, 50), 0);
        private shader _shader;
        private List<tile_data> _tile_data = new List<tile_data>{};
        private List<Matrix4> _tile_modle_matrix = new List<Matrix4>();
        private readonly float[] _rotations = { 0, 90, 180 ,270 };

        private void init() {


            resource_manager.instance.load_texture("textures/floor_tile_00.png");
            resource_manager.instance.load_texture("textures/floor_tile_01.png");
            resource_manager.instance.load_texture("textures/floor_tile_02.png");
            resource_manager.instance.load_texture("textures/floor_tile_03.png");
            resource_manager.instance.load_texture("textures/floor_tile_04.png");
        }

    }
}
