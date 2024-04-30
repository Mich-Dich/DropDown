using Core.renderer;

namespace Core {

    public interface IAnimatable {
        
        SpriteBatch? SpriteBatch { get; set; }
        Animation? Animation { get; set; }
        int CurrentFrameIndex { get; set; }

        void update_animation();
    }
}