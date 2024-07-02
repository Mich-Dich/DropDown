namespace Core.util
{
    public class CameraShake
    {
        public static ShakeProfile LargeProjectileHit = new ShakeProfile(0.5f, 0.98f, 25f);
        public static ShakeProfile Explosion = new ShakeProfile(1f, 0.95f, 30f);
        public static ShakeProfile Earthquake = new ShakeProfile(0.4f, 0.99f, 10f);
    }
}