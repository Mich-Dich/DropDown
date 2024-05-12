
namespace Core.render {

    public interface I_animatable {

        Animation? animation { get; set; }
        float animation_timer { get; set; }

        /// <summary>
        /// Selects a specific texture region from a sprite sheet based on the given parameters.
        /// </summary>
        /// <param name="number_of_columns">The number of columns in the sprite sheet (default is 1).</param>
        /// <param name="number_of_rows">The number of rows in the sprite sheet (default is 1).</param>
        /// <param name="column_index">The index of the column containing the desired texture region (default is 0).</param>
        /// <param name="row_index">The index of the row containing the desired texture region (default is 0).</param>
        /// <returns>The sprite representing the selected texture region.</returns>
        public Sprite select_texture_region(int number_of_columns = 1, int number_of_rows = 1, int column_index = 0, int row_index = 0);

    }
}
