
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
        public float animationTimer { get; set; } = 0;

        // =============================================== constructors =============================================== 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.    => fields are set in Init()
        public Sprite(Shader shader) { this.shader = shader; Init(); }

        public Sprite(Transform transform, Texture texture) {
            this.transform = transform;
            this.texture = texture;
            Init();
        }

        public Sprite(Texture texture) {
            this.texture = texture;
            Init();
        }

        public Sprite(Animation animation) {
            this.animation = animation;
            Init();
        }

        public Sprite(Vector2? position = null, Vector2? size = null, Single rotation = 0.0f, Mobility mobility = Mobility.DYNAMIC) {

            this.transform.position = position ?? new Vector2(0, 0);
            this.transform.size = size ?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            Init();
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // =============================================== setters/getters =============================================== 

        /// <summary>
        /// Adds an existing animation to this sprite.
        /// </summary>
        /// <param name="animation">The animation object to add.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite Add_Animation(Animation animation) {

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
        public Sprite Add_Animation(string path_to_directory, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new Animation(this, new SpriteBatch(path_to_directory, is_pixel_art), fps, loop);
            if(start_playing)
                this.animation.Play();

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
        public Sprite Add_Animation(string path_to_texture_atlas, int num_of_rows, int num_of_columns, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new Animation(this, Resource_Manager.Get_Texture(path_to_texture_atlas, is_pixel_art), num_of_rows, num_of_columns, fps, loop);
            if(start_playing)
                this.animation.Play();

            return this;
        }

        /// <summary>
        /// Sets the mobility (static, movable or dynamic) of this sprite's transformation.
        /// </summary>
        /// <param name="mobility">The mobility mode to set.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public void Set_Mobility(Mobility mobility) {

            this.transform.mobility = mobility;
            if(this.transform.mobility == Mobility.STATIC)
                modelMatrix = Calc_Modle_Matrix();
        }

        // =============================================== functions =============================================== 

        // ======================================== animation ======================================== 

        /// <summary>
        /// Selects a specific region of a texture for rendering on the sprite.
        /// </summary>
        /// <param name="numberOfColumns">Number of columns in the texture atlas.</param>
        /// <param name="numberOfRows">Number of rows in the texture atlas.</param>
        /// <param name="columnIndex">Index of the column in the texture atlas.</param>
        /// <param name="rowIndex">Index of the row in the texture atlas.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite Select_Texture_Region(int numberOfColumns = 1, int numberOfRows = 1, int columnIndex = 0, int rowIndex = 0) {

            float offset_y = 1.0f / ((float)numberOfRows * 50);
            float offset_x = 1.0f / ((float)numberOfColumns * 50);

            // bottom - right
            _verticies[3] = ((float)rowIndex / (float)numberOfRows) + offset_y;
            _verticies[2] = ((float)columnIndex / (float)numberOfColumns) + (1.0f / (float)numberOfColumns) - offset_x;

            // top - right
            _verticies[7] = ((float)rowIndex / (float)numberOfRows) + (1.0f / (float)numberOfRows) - offset_y;
            _verticies[6] = ((float)columnIndex / (float)numberOfColumns) + (1.0f / (float)numberOfColumns) - offset_x;

            // top - left
            _verticies[11] = ((float)rowIndex / (float)numberOfRows) + (1.0f / (float)numberOfRows) - offset_y;
            _verticies[10] = ((float)columnIndex / (float)numberOfColumns) + offset_x;

            // bottom - left
            _verticies[15] = ((float)rowIndex / (float)numberOfRows) + offset_y;
            _verticies[14] = ((float)columnIndex / (float)numberOfColumns) + offset_x;


            vertexBuffer.Update_content(_verticies);
            vertexArray.Add_Buffer(vertexBuffer, this.Get_Buffer_Layout());

            return this;
        }

        /// <summary>
        /// Selects a specific region of a texture using a more accurate method.
        /// </summary>
        /// <param name="numberOfColumns">Number of columns in the texture atlas.</param>
        /// <param name="numberOfRows">Number of rows in the texture atlas.</param>
        /// <param name="columnIndex">Index of the column in the texture atlas.</param>
        /// <param name="rowIndex">Index of the row in the texture atlas.</param>
        /// <param name="tileID">ID of the tile.</param>
        /// <param name="textureWidth">Width of the texture atlas.</param>
        /// <param name="textureHeight">Height of the texture atlas.</param>
        /// <returns>The current sprite instance for method chaining.</returns>
        public Sprite Select_Texture_RegionNew(int numberOfColumns, int numberOfRows, int columnIndex, int rowIndex, int tileID, int textureWidth, int textureHeight) {

            float offset_y = 1.0f / ((float)numberOfRows * 50);
            float offset_x = 1.0f / ((float)numberOfColumns * 50);

            float uvWidth = 1f / numberOfColumns;
            float uvHeight = 1f / numberOfRows;

            float u = columnIndex * uvWidth;
            float v = (numberOfRows - rowIndex - 1) * uvHeight;

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

            vertexBuffer.Update_content(_verticies);
            vertexArray.Add_Buffer(vertexBuffer, this.Get_Buffer_Layout());

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
                Debug_Data.spriteDrawCallsNum++;


            if(this.animation != null)
                animation?.Update();

            this.texture?.Use(TextureUnit.Texture0);


            // -------------------------------------- bind data for Draw -------------------------------------- 
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader.Use();
            vertexArray.Bind();
            indexBuffer.Bind();

            // -------------------------------------- modle matrix -------------------------------------- 
            if(model != null)
                this.shader.Set_Matrix_4x4("model", model.Value);

            // else Use precalculated matrix
            else if(this.transform.mobility == Mobility.STATIC)
                this.shader.Set_Matrix_4x4("model", modelMatrix);

            // recalculate matrix every frame
            else if(this.transform.mobility == Mobility.DYNAMIC || needsUpdate) {

                this.shader.Set_Matrix_4x4("model", Calc_Modle_Matrix());
                needsUpdate = false;
            }

            // -------------------------------------- Draw call -------------------------------------- 
            GL.DrawElements(PrimitiveType.Triangles, _indeices.Length, DrawElementsType.UnsignedInt, 0);

        }


        // ============================================ private  ============================================ 
        private Index_Buffer    indexBuffer;
        private Vertex_Buffer   vertexBuffer;
        private Vertex_Array    vertexArray;
        private Matrix4         modelMatrix;
        private bool needsUpdate { get; set; } = true;

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

        public Sprite Init() {

            if(this.shader == null)
                this.shader = Game.instance.default_sprite_shader;

            indexBuffer = new Index_Buffer(_indeices);
            vertexBuffer = new Vertex_Buffer(_verticies);
            vertexBuffer.Bind();
            vertexArray = new();
            vertexArray.Add_Buffer(vertexBuffer, this.Get_Buffer_Layout());

            if(this.transform.mobility == Mobility.STATIC)
                modelMatrix = Calc_Modle_Matrix();

            if(texture == null) 
                this.texture = Resource_Manager.Get_Texture("assets/defaults/default_grid.png");
            
            return this;
        }

        private Buffer_Layout Get_Buffer_Layout() {

            Buffer_Layout layout = new Buffer_Layout()
                .add<float>(2)      // vertex coordinates
                .add<float>(2);     // UV coordinates

            return layout;
        }

        private Matrix4 Calc_Modle_Matrix() {

            Matrix4 trans = Matrix4.CreateTranslation(this.transform.position.X, this.transform.position.Y, 0);
            Matrix4 sca = Matrix4.CreateScale(this.transform.size.X, this.transform.size.Y, 0);
            Matrix4 rot = Matrix4.CreateRotationZ(this.transform.rotation);
            return sca * rot * trans;
        }

    }
}
