using System;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Audio.OpenAL;

namespace Core.util
{
    public class Sound
    {
        public string FilePath { get; private set; }
        public float Volume { get; set; } = 1.0f;
        public bool Loop { get; set; } = false;

        private int bufferId;
        private int sourceId;

        public Sound(string filePath)
        {
            FilePath = filePath;
            bufferId = AL.GenBuffer();
            sourceId = AL.GenSource();

            var soundData = LoadSoundData(filePath);

            GCHandle handle = GCHandle.Alloc(soundData.RawData, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                AL.BufferData(bufferId, soundData.SoundFormat, pointer, soundData.RawData.Length, soundData.SampleRate);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }

            AL.Source(sourceId, ALSourcei.Buffer, bufferId);
        }

        private (byte[] RawData, ALFormat SoundFormat, int SampleRate) LoadSoundData(string filePath)
        {
            // Implementation for loading WAV files and extracting PCM data
            throw new NotImplementedException("LoadSoundData method needs to be implemented based on your sound file format.");
        }

        public void Play()
        {
            AL.Source(sourceId, ALSourcef.Gain, Volume);
            AL.Source(sourceId, ALSourceb.Looping, Loop);
            AL.SourcePlay(sourceId);
        }

        public void Stop()
        {
            AL.SourceStop(sourceId);
        }

        public void Pause()
        {
            AL.SourcePause(sourceId);
        }
    }
}