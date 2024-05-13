namespace Core.render
{
    using Core.util;

    public sealed class Animation
    {
        public bool Loop;
        private readonly SpriteBatch? spriteBatch;
        private readonly Texture? textureAtlas;
        private readonly int numOfRows;
        private readonly int numOfColumns;
        private Sprite sprite;
        private float frameTime;
        private bool isPlaying = false;

        public Animation(Sprite sprite, SpriteBatch sprite_batch, int fps = 30, bool loop = true)
        {
            this.spriteBatch = sprite_batch;
            this.Init(sprite, fps, loop);
        }

        public Animation(Sprite sprite, Texture texture_atlas, int num_of_columns, int num_of_rows, int fps = 30, bool loop = true)
        {
            this.textureAtlas = texture_atlas;
            this.numOfRows = num_of_rows;
            this.numOfColumns = num_of_columns;
            this.Init(sprite, fps, loop);
        }

        public void Update()
        {
            if (!this.isPlaying)
            {
                return;
            }

            if (Game.Instance.showDebug)
            {
                DebugData.playingAnimationNum++;
            }

            this.sprite.animationTimer += Game_Time.delta;
            int current_frame_index = (int)(this.sprite.animationTimer / this.frameTime);

            int max_image_index = 0;
            if (this.spriteBatch != null)
            {
                max_image_index = this.spriteBatch.frameCount;
            }
            else if (this.textureAtlas != null)
            {
                max_image_index = this.numOfColumns * this.numOfRows;
            }

            if (current_frame_index >= max_image_index)
            {
                if (this.Loop)
                {
                    current_frame_index = 0;
                    this.sprite.animationTimer = 0;
                }
                else
                {
                    current_frame_index = max_image_index - 1;
                    this.Stop();
                }
            }

            if (this.spriteBatch != null)
            {
                this.sprite.texture = this.spriteBatch.GetFrame(current_frame_index);
            }
            else if (this.textureAtlas != null)
            {
                this.sprite.Select_Texture_Region(this.numOfColumns, this.numOfRows, current_frame_index % this.numOfColumns, current_frame_index / this.numOfColumns);
            }
        }

        public void Play()
        {
            this.sprite.animationTimer = 0;
            this.isPlaying = true;
        }

        public void Continue()
        {
            this.isPlaying = true;
        }

        public void Stop()
        {
            this.isPlaying = false;
        }

        public void set_speed(int fps)
        {
            this.frameTime = 1.0f / fps;
        }

        // ======================================= private =======================================
        private void Init(Sprite sprite, int fps = 30, bool loop = true)
        {
            this.sprite = sprite;
            this.frameTime = 1.0f / fps;
            this.Loop = loop;

            if (this.spriteBatch != null)
            {
                this.sprite.texture = this.spriteBatch.GetFrame(0);
            }

            if (this.textureAtlas != null)
            {
                this.sprite.texture = this.textureAtlas;
            }
        }
    }
}
