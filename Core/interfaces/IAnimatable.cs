using Core.renderer;

namespace Core {

    public interface IAnimatable {
        
        SpriteBatch? SpriteBatch { get; set; }
        int CurrentFrameIndex { get; set; }
        Animation? Animation { get; set; }
        void Update();
    }
}