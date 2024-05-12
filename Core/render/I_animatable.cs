
namespace Core.render {

    public interface I_animatable {

        Animation? animation { get; set; }
        float animation_timer { get; set; }

        public Sprite select_texture_region(int number_of_columns = 1, int number_of_rows = 1, int column_index = 0, int row_index = 0);

    }
}
