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

        protected Vector2 position { get; set; }
        protected Vector2 size { get; set; }
        protected float scale { get; set; }
        protected float rotation { get; set; }

    }
}
