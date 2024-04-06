using OpenTK.Mathematics;

namespace Core.game_objects {

    public class game_object {
    
        public game_object() { }

        public game_object(Vector2 position, Vector2 size, Single scale, Single rotation) {
        
            this.position = position;
            this.size = size;
            this.scale = scale;
            this.rotation = rotation;
        }

        public mobility mobility { get; set; }      // conserning update method
        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public float scale { get; set; }
        public float rotation { get; set; }
    }

    public enum mobility {

        STATIC = 0,
        MOVABLE = 1,
        DYNAMIC = 2,
    }
}
