using OpenTK.Mathematics;

namespace Core.game_objects {

    public class game_object {
    
        public mobility mobility { get; set; }      // conserning update method
        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public Vector2 scale { get; set; }
        public float rotation { get; set; }

        public game_object() { }

        public game_object(mobility mobility, Vector2 position, Vector2 size, Vector2 scale, Single rotation) {

            this.mobility = mobility;
            this.position = position;
            this.size = size;
            this.scale = scale;
            this.rotation = rotation;
        }

    }

    public enum mobility {

        STATIC = 0,
        MOVABLE = 1,
        DYNAMIC = 2,
    }
}
