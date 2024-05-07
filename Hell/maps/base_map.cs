using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using OpenTK.Mathematics;

namespace Hell {
    public class base_map : map {
        public base_map() {
            this.cell_size = 32;
            this.tile_size = 1;
            string tmxFilePath = "assets/firstLevel/TestLevel.tmx";
            string tsxFilePath = "assets/firstLevel/spr_heaven.tsx";
            string tilesetImageFilePath = "assets/firstLevel/spr_heaven.png";

            LoadLevel(tmxFilePath, tsxFilePath, tilesetImageFilePath);
        }
    }
}