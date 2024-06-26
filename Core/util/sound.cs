
namespace Core.util {

    using NAudio.Wave;
    using System;
    using System.Collections.Generic;
    using System.IO;
    public class Sound : IDisposable {
        public string FilePath { get; private set; }
        public float Volume { get; set; } = 1.0f;
        public bool Loop { get; set; } = false;

        private WaveFormat _waveFormat;
        private byte[] _audioData;
        private List<WaveOutEvent> _playingEvents;
        private List<WaveStream> _waveStreams;

        public event EventHandler<StoppedEventArgs> OnPlaybackStopped;

        // ---------------------------------------------------------------------------------------------------------------
        // CONSTRUCTOR / CLEANUP
        // ---------------------------------------------------------------------------------------------------------------

        public Sound(string filePath, float volume = 1.0f, bool loop = false) {

            FilePath = filePath;
            Volume = volume;
            Loop = loop;
            _playingEvents = new List<WaveOutEvent>();
            _waveStreams = new List<WaveStream>();

            load_audio_data();
        }

        public void Dispose() {

#if false
            var playingEventsCopy = new List<WaveOutEvent>(_playingEvents);
            var waveStreamsCopy = new List<WaveStream>(_waveStreams);

            try {
                foreach(var waveOut in playingEventsCopy) {
                    waveOut.Dispose();
                }

                foreach(var waveStream in waveStreamsCopy) {
                    waveStream.Dispose();
                }
            }
            catch(Exception ex) {

                Console.WriteLine($"Exeption trown in Sound.cs[stop()] => [{ex.ToString()}]");
            }

            _playingEvents.Clear();
            _waveStreams.Clear();
#else
            try {
                foreach(var waveOut in _playingEvents)
                    waveOut.Dispose();

                foreach(var waveStream in _waveStreams)
                    waveStream.Dispose();
                                }
            catch(Exception ex) {

                Console.WriteLine($"Exeption trown in Sound.cs[stop()] => [{ex.ToString()}]");
            }

            _playingEvents.Clear();
            _waveStreams.Clear();
#endif
        }

        // ---------------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ---------------------------------------------------------------------------------------------------------------

        public void play() {

            var memoryStream = new MemoryStream(_audioData);
            var waveStream = new RawSourceWaveStream(memoryStream, _waveFormat);
            var waveOutEvent = new WaveOutEvent
            {
                Volume = this.Volume
            };
            waveOutEvent.Init(waveStream);
            waveOutEvent.PlaybackStopped += (object? sender, StoppedEventArgs e) => {
                OnPlaybackStopped?.Invoke(this, e);
                waveStream.Dispose();
                waveOutEvent.Dispose();
                _playingEvents.Remove(waveOutEvent);
                _waveStreams.Remove(waveStream);
                if(Loop)
                    play();
            };

            _playingEvents.Add(waveOutEvent);
            _waveStreams.Add(waveStream);
            waveOutEvent.Play();
        }

        public bool is_playing() { return _playingEvents.Exists(waveOut => waveOut.PlaybackState == PlaybackState.Playing); }

        public void pause() {

            foreach(var waveOut in _playingEvents)
                waveOut.Pause();
        }

        public void stop() {

#if false
            var playingEventsCopy = new List<WaveOutEvent>(_playingEvents);
            var waveStreamsCopy = new List<WaveStream>(_waveStreams);

            foreach(var waveOut in playingEventsCopy) {
                waveOut.Stop();
                waveOut.Dispose();
            }

            foreach(var waveStream in waveStreamsCopy) {
                waveStream.Dispose();
            }

            _playingEvents.Clear();
            _waveStreams.Clear();
#else
            try {

                foreach(var waveOut in _playingEvents)
                    waveOut.Stop();
            }
            catch(Exception ex) {

                Console.WriteLine($"Exeption trown in Sound.cs[stop()] => [{ex.ToString()}]");
            }

            _playingEvents.Clear();
            _waveStreams.Clear();
#endif
        }

        // ---------------------------------------------------------------------------------------------------------------
        // PRIVATE
        // ---------------------------------------------------------------------------------------------------------------

        private void load_audio_data() {

            using(var reader = create_reader(FilePath)) {
                _waveFormat = reader.WaveFormat;
                using(var memoryStream = new MemoryStream()) {
                    reader.CopyTo(memoryStream);
                    _audioData = memoryStream.ToArray();
                }
            }
        }

        private WaveStream create_reader(string filePath) {

            if(filePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                return new MediaFoundationReader(filePath);

            else if(filePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                return new AudioFileReader(filePath);

            else
                throw new NotSupportedException("Unsupported file format");
        }

    }

}
