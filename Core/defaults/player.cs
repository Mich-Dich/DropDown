namespace Core.defaults
{
    using Core.util;
    using Core.world;

    public class Player : Character
    {
        public Player()
        {
            this.transform.size = new OpenTK.Mathematics.Vector2(50);
            this.transform.rotation = float.Pi;
            this.Set_Sprite(new Core.render.Sprite(Resource_Manager.Get_Texture("assets/textures/player/00.png")));

            this.movement_speed = 5.0f;
        }
    }
}
