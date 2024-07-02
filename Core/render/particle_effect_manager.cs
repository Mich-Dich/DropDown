namespace Core.render
{
    public class ParticleEffectManager
    {
        private List<ParticleEffect> effects = new List<ParticleEffect>();

        public void AddEffect(ParticleEffect effect)
        {
            effects.Add(effect);
        }

        public void Update(float deltaTime)
        {
            //effects.ForEach(effect => effect.Update(deltaTime));
        }

        public void Draw()
        {
            effects.ForEach(effect => effect.Draw());
        }
    }
}