﻿using Core.game_objects;
using Core.manager;
using Core.renderer;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Core.visual {

    public class sprite : IAnimatable {

        public transform        transform { get; set; } = new();
        public shader?           shader { get; set; }
        public Texture?         texture { get; set; }
        public SpriteBatch?     spriteBatch { get; set; }

        public sprite(shader shader) { this.shader = shader; }

        public sprite(transform transform, Texture texture) {

            this.transform = transform;
            this.texture = texture;
            init();
        }

        public sprite(transform transform, SpriteBatch SpriteBatch) {

            this.transform = transform;
            this.SpriteBatch = SpriteBatch;
            init();
        }

        public sprite(Vector2? position = null, Vector2? size = null, Single rotation = 0.0f, mobility mobility = mobility.DYNAMIC) {

            this.transform.position = position ?? new Vector2(0 ,0);
            this.transform.size = size ?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            init();
        }

        // =============================================== functions =============================================== 

        //public void add_texture(string file_path) {

        //    resource_manager.instance.load_texture(file_path);
        //}

        //public void add_texture(List<string> file_paths) {

        //    foreach (string file_path in file_paths)
        //        resource_manager.instance.load_texture(file_path);
        //}

        //// set translation in world
        //public void set_position(Vector2 position) { 
            
        //    this.transform.position = position;
        //    needs_update = true;
        //}
        
        //public void set_size(Vector2 scale) { 

        //    this.transform.size = scale;
        //    needs_update = true;
        //}
        
        //public void set_rotation(float rotation) {
            
        //    this.transform.rotation = rotation;
        //    needs_update = true;
        //}

        //public void set_translation(Vector2 position, Vector2 scale, float rotation) {
        
        //    this.transform.position = position;
        //    this.transform.size= scale;
        //    this.transform.rotation = rotation;
        //    needs_update = true;
        //}

        public void set_mobility(mobility mobility) {

            this.transform.mobility = mobility;
            if(this.transform.mobility == mobility.STATIC)
                _model_matrix = calc_modle_matrix();

        }

        public void draw() {
            
            setup_draw_call();

            // recalculate matrix every frame
            if(this.transform.mobility == mobility.DYNAMIC || needs_update) {

                this.shader.set_matrix_4x4("model", calc_modle_matrix());
                needs_update = false;
            }

            // else use precalculated matrix
            else if (this.transform.mobility == mobility.STATIC)
                this.shader.set_matrix_4x4("model", _model_matrix);

            draw_call();
        }

        public void draw(Matrix4 model) {
            
            setup_draw_call();
            this.shader.set_matrix_4x4("model", model);
            draw_call();
        }

        public buffer_layout get_buffer_layout() {

            buffer_layout layout = new buffer_layout()
                .add<float>(2)      // vertex coordinates
                .add<float>(2)      // UV coordinates
                .add<float>(1);     // texture_slot

            return layout;
        }

        // ------------------------------ animation ------------------------------
        public SpriteBatch? SpriteBatch { get; set; }
        public Animation? Animation { get; set; }
        public Int32 CurrentFrameIndex { get; set; }

        public void update_animation() {
            throw new NotImplementedException();
        }

        // ============================================ private  ============================================ 
        private index_buffer _index_buffer { get; set; }
        private vertex_buffer _vertex_buffer { get; set; }
        private vertex_array _vertex_array { get; set; }
        private Matrix4 _model_matrix;
        private bool needs_update { get; set; } = true;

        private float[] _verticies = {
        //   x    y    UV.y  UV.x
             1f,  1f,  1f,   1f,  1f,
             1f, -1f,  1f,   0f,  1f,
            -1f, -1f,  0f,   0f,  1f,
            -1f,  1f,  0f,   1f,  1f,
        };
        private float[] _cooord_data = {
        //   x    y
             1f,  1f,
             1f, -1f,
            -1f, -1f,
            -1f,  1f,
        };
        private float[] _UV_data = {
            1f, 1f,
            1f, 0f,
            0f, 0f,
            0f, 1f,
        };
        private int _texture_index = 0;

        private uint[] _indeices = {
            0, 1, 3,
            1, 2 ,3
        };

        private void update_data() {

        }

        private void setup_draw_call() {

            if(this.shader == null || (this.texture == null && this.SpriteBatch == null))
                throw new ResourceNotAssignedException("Shader or Texture not assigned");

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader.use();
            _vertex_array.bind();
            _index_buffer.bind();

            // display correct format
            if(this.texture != null)
                this.texture.Use(TextureUnit.Texture0);

            else if(this.Animation != null) {

                Texture frame = this.Animation.GetCurrentFrame();
                frame.Use(TextureUnit.Texture0);
            }
            else if(this.SpriteBatch != null) {

                Texture frame = this.SpriteBatch.GetFrame(this.CurrentFrameIndex);
                frame.Use(TextureUnit.Texture0);
            }
        }

        private void draw_call() {

            GL.DrawElements(PrimitiveType.Triangles, _indeices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private void init() {

            if (this.shader == null)
                this.shader = game.instance.default_sprite_shader;

            _vertex_buffer = new vertex_buffer(_verticies);
            _vertex_buffer.bind();
            _vertex_array = new();
            _vertex_array.add_buffer(_vertex_buffer, this.get_buffer_layout());
            _index_buffer = new index_buffer(_indeices);

            if(this.transform.mobility == mobility.STATIC)
                _model_matrix = calc_modle_matrix();
        }

        private Matrix4 calc_modle_matrix() {

            Matrix4 trans = Matrix4.CreateTranslation(this.transform.position.X, this.transform.position.Y, 0);
            Matrix4 sca = Matrix4.CreateScale(this.transform.size.X, this.transform.size.Y, 0);
            Matrix4 rot = Matrix4.CreateRotationZ(this.transform.rotation);
            return sca * rot * trans;
        }

    }
}
