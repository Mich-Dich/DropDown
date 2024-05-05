using Core.game_objects;
using Core.renderer;
using Core.util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Core.visual
{

    public class sprite : I_animatable, IDisposable {

        public transform transform { get; set; } = new();
        public shader? shader { get; set; }
        public Texture texture { get; set; }

        // ------------------------------ animation ------------------------------
        public animation? animation { get; set; }
        public float animation_timer { get; set; } = 0;

        // =============================================== constructors =============================================== 

        public sprite(shader shader) { this.shader = shader; init(); }

        public sprite(transform transform, Texture texture) {
            this.transform = transform;
            this.texture = texture;
            init();
        }

        public sprite(Texture texture) {
            this.texture = texture;
            init();
        }

        public sprite(animation animation) {
            this.animation = animation;
            init();
        }

        public sprite(Vector2? position = null, Vector2? size = null, Single rotation = 0.0f, mobility mobility = mobility.DYNAMIC) {

            this.transform.position = position ?? new Vector2(0, 0);
            this.transform.size = size ?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            init();
        }

        public void Dispose() {

            throw new NotImplementedException();
        }

        // =============================================== setters/getters =============================================== 

        public sprite add_animation(animation animation) {

            this.animation = animation;
            return this;
        }

        public sprite add_animation(string path_to_directory, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new animation(this, new SpriteBatch(path_to_directory, is_pixel_art), fps, loop);
            if(start_playing)
                this.animation.play();
            
            return this;
        }

        public sprite add_animation(string path_to_texture_atlas, int num_of_rows, int num_of_columns, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new animation(this, resource_manager.get_texture(path_to_texture_atlas, is_pixel_art), num_of_rows, num_of_columns, fps, loop);
            if(start_playing)
                this.animation.play();
            
            return this;
        }

        public void set_mobility(mobility mobility) {

            this.transform.mobility = mobility;
            if(this.transform.mobility == mobility.STATIC)
                _model_matrix = calc_modle_matrix();
        }

        // =============================================== functions =============================================== 

        public void draw(Matrix4? model = null) {

            if(this.shader == null || (this.texture == null && this.animation == null))
                throw new NotImplementedException("Neither a texture nor an animation is assigned to the sprite. The sprite cannot be rendered.");

            // -------------------------------------- select display mode -------------------------------------- 
            
            if(game.instance.show_debug) 
                game.instance.debug_data.sprite_draw_calls_num++;
            

            if (this.animation != null)
                update_animation();

            this.texture.Use(TextureUnit.Texture0);


            // -------------------------------------- bind data for draw -------------------------------------- 
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader.use();
            _vertex_array.bind();
            _index_buffer.bind();

            // -------------------------------------- modle matrix -------------------------------------- 
            if(model != null)
                this.shader.set_matrix_4x4("model", model.Value);

            // else use precalculated matrix
            else if(this.transform.mobility == mobility.STATIC)
                this.shader.set_matrix_4x4("model", _model_matrix);

            // recalculate matrix every frame
            else if(this.transform.mobility == mobility.DYNAMIC || needs_update) {

                this.shader.set_matrix_4x4("model", calc_modle_matrix());
                needs_update = false;
            }

            // -------------------------------------- draw call -------------------------------------- 
            GL.DrawElements(PrimitiveType.Triangles, _indeices.Length, DrawElementsType.UnsignedInt, 0);
        }

        // ======================================== animation ======================================== 

        public void update_animation() {
            this.animation.update();
        }

        public sprite select_texture_region(int number_of_columns = 1, int number_of_rows = 1, int column_index = 0, int row_index = 0) {

            float offset_y = 1.0f / ((float)number_of_rows * 50);
            float offset_x = 1.0f / ((float)number_of_columns * 50);

            // bottom - right
            _verticies[3] = ((float)row_index / (float)number_of_rows) + offset_y;
            _verticies[2] = ((float)column_index / (float)number_of_columns) + (1.0f / (float)number_of_columns) - offset_x;

            // top - right
            _verticies[7] = ((float)row_index / (float)number_of_rows) + (1.0f / (float)number_of_rows) - offset_y;
            _verticies[6] = ((float)column_index / (float)number_of_columns) + (1.0f / (float)number_of_columns) - offset_x;

            // top - left
            _verticies[11] = ((float)row_index / (float)number_of_rows) + (1.0f / (float)number_of_rows) - offset_y;
            _verticies[10] = ((float)column_index / (float)number_of_columns) + offset_x;

            // bottom - left
            _verticies[15] = ((float)row_index / (float)number_of_rows) + offset_y;
            _verticies[14] = ((float)column_index / (float)number_of_columns) + offset_x;


            _vertex_buffer.update_content(_verticies);
            _vertex_array.add_buffer(_vertex_buffer, this.get_buffer_layout());

            return this;
        }

       public sprite select_texture_regionNew(int number_of_columns, int number_of_rows, int column_index, int row_index, int tileID, int textureWidth, int textureHeight) {

            float offset_y = 1.0f / ((float)number_of_rows * 50);
            float offset_x = 1.0f / ((float)number_of_columns * 50);

            float uvWidth = 1f / number_of_columns;
            float uvHeight = 1f / number_of_rows;

            float u = column_index * uvWidth;
            float v = (number_of_rows - row_index - 1) * uvHeight;

            // Bottom-left
            _verticies[14] = u + offset_x;
            _verticies[15] = v + offset_y;
            // Bottom-right
            _verticies[2] = u + uvWidth - offset_x;
            _verticies[3] = v + offset_y;
            // Top-right
            _verticies[6] = u + uvWidth - offset_x;
            _verticies[7] = v + uvHeight - offset_y;
            // Top-left
            _verticies[10] = u + offset_x;
            _verticies[11] = v + uvHeight - offset_y;

            _vertex_buffer.update_content(_verticies);
            _vertex_array.add_buffer(_vertex_buffer, this.get_buffer_layout());

            int pixelX = (int)(u * textureWidth);
            int pixelY = textureHeight - (int)((v + uvHeight) * textureHeight);
            int pixelWidth = (int)(uvWidth * textureWidth);
            int pixelHeight = (int)(uvHeight * textureHeight);

            Console.WriteLine($"Tile ID: {tileID}, Pixel Position - x: {pixelX}, y: {pixelY}, Size - width: {pixelWidth}, height: {pixelHeight}");

            return this;
        }

        // ============================================ private  ============================================ 
        private index_buffer    _index_buffer;
        private vertex_buffer   _vertex_buffer;
        private vertex_array    _vertex_array;
        private Matrix4         _model_matrix;
        private bool needs_update { get; set; } = true;

        private float[] _verticies { get; set; } = {
        //   x      y      UV.y  UV.x
             0.5f,  0.5f,  1f,   0f,
             0.5f, -0.5f,  1f,   1f,
            -0.5f, -0.5f,  0f,   1f,
            -0.5f,  0.5f,  0f,   0f,
        };
        private float[] _cooord_data = {
        //   x      y
             0.5f,  0.5f,
             0.5f, -0.5f,
            -0.5f, -0.5f,
            -0.5f,  0.5f,
        };
        private float[] _UV_data = {
            1f, 0f,
            1f, 1f,
            0f, 1f,
            0f, 0f,
        };

        private uint[] _indeices = {
            0, 1, 3,
            1, 2 ,3
        };

        public sprite init() {

            if(this.shader == null)
                this.shader = game.instance.default_sprite_shader;

            _vertex_buffer = new vertex_buffer(_verticies);
            _vertex_buffer.bind();
            _vertex_array = new();
            _vertex_array.add_buffer(_vertex_buffer, this.get_buffer_layout());
            _index_buffer = new index_buffer(_indeices);

            if(this.transform.mobility == mobility.STATIC)
                _model_matrix = calc_modle_matrix();

            if(texture == null) {
                this.texture = resource_manager.get_texture("assets/defaults/default_grid.png");
            }

            return this;
        }

        private buffer_layout get_buffer_layout() {

            buffer_layout layout = new buffer_layout()
                .add<float>(2)      // vertex coordinates
                .add<float>(2);     // UV coordinates

            return layout;
        }

        private Matrix4 calc_modle_matrix() {

            Matrix4 trans = Matrix4.CreateTranslation(this.transform.position.X, this.transform.position.Y, 0);
            Matrix4 sca = Matrix4.CreateScale(this.transform.size.X, this.transform.size.Y, 0);
            Matrix4 rot = Matrix4.CreateRotationZ(this.transform.rotation);
            return sca * rot * trans;
        }

    }
}
