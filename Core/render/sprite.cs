
namespace Core.render {

    using Core.render.shaders;
    using Core.util;
    using Core.world;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public sealed class Sprite : I_animatable {
        
        private Index_Buffer indexBuffer;
        private Vertex_Buffer vertexBuffer;
        private Vertex_Array vertexArray;
        private Matrix4 modelMatrix;

        // =============================================== constructors ===============================================

        public Sprite(Shader shader) {
            
            this.shader = shader;
            this.Init();
        }

        public Sprite(Transform transform, Texture texture) {
            
            this.transform = transform;
            this.texture = texture;
            this.Init();
        }

        public Sprite(Texture texture) {
            
            this.texture = texture;
            this.Init();
        }

        public Sprite(Animation animation) {
            
            this.animation = animation;
            this.Init();
        }

        public Sprite(Vector2? position = null, Vector2? size = null, float rotation = 0.0f, Mobility mobility = Mobility.DYNAMIC) {
        
            this.transform.position = position ?? new Vector2(0, 0);
            this.transform.size = size ?? new Vector2(100, 100);
            this.transform.rotation = rotation;
            this.transform.mobility = mobility;
            this.Init();
        }

        public Transform transform { get; set; } = new();
        public Shader? shader { get; set; }
        public Texture texture { get; set; }

        // ------------------------------ animation ------------------------------
        public Animation? animation { get; set; }
        public float animationTimer { get; set; } = 0;

        // ------------------------------ private ------------------------------
        private bool needsUpdate { get; set; } = true;
        private float[] _verticies { get; set; } = {
        //       x      y   UV.y  UV.x
              0.5f,  0.5f,  1f,   0f,
              0.5f, -0.5f,  1f,   1f,
             -0.5f, -0.5f,  0f,   1f,
             -0.5f,  0.5f,  0f,   0f,
        };

        // =============================================== setters/getters ===============================================
        public Sprite Add_Animation(Animation animation) {

            this.animation = animation;
            return this;
        }

        public Sprite set_animation(string path_to_directory, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new Animation(this, new SpriteBatch(path_to_directory, is_pixel_art), fps, loop);
            if(start_playing) 
                this.animation.Play();

            return this;
        }

        public Sprite set_animation(string path_to_texture_atlas, int num_of_rows, int num_of_columns, bool start_playing = false, bool is_pixel_art = false, int fps = 30, bool loop = false) {

            this.animation = new Animation(this, Resource_Manager.Get_Texture(path_to_texture_atlas, is_pixel_art), num_of_rows, num_of_columns, fps, loop);
            if(start_playing) 
                this.animation.Play();

            return this;
        }

        public void Set_Mobility(Mobility mobility) {

            this.transform.mobility = mobility;
            if(this.transform.mobility == Mobility.STATIC) 
                this.modelMatrix = this.Calc_Modle_Matrix();
        }

        // =============================================== functions ===============================================

        // ======================================== animation ========================================
        public Sprite Select_Texture_Region(int numberOfColumns = 1, int numberOfRows = 1, int columnIndex = 0, int rowIndex = 0) {
            
            float offset_y = 1.0f / ((float)numberOfRows * 50);
            float offset_x = 1.0f / ((float)numberOfColumns * 50);

            // bottom - right
            this._verticies[3] = ((float)(numberOfRows - rowIndex - 1) / (float)numberOfRows) + offset_y;
            this._verticies[2] = ((float)columnIndex / (float)numberOfColumns) + (1.0f / (float)numberOfColumns) - offset_x;

            // top - right
            this._verticies[7] = ((float)(numberOfRows - rowIndex - 1) / (float)numberOfRows) + (1.0f / (float)numberOfRows) - offset_y;
            this._verticies[6] = ((float)columnIndex / (float)numberOfColumns) + (1.0f / (float)numberOfColumns) - offset_x;

            // top - left
            this._verticies[11] = ((float)(numberOfRows - rowIndex - 1) / (float)numberOfRows) + (1.0f / (float)numberOfRows) - offset_y;
            this._verticies[10] = ((float)columnIndex / (float)numberOfColumns) + offset_x;

            // bottom - left
            this._verticies[15] = ((float)(numberOfRows - rowIndex - 1) / (float)numberOfRows) + offset_y;
            this._verticies[14] = ((float)columnIndex / (float)numberOfColumns) + offset_x;

            this.vertexBuffer.Update_content(this._verticies);
            this.vertexArray.Add_Buffer(this.vertexBuffer, this.Get_Buffer_Layout());

            return this;
        }

        public Sprite Select_Texture_RegionNew(int numberOfColumns, int numberOfRows, int columnIndex, int rowIndex, int tileID, int textureWidth, int textureHeight) {
            
            float offset_y = 1.0f / ((float)numberOfRows * 50);
            float offset_x = 1.0f / ((float)numberOfColumns * 50);

            float uvWidth = 1f / numberOfColumns;
            float uvHeight = 1f / numberOfRows;

            float u = columnIndex * uvWidth;
            float v = (numberOfRows - rowIndex - 1) * uvHeight;

            // Bottom-left
            this._verticies[14] = u + offset_x;
            this._verticies[15] = v + offset_y;

            // Bottom-right
            this._verticies[2] = u + uvWidth - offset_x;
            this._verticies[3] = v + offset_y;

            // Top-right
            this._verticies[6] = u + uvWidth - offset_x;
            this._verticies[7] = v + uvHeight - offset_y;

            // Top-left
            this._verticies[10] = u + offset_x;
            this._verticies[11] = v + uvHeight - offset_y;

            this.vertexBuffer.Update_content(this._verticies);
            this.vertexArray.Add_Buffer(this.vertexBuffer, this.Get_Buffer_Layout());

            // int pixelX = (int)(u * textureWidth);
            // int pixelY = textureHeight - (int)((v + uvHeight) * textureHeight);
            // int pixelWidth = (int)(uvWidth * textureWidth);
            // int pixelHeight = (int)(uvHeight * textureHeight);
            // Console.WriteLine($"Tile ID: {tileID}, Pixel Position - x: {pixelX}, y: {pixelY}, Size - width: {pixelWidth}, height: {pixelHeight}");
            return this;
        }

        // ================================================================= internal =================================================================
        public void Draw(Matrix4? model = null) {

            if(this.shader == null || (this.texture == null && this.animation == null)) 
                throw new NotImplementedException("Neither a texture nor an animation is assigned to the sprite. The sprite cannot be rendered.");

            // -------------------------------------- select display mode --------------------------------------
            if(Game.Instance.showDebug) 
                DebugData.spriteDrawCallsNum++;

            if(this.animation != null) 
                this.animation?.Update();

            this.texture?.Use(TextureUnit.Texture0);

            // -------------------------------------- bind data for Draw --------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            this.shader.Use();
            this.vertexArray.Bind();
            this.indexBuffer.Bind();

            // -------------------------------------- modle matrix --------------------------------------
            if(model != null) 
                this.shader.Set_Matrix_4x4("model", model.Value);

            // else Use precalculated matrix
            else if(this.transform.mobility == Mobility.STATIC) 
                this.shader.Set_Matrix_4x4("model", this.modelMatrix);

            // recalculate matrix every frame
            else if(this.transform.mobility == Mobility.DYNAMIC || this.needsUpdate) {
                this.shader.Set_Matrix_4x4("model", this.Calc_Modle_Matrix());
                this.needsUpdate = false;
            }

            // -------------------------------------- Draw call --------------------------------------
            GL.DrawElements(PrimitiveType.Triangles, this.indeices.Length, DrawElementsType.UnsignedInt, 0);
        }

        // ============================================ private  ============================================

        private readonly float[] cooordData = {
        // x      y
              0.5f,  0.5f,
              0.5f, -0.5f,
             -0.5f, -0.5f,
             -0.5f,  0.5f,
        };

        private readonly float[] uVData = {
            1f, 0f,
            1f, 1f,
            0f, 1f,
            0f, 0f,
        };

        private readonly uint[] indeices = {
            0, 1, 3,
            1, 2, 3,
        };

        public Sprite Init() {

            if(this.shader == null) 
                this.shader = Game.Instance.defaultSpriteShader;

            this.indexBuffer = new Index_Buffer(this.indeices);
            this.vertexBuffer = new Vertex_Buffer(this._verticies);
            this.vertexBuffer.Bind();
            this.vertexArray = new();
            this.vertexArray.Add_Buffer(this.vertexBuffer, this.Get_Buffer_Layout());

            if(this.transform.mobility == Mobility.STATIC) 
                this.modelMatrix = this.Calc_Modle_Matrix();

            if(this.texture == null) 
                this.texture = Resource_Manager.Get_Texture("assets/defaults/default_grid.png");

            return this;
        }

        private Buffer_Layout Get_Buffer_Layout() {

            Buffer_Layout layout = new Buffer_Layout()
                .add<float>(2) // vertex coordinates
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
