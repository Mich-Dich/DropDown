using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using OpenTK.Mathematics;

namespace Hell {
    public class base_map : map {
        public base_map() {
            string tmxFilePath = "assets/levels/LargeTestMap.tmx";
            string tsxFilePath = "assets/levels/Tileset.tsx";
            string tilesetImageFilePath = "assets/levels/Tileset.png";

            LoadLevel(tmxFilePath, tsxFilePath, tilesetImageFilePath);
        }
    }
}