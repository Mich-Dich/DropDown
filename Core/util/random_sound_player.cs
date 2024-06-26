
namespace Core.util {

    using NAudio.Wave;
    using System;
    using System.Collections.Generic;

    public class random_sound_player : IDisposable {
    
        private List<Sound> _sounds;
        private Random _random;
        private Sound _currentSound;

        // ---------------------------------------------------------------------------------------------------------------
        // CONSTRUCTOR / CLEANUP
        // ---------------------------------------------------------------------------------------------------------------

        public random_sound_player(List<Sound> sounds) {

            if(sounds == null || sounds.Count == 0)
                throw new ArgumentException("The sound list cannot be null or empty.");

            _sounds = sounds;
            _random = new Random();
            _currentSound = null;
        }

        public void Dispose() {
            
            if(_currentSound != null) 
                _currentSound.Dispose();

            foreach(var sound in _sounds)
                sound.Dispose();
        }

        // ---------------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ---------------------------------------------------------------------------------------------------------------

        public void play() { play_next_sound(); }

        public bool is_playing() { return _currentSound != null && _currentSound.is_playing(); }

        public void play_next_sound() {

            if(_currentSound != null) {
                _currentSound.OnPlaybackStopped -= current_sound___on_playback_stopped;
                _currentSound.Dispose();
            }

            _currentSound = get_random_sound();
            _currentSound.OnPlaybackStopped += current_sound___on_playback_stopped;
            _currentSound.play();
        }

        public void stop() {

            if(_currentSound != null) {
                _currentSound.stop();
                _currentSound.OnPlaybackStopped -= current_sound___on_playback_stopped;
                _currentSound.Dispose();
            }
        }

        public void pause() {
         
            if(_currentSound != null) 
                _currentSound.pause();
        }

        // ---------------------------------------------------------------------------------------------------------------
        // PRIVATE
        // ---------------------------------------------------------------------------------------------------------------

        private Sound get_random_sound() {

            int index = _random.Next(_sounds.Count);
            return _sounds[index];
        }

        private void current_sound___on_playback_stopped(object sender, StoppedEventArgs e) { play_next_sound(); }

    }
}
