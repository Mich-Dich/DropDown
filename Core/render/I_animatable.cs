namespace Core.render
{
    public interface I_animatable
    {
        Animation? animation { get; set; }

        float animationTimer { get; set; }

        /// <summary>
        /// Selects a specific texture region from a sprite sheet based on the given parameters.
        /// </summary>
        /// <param name="numberOfColumns">The number of columns in the sprite sheet (default is 1).</param>
        /// <param name="numberOfRows">The number of rows in the sprite sheet (default is 1).</param>
        /// <param name="columnIndex">The index of the column containing the desired texture region (default is 0).</param>
        /// <param name="rowIndex">The index of the row containing the desired texture region (default is 0).</param>
        /// <returns>The sprite representing the selected texture region.</returns>
        public Sprite Select_Texture_Region(int numberOfColumns = 1, int numberOfRows = 1, int columnIndex = 0, int rowIndex = 0);
    }
}
