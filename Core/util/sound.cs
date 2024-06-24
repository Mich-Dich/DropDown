
namespace Core.util {

    using NAudio.Wave;

    public class Sound : IDisposable {

        public string FilePath { get; private set; }
        public float Volume { get; set; } = 1.0f;
        public bool Loop { get; set; } = false;

        private AudioFileReader _audioFileReader;
        private WaveOutEvent _event;

        public Sound(string filePath, float Volume = 10.0f, bool Loop = false) {
            
            FilePath = filePath;
            Volume = Volume;
            Loop = Loop;

            // Play the .wav file
            _audioFileReader = new AudioFileReader(filePath);
            _event = new WaveOutEvent();
            _event.Init(_audioFileReader);
            _event.PlaybackStopped += (object sender, StoppedEventArgs e) => {

                _audioFileReader.Position = 0; // Reset position to start
                if(Loop)
                    _event.Play();
            };
        }

        public void Dispose() {
            _event.Dispose();
            _audioFileReader.Dispose();
        }

        public void Play() { _event.Play(); }

        public void Pause() { _event.Pause(); }

        public void Stop() { 

            _event.Stop();
            _audioFileReader.Position = 0; // Reset position to start
        }
    }
}
