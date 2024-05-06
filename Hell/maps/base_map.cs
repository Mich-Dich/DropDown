using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using OpenTK.Mathematics;

namespace Hell {
    public class base_map : map {
        public base_map(int tilesOnScreenWidth, int tilesOnScreenHeight, camera camera) 
            : base(tilesOnScreenWidth, tilesOnScreenHeight, camera) {
            string tmxFilePath = "assets/levels/LargeTestMap.tmx";
            string tsxFilePath = "assets/levels/Tileset.tsx";
            string tilesetImageFilePath = "assets/levels/Tileset.png";

            LoadLevel(tmxFilePath, tsxFilePath, tilesetImageFilePath);
        }
    }
}