
namespace Core.render {

    using Core.util;

    public sealed class Animation {

        public bool Loop;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.        => fields are set in init()
        public Animation(Sprite sprite, SpriteBatch sprite_batch, int fps = 30, bool loop = true) {

            this._sprite_batch = sprite_batch;
            init(sprite, fps, loop);
        }

        public Animation(Sprite sprite, Texture texture_atlas, int num_of_columns, int num_of_rows, int fps = 30, bool loop = true) {

            this._texture_atlas = texture_atlas;
            this._num_of_rows = num_of_rows;
            this._num_of_columns = num_of_columns;
            init(sprite, fps, loop);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void update() {

            if(!_is_playing)
                return;

            if(Game.instance.show_debug)
                debug_data.playing_animation_num++;

            _sprite.animation_timer += Game_Time.delta;
            int _current_frame_index = (int)(_sprite.animation_timer / _frame_time);


            int max_image_index = 0;
            if(_sprite_batch != null)
                max_image_index = _sprite_batch.FrameCount;

            else if(_texture_atlas != null)
                max_image_index = _num_of_columns * _num_of_rows;


            if(_current_frame_index >= max_image_index) {

                if(Loop) {
                    _current_frame_index = 0;
                    _sprite.animation_timer = 0;
                }
                else {
                    _current_frame_index = max_image_index - 1;
                    stop();
                }
            }

            if(_sprite_batch != null)
                this._sprite.texture = _sprite_batch.GetFrame(_current_frame_index);

            else if(_texture_atlas != null)
                this._sprite.select_texture_region(_num_of_columns, _num_of_rows, _current_frame_index % _num_of_columns, _current_frame_index / _num_of_columns);

        }

        public void play() {

            _sprite.animation_timer = 0;
            _is_playing = true;
            //_current_frame_index = 0;
        }

        public void Continue() { _is_playing = true; }

        public void stop() { _is_playing = false; }

        public void set_speed(int fps) { _frame_time = 1.0f / fps; }

        // ======================================= private ======================================= 

        private Sprite          _sprite;
        private float           _frame_time;
        private bool            _is_playing = false;

        private SpriteBatch? _sprite_batch;

        private Texture? _texture_atlas;
        private int _num_of_rows;
        private int _num_of_columns;


        private void init(Sprite sprite, int fps = 30, bool loop = true) {

            this._sprite = sprite;
            _frame_time = 1.0f / fps;
            Loop = loop;

            if(_sprite_batch != null)
                this._sprite.texture = _sprite_batch.GetFrame(0);
            if(_texture_atlas != null)
                this._sprite.texture = _texture_atlas;
        }

    }

}
