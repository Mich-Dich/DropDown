
namespace Core.util {

    using NAudio.Wave;
    using System;
    using System.Collections.Generic;

    public class Sound : IDisposable {

        public string FilePath { get; private set; }
        public float Volume { get; set; } = 1.0f;
        public bool Loop { get; set; } = false;

        private List<WaveOutEvent> _playingEvents;
        private List<AudioFileReader> _audioFileReaders;

        public Sound(string filePath, float volume = 1.0f, bool loop = false) {

            FilePath = filePath;
            Volume = volume;
            Loop = loop;
            _playingEvents = new List<WaveOutEvent>();
            _audioFileReaders = new List<AudioFileReader>();
        }

        public void Dispose() {

            foreach(var waveOut in _playingEvents)
                waveOut.Dispose();
            
            foreach(var reader in _audioFileReaders)
                reader.Dispose();
            
            _playingEvents.Clear();
            _audioFileReaders.Clear();
        }

        public void Play() {

            var audioFileReader = new AudioFileReader(FilePath) {
                Volume = Volume
            };
            var waveOutEvent = new WaveOutEvent();
            waveOutEvent.Init(audioFileReader);
            waveOutEvent.PlaybackStopped += (object? sender, StoppedEventArgs e) => {

                audioFileReader.Dispose();
                waveOutEvent.Dispose();
                _playingEvents.Remove(waveOutEvent);
                _audioFileReaders.Remove(audioFileReader);
                if(Loop)
                    Play();
            };

            _playingEvents.Add(waveOutEvent);
            _audioFileReaders.Add(audioFileReader);
            waveOutEvent.Play();
        }

        public void Pause() {

            foreach(var waveOut in _playingEvents)
                waveOut.Pause();
        }

        public void Stop() {

            foreach(var waveOut in _playingEvents)
                waveOut.Stop();

            _playingEvents.Clear();
            _audioFileReaders.Clear();
        }
    }
}
