namespace Core.util
{
    public class ShakeProfile
    {
        public float Intensity { get; set; }
        public float Decay { get; set; }
        public float Frequency { get; set; }

        public ShakeProfile(float intensity, float decay, float frequency)
        {
            Intensity = intensity;
            Decay = decay;
            Frequency = frequency;
        }
    }
}