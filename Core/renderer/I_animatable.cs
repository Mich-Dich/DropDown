using Core.visual;

namespace Core.renderer
{

    public interface I_animatable
    {

        animation? animation { get; set; }
        float animation_timer { get; set; }

        void update_animation();
    }
}