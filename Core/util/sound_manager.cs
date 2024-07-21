using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.util {

    public class SoundManager {

        private readonly Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private Sound currentBackgroundMusic;

        public void LoadSound(string name, string filePath) {

            sounds[name] = new Sound(filePath);
        }

        public async Task PlaySound(string name, float volume = 1.0f, bool loop = false) {

            if(sounds.TryGetValue(name, out Sound sound)) {
                sound.Volume = volume;
                sound.Loop = loop;
                await sound.Play();
            }
        }

        public async Task PlayBackgroundMusic(string name, float volume = 1.0f, bool loop = true) {

            currentBackgroundMusic?.Stop();
            if(sounds.TryGetValue(name, out Sound sound)) {
                sound.Volume = volume;
                sound.Loop = loop;
                await sound.Play();
                currentBackgroundMusic = sound;
            }
        }

        public void StopBackgroundMusic() {

            currentBackgroundMusic?.Stop();
        }
    }
}
