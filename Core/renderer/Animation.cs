using Core.util;

namespace Core {

    public class animation {

        public bool         Loop { get; set; }
        public SpriteBatch? SpriteBatch { get; set; }


        public animation(IAnimatable animatable, int fps, bool loop = true) {

            this.animatable = animatable;
            this.frameTime = 1.0f / fps;
            this.Loop = loop;
        }

        public void Update() {
            
            if(!this.isPlaying)
                return;
            
            this.timer += game_time.elapsed;
            if(this.timer < this.frameTime)
                return;
                            
            this.timer -= this.frameTime;
            this.currentFrameIndex++;
            if(this.currentFrameIndex >= this.SpriteBatch.FrameCount) {
                if(this.Loop) {
                    this.currentFrameIndex = 0;
                }
                else {
                    this.currentFrameIndex = this.SpriteBatch.FrameCount - 1;
                    this.Stop();
                }
            }

            this.animatable.CurrentFrameIndex = this.currentFrameIndex;
        }

        public Texture GetCurrentFrame() {
            
            Texture currentFrame = this.SpriteBatch.GetFrame(this.animatable.CurrentFrameIndex);
            return currentFrame;
        }

        public void Play() {
            
            this.isPlaying = true;
            this.currentFrameIndex = 0;
        }

        public void Continue() { this.isPlaying = true;}

        public void Stop() { this.isPlaying = false;}

        public void SetSpeed(int fps) { this.frameTime = 1.0f / fps; }

        // ======================================= private ======================================= 

        private IAnimatable animatable;
        private int         currentFrameIndex = 0;
        private float       frameTime;
        private float       timer = 0;
        private bool        isPlaying = false;
    }

}
