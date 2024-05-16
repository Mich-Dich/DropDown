using Core.render;
using System.Reflection;
namespace Core.defaults {

    public class CH_default_player : world.Character {

        public CH_default_player() {
            
            this.transform.size = new OpenTK.Mathematics.Vector2(50);
            this.transform.rotation = float.Pi;
            
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Core.defaults.textures.default_grid.png";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return;
                var texture = new Texture(stream);
                this.Set_Sprite(new Core.world.Sprite(texture));
            }

            this.movement_speed = 200.0f;
        }
    }
}
