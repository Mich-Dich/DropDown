using Core.renderer;
using Core.visual;

namespace Core
{

    public interface IAnimatable {
        
        animation? animation { get; set; }
        float animation_timer { get; set; }

        void update_animation();
    }
}