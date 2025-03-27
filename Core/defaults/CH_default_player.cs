
using Core.render;
using OpenTK.Mathematics;
using System.Reflection;

namespace Core.defaults {

    public class CH_default_player : world.Character {

        private const float DefaultSize = 50.0f;
        private const float DefaultRotation = MathF.PI;
        private const string DefaultResourceName = "Core.defaults.textures.default_grid.png";
        private const float DefaultMovementSpeed = 200.0f;

        // Initializes a new instance of the <see cref="CH_default_player"/> class.
        public CH_default_player() {

            // init transform
            this.transform.size = new Vector2(DefaultSize);
            this.transform.rotation = DefaultRotation;

            // load and set sprite
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(DefaultResourceName))
            {

                if (stream == null)
                    return;

                var texture = new Texture(stream);
                this.Set_Sprite(new Core.world.Sprite(texture));
            }

            // init movement
            this.movement_speed = DefaultMovementSpeed;
        }
    }
}
