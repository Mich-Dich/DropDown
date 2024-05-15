namespace Hell {

    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using Core.world.map;
    using Hell.enemy;
    using Hell.weapon;
    using ImGuiNET;
    using OpenTK.Mathematics;
    using System.Diagnostics;

    public class Base_Map : Map {

        public Base_Map() {
            Base_Enemy enemy = new Base_Enemy();
            Base_AI_Controller aiController = new Base_AI_Controller(enemy);

            Random random = new Random();
            int offset_x = random.Next(64);
            int offset_y = random.Next(64);

            this.Add_Character(aiController, new Vector2((offset_x - 32) * this.cellSize, (offset_y - 32) * this.cellSize), 0);
        }
    }
}