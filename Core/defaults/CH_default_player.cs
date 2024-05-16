
namespace Core.defaults {

    public class CH_default_player : world.Character {

        public CH_default_player() {
            
            this.transform.size = new OpenTK.Mathematics.Vector2(50);
            this.transform.rotation = float.Pi;
            this.Set_Sprite(new Core.world.Sprite(util.Resource_Manager.Get_Texture("defaults/textures/default_grid.png")));
            this.movement_speed = 200.0f;
        }
    }
}
