
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
        }

        public void InitializeProjectiles() {
            // Create the TestProjectile here instead
            this.Add_Game_Object(new TestProjectile(new Vector2(0, 0), new Vector2(1, 0)));
        }
    }
}