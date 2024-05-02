using Core.renderer;
using Core.util;

namespace Core.visual {

    public class animation {

        public bool Loop;
        public SpriteBatch? SpriteBatch;

        public animation(I_animatable animatable, int fps, bool loop = true) {

            this._animatable = animatable;
            _frame_time = 1.0f / fps;
            Loop = loop;
        }

        public void update() {

            if(!_is_playing)
                return;

            _animatable.animation_timer += game_time.delta;
            _current_frame_index = (int)(_animatable.animation_timer / _frame_time);

            if(_current_frame_index >= SpriteBatch.FrameCount) {
                if(Loop) {
                    _current_frame_index = 0;
                    _animatable.animation_timer = 0;
                }
                else {
                    _current_frame_index = SpriteBatch.FrameCount - 1;
                    stop();
                }
            }
        }

        public Texture get_current_frame() {

            Texture currentFrame = SpriteBatch.GetFrame(_current_frame_index);
            return currentFrame;
        }

        public void play() {

            _animatable.animation_timer = 0;
            _is_playing = true;
            _current_frame_index = 0;
        }

        public void Continue() { _is_playing = true; }

        public void stop() { _is_playing = false; }

        public void set_speed(int fps) { _frame_time = 1.0f / fps; }

        // ======================================= private ======================================= 

        private I_animatable     _animatable;
        private int             _current_frame_index = 0;
        private float           _frame_time;
        private bool            _is_playing = false;
    }

}
