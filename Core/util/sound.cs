namespace Core.util
{
    using NetCoreAudio;
    using System;
    
    public class Sound
    {
        public string FilePath { get; private set; }
        public float Volume { get; set; } = 1.0f;
        public bool Loop { get; set; } = false;

        private readonly Player player = new();

        public Sound(string filePath)
        {
            FilePath = filePath;
            player.PlaybackFinished += Player_PlaybackFinished;
        }

        private async void Player_PlaybackFinished(object? sender, EventArgs e)
        {
            if (Loop)
            {
                await Play();
            }
        }

        public async Task Play()
        {
            await player.Play(FilePath);
        }

        public void Stop()
        {
            player.Stop();
        }

        public void Pause()
        {
            player.Pause();
        }
    }
}