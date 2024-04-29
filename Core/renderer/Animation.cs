
namespace Core.renderer {

    public class Animation {

        private IAnimatable animatable;
        private int currentFrameIndex;
        private float frameTime;
        private float timer;
        private bool isPlaying;
        public bool Loop { get; set; }

        public Animation(IAnimatable animatable, int fps, bool loop = true) {
            this.animatable = animatable;
            this.currentFrameIndex = 0;
            this.frameTime = 1.0f / fps;
            this.timer = 0;
            this.isPlaying = false;
            this.Loop = loop;
        }

        public void Update(float deltaTime) {
            if(!this.isPlaying) {
                return;
            }

            this.timer += deltaTime;
            if(this.timer >= this.frameTime) {
                this.timer -= this.frameTime;
                this.currentFrameIndex++;
                if(this.currentFrameIndex >= this.animatable.SpriteBatch.FrameCount) {
                    if(this.Loop) {
                        this.currentFrameIndex = 0;
                    }
                    else {
                        this.currentFrameIndex = this.animatable.SpriteBatch.FrameCount - 1;
                        this.Stop();
                    }
                }

                this.animatable.CurrentFrameIndex = this.currentFrameIndex;
            }
        }

        public Texture GetCurrentFrame() {
            Texture currentFrame = this.animatable.SpriteBatch.GetFrame(this.animatable.CurrentFrameIndex);
            return currentFrame;
        }

        public void Play() {
            this.isPlaying = true;
            this.currentFrameIndex = 0;
        }

        public void Continue() {
            this.isPlaying = true;
        }

        public void Stop() {
            this.isPlaying = false;
        }

        public void SetSpeed(int fps) {
            this.frameTime = 1.0f / fps;
        }
    }
}