namespace Core.util
{
    public class CameraShake
    {
        public float Intensity { get; set; }
        public float Decay { get; set; }

        public CameraShake(float intensity, float decay)
        {
            Intensity = intensity;
            Decay = decay;
        }

        // Define presets for different types of shakes
        public static CameraShake LargeProjectileHit = new CameraShake(0.5f, 0.98f);
    }
}