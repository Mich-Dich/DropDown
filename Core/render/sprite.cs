
namespace Core.render {

    using Core.render.shaders;
    using Core.util;
    using Core.world;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public sealed class Sprite : I_animatable {
        
        public Transform transform { get; set; } = new();
        public Shader? shader { get; set; }
        public Texture texture { get; set; }

        // ------------------------------ animation ------------------------------
        public Animation? animation { get; set; }
        public float animation_timer { get; set; } = 0;

        // =============================================== constructors =============================================== 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.    => fields are set in init()
        public Sprite(Shader shader) { this.shader = shader; init(); }

        public Sprite(Transform transform, Texture texture) {
            this.transform = transform;
            this.texture = texture;
            init();
        }

        public Sprite(Texture texture) {
            this.texture = texture;
            init();
        }

        public Sprite(Animation animation) {
            this.animation = animation;
            init();
        }

        public Sprite(Vector2? position = null, Vector2? size = null, Single rotation = 0.0f, Mobility mobility = Mobility.DYNAMIC) {

            this.transform.position = position ?? new Vector2(0, 0);
            this.transform.size = size ?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            init();
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // =============================================== setters/getters =============================================== 

        /// <summary>
        /// Adds an existing animation to this sprite.
        /// </summary>
        /// <param name="animation">The animation object to add.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite add_animation(Animation animation) {

            this.animation = animation;
            return this;
        }

        /// <summary>
        /// Creates and adds a new animation to this sprite from image files in the specified directory.
        /// </summary>
        /// <param name="path_to_directory">The directory containing animation frames.</param>
        /// <param name="start_playing">Whether to start playing the animation immediately.</param>
        /// <param name="is_pixel_art">Specifies if the animation uses pixel art (default: false).</param>
        /// <param name="fps">Frames per second for the animation (default: 30).</param>
        /// <param name="loop">Whether the animation should loop (default: false).</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite add_animation(string path_to_directory, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new Animation(this, new SpriteBatch(path_to_directory, is_pixel_art), fps, loop);
            if(start_playing)
                this.animation.play();

            return this;
        }

        /// <summary>
        /// Creates and adds a new animation to this sprite from a texture atlas.
        /// </summary>
        /// <param name="path_to_texture_atlas">The path to the texture atlas file.</param>
        /// <param name="num_of_rows">Number of rows in the texture atlas.</param>
        /// <param name="num_of_columns">Number of columns in the texture atlas.</param>
        /// <param name="start_playing">Whether to start playing the animation immediately.</param>
        /// <param name="is_pixel_art">Specifies if the animation uses pixel art (default: false).</param>
        /// <param name="fps">Frames per second for the animation (default: 30).</param>
        /// <param name="loop">Whether the animation should loop (default: false).</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite add_animation(string path_to_texture_atlas, int num_of_rows, int num_of_columns, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new Animation(this, Resource_Manager.Get_Texture(path_to_texture_atlas, is_pixel_art), num_of_rows, num_of_columns, fps, loop);
            if(start_playing)
                this.animation.play();

            return this;
        }

        /// <summary>
        /// Sets the mobility (static, movable or dynamic) of this sprite's transformation.
        /// </summary>
        /// <param name="mobility">The mobility mode to set.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public void set_mobility(Mobility mobility) {

            this.transform.mobility = mobility;
            if(this.transform.mobility == Mobility.STATIC)
                _model_matrix = calc_modle_matrix();
        }

        // =============================================== functions =============================================== 

        // ======================================== animation ======================================== 

        /// <summary>
        /// Selects a specific region of a texture for rendering on the sprite.
        /// </summary>
        /// <param name="number_of_columns">Number of columns in the texture atlas.</param>
        /// <param name="number_of_rows">Number of rows in the texture atlas.</param>
        /// <param name="column_index">Index of the column in the texture atlas.</param>
        /// <param name="row_index">Index of the row in the texture atlas.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite select_texture_region(int number_of_columns = 1, int number_of_rows = 1, int column_index = 0, int row_index = 0) {

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
            _vertex_array.Add_Buffer(_vertex_buffer, this.get_buffer_layout());

            return this;
        }

        /// <summary>
        /// Selects a specific region of a texture using a more accurate method.
        /// </summary>
        /// <param name="number_of_columns">Number of columns in the texture atlas.</param>
        /// <param name="number_of_rows">Number of rows in the texture atlas.</param>
        /// <param name="column_index">Index of the column in the texture atlas.</param>
        /// <param name="row_index">Index of the row in the texture atlas.</param>
        /// <param name="tileID">ID of the tile.</param>
        /// <param name="textureWidth">Width of the texture atlas.</param>
        /// <param name="textureHeight">Height of the texture atlas.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite select_texture_regionNew(int number_of_columns, int number_of_rows, int column_index, int row_index, int tileID, int textureWidth, int textureHeight) {

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
            _vertex_array.Add_Buffer(_vertex_buffer, this.get_buffer_layout());

            //int pixelX = (int)(u * textureWidth);
            //int pixelY = textureHeight - (int)((v + uvHeight) * textureHeight);
            //int pixelWidth = (int)(uvWidth * textureWidth);
            //int pixelHeight = (int)(uvHeight * textureHeight);
            //Console.WriteLine($"Tile ID: {tileID}, Pixel Position - x: {pixelX}, y: {pixelY}, Size - width: {pixelWidth}, height: {pixelHeight}");

            return this;
        }

        // ================================================================= internal =================================================================

        /// <summary>
        /// Draws the sprite using the specified model matrix.
        /// </summary>
        /// <param name="model">The model matrix to Use for drawing. If null, the sprite's own matrix is used.</param>
        /// <exception cref="NotImplementedException">Thrown if neither a texture nor an animation is assigned to the sprite.</exception>
        public void Draw(Matrix4? model = null) {

            if(this.shader == null || (this.texture == null && this.animation == null))
                throw new NotImplementedException("Neither a texture nor an animation is assigned to the sprite. The sprite cannot be rendered.");

            // -------------------------------------- select display mode -------------------------------------- 

            if(Game.instance.show_debug)
                debug_data.sprite_draw_calls_num++;


            if(this.animation != null)
                animation?.update();

            this.texture?.Use(TextureUnit.Texture0);


            // -------------------------------------- bind data for Draw -------------------------------------- 
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader.Use();
            _vertex_array.Bind();
            _index_buffer.Bind();

            // -------------------------------------- modle matrix -------------------------------------- 
            if(model != null)
                this.shader.Set_Matrix_4x4("model", model.Value);

            // else Use precalculated matrix
            else if(this.transform.mobility == Mobility.STATIC)
                this.shader.Set_Matrix_4x4("model", _model_matrix);

            // recalculate matrix every frame
            else if(this.transform.mobility == Mobility.DYNAMIC || needs_update) {

                this.shader.Set_Matrix_4x4("model", calc_modle_matrix());
                needs_update = false;
            }

            // -------------------------------------- Draw call -------------------------------------- 
            GL.DrawElements(PrimitiveType.Triangles, _indeices.Length, DrawElementsType.UnsignedInt, 0);

        }


        // ============================================ private  ============================================ 
        private Index_Buffer    _index_buffer;
        private Vertex_Buffer   _vertex_buffer;
        private Vertex_Array    _vertex_array;
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

        public Sprite init() {

            if(this.shader == null)
                this.shader = Game.instance.default_sprite_shader;

            _index_buffer = new Index_Buffer(_indeices);
            _vertex_buffer = new Vertex_Buffer(_verticies);
            _vertex_buffer.Bind();
            _vertex_array = new();
            _vertex_array.Add_Buffer(_vertex_buffer, this.get_buffer_layout());

            if(this.transform.mobility == Mobility.STATIC)
                _model_matrix = calc_modle_matrix();

            if(texture == null) 
                this.texture = Resource_Manager.Get_Texture("assets/defaults/default_grid.png");
            
            return this;
        }

        private Buffer_Layout get_buffer_layout() {

            Buffer_Layout layout = new Buffer_Layout()
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
