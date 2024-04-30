using Core.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Core.renderer;
using Core.manager;
using Core.game_objects;
using Core.physics.material;

namespace Core {

    public class map {

        public List<game_object> all_game_objects {  get; set; } = new List<game_object>();

        public map() {

            init();
        }

        public map(shader shader) {
         
            _shader = shader;
            init();
        }

        public map(shader shader, List<tile_data> positions ) {

            _shader = shader;
            _tile_data = positions;
            init();
        }

        public Vector2 tile_size { get; set; } = new Vector2(100, 100);

        public struct tile_data {

            public int texture_slot { get; set; }
            public Matrix4 modle_matrix { get; set; }

            public tile_data(Int32 texture_slot, Matrix4 modle_matrix) {

                this.texture_slot = texture_slot;
                this.modle_matrix = modle_matrix;
            }
        }

        public void draw() {

            for(int x = 0; x < _tile_data.Count; x++) 
                _core_sprite.draw(_tile_data[x].modle_matrix, _tile_data[x].texture_slot);

            //parameter_buffer _parameter_buffer = new parameter_buffer();
            //_parameter_buffer.bind();

            // write buffer to save [modle_matrix, texture_slot]

            //_core_sprite.draw_instanced(_shader, _tile_data[0].modle_matrix, _tile_data[0].texture_slot, _tile_data.Count);
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
                    Matrix4 trans = Matrix4.CreateTranslation(x * tile_size.X - offset_x, y * tile_size.Y - offset_y, 0);
                    Matrix4 sca = Matrix4.CreateScale(tile_size.X / 2, tile_size.Y / 2, 0);
                    Matrix4 rot = Matrix4.CreateRotationZ(rotation);

                    _tile_data.Add(new tile_data(random.Next(5), rot * sca * trans));
                }
            }

            var test = _tile_data[random.Next(width)];
            test.texture_slot = 4;

            return this;
        }

        // ========================================== private ==========================================

        private sprite _core_sprite = new sprite(Vector2.Zero, new Vector2(100), 0, mobility.STATIC);
        private shader _shader;
        private List<tile_data> _tile_data = new List<tile_data>{};
        private List<Matrix4> _tile_modle_matrix = new List<Matrix4>();
        private readonly float[] _rotations = { 0, 90, 180 ,270 };

        //private List<float> 

        private void init() {

            resource_manager.instance.load_texture("textures/floor_tile_00.png");
            resource_manager.instance.load_texture("textures/floor_tile_01.png");
            resource_manager.instance.load_texture("textures/floor_tile_02.png");
            resource_manager.instance.load_texture("textures/floor_tile_03.png");
            resource_manager.instance.load_texture("textures/floor_tile_04.png");
        }

    }
}
