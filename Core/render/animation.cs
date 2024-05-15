
namespace Core.render {

    using Core.util;
    using Core.world;

    public sealed class Animation {

        public bool Loop;

        public Animation(Sprite sprite, SpriteBatch sprite_batch, int fps = 30, bool loop = true) {

            this.spriteBatch = sprite_batch;
            this.Init(sprite, fps, loop);
        }

        public Animation(Sprite sprite, Texture texture_atlas, int num_of_columns, int num_of_rows, int fps = 30, bool loop = true) {

            this.textureAtlas = texture_atlas;
            this.numOfRows = num_of_rows;
            this.numOfColumns = num_of_columns;
            this.Init(sprite, fps, loop);
        }

        public void Update() {

            if(!this.isPlaying) 
                return;

            if(Game.Instance.show_performance) 
                DebugData.playingAnimationNum++;

            this.sprite.animationTimer += Game_Time.delta;
            int current_frame_index = (int)(this.sprite.animationTimer / this.frameTime);

            // call notification
            foreach (var notify in m_animation_notificationList) {

                if(previous_frame_index < notify.frame_index
                    && current_frame_index >= notify.frame_index)
                    notify.action();
            }

            int max_image_index = 0;
            if(this.spriteBatch != null) 
                max_image_index = this.spriteBatch.frameCount;
            else if(this.textureAtlas != null) 
                max_image_index = this.numOfColumns * this.numOfRows;

            if(current_frame_index >= max_image_index) {

                if(this.Loop) {
                    
                    current_frame_index = 0;
                    this.sprite.animationTimer = 0;
                } else {
                  
                    current_frame_index = max_image_index - 1;
                    this.Stop();
                }
            }

            if(this.spriteBatch != null) 
                this.sprite.texture = this.spriteBatch.GetFrame(current_frame_index);
            else if(this.textureAtlas != null) 
                this.sprite.Select_Texture_Region(this.numOfColumns, this.numOfRows, current_frame_index % this.numOfColumns, current_frame_index / this.numOfColumns);

            previous_frame_index = current_frame_index;
        }

        public void Play() {

            this.sprite.animationTimer = 0;
            this.isPlaying = true;
        }

        public Animation add_animation_notification(animation_notification notification) {

            m_animation_notificationList.Add(notification);
            return this;
        }

        public Animation add_animation_notification(int frame_index, Action action) {

            m_animation_notificationList.Add(new animation_notification(frame_index, action));
            return this;
        }

        public void Continue() { this.isPlaying = true; }

        public void Stop() { this.isPlaying = false; }

        public void set_speed(int fps) { this.frameTime = 1.0f / fps; }

        // ======================================= private =======================================

        private List<animation_notification> m_animation_notificationList = new List<animation_notification>();
        private int previous_frame_index = 0;
        private readonly SpriteBatch? spriteBatch;
        private readonly Texture? textureAtlas;
        private readonly int numOfRows;
        private readonly int numOfColumns;
        private Sprite sprite;
        private float frameTime;
        private bool isPlaying = false;

        private void Init(Sprite sprite, int fps = 30, bool loop = true) {

            this.sprite = sprite;
            this.frameTime = 1.0f / fps;
            this.Loop = loop;

            if(this.spriteBatch != null) 
                this.sprite.texture = this.spriteBatch.GetFrame(0);

            if(this.textureAtlas != null) 
                this.sprite.texture = this.textureAtlas;
        }
    }

    public struct animation_notification {

        public int frame_index;
        public Action action;

        public animation_notification(Int32 frame_index, Action action) {

            this.frame_index = frame_index;
            this.action = action;
        }
    }


}
