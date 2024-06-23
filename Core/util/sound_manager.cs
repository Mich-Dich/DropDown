using System.Collections.Generic;
using OpenTK.Audio.OpenAL;

namespace Core.util
{
    public class SoundManager
    {
        private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private Sound currentBackgroundMusic;

        public void LoadSound(string name, string filePath)
        {
            sounds[name] = Resource_Manager.Get_Sound(filePath);
        }

        public void PlaySound(string name, float volume = 1.0f, bool loop = false)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                sound.Volume = volume;
                sound.Loop = loop;
                sound.Play();
            }
        }

        public void PlayBackgroundMusic(string name, float volume = 1.0f, bool loop = true)
        {
            if (currentBackgroundMusic != null)
            {
                currentBackgroundMusic.Stop();
            }

            if (sounds.TryGetValue(name, out Sound sound))
            {
                sound.Volume = volume;
                sound.Loop = loop;
                sound.Play();
                currentBackgroundMusic = sound;
            }
        }

        public void StopBackgroundMusic()
        {
            currentBackgroundMusic?.Stop();
        }
    }
}