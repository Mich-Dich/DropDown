using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using OpenTK.Mathematics;
using System;
using System.Linq;

namespace Hell {
    public class Base_Map : map {
        

        public Base_Map() {
            this.cellSize = 32;
            this.tileSize = 1;
            string tmxFilePath = "assets/firstLevel/TestLevel.tmx";
            string tsxFilePath = "assets/firstLevel/spr_heaven.tsx";
            string tilesetImageFilePath = "assets/firstLevel/spr_heaven.png";

            LoadLevel(tmxFilePath, tsxFilePath, tilesetImageFilePath);

            add_character(new enemy(), new Vector2(300, 5800));
            add_character(new enemy(), new Vector2(250, 5600));
            add_character(new enemy(), new Vector2(300, 5000));
            add_character(new enemy(), new Vector2(210, 4800));
            add_character(new enemy(), new Vector2(350, 4586));
            add_character(new enemy(), new Vector2(350, 4600));
            add_character(new enemy(), new Vector2(300, 4300));
            add_character(new enemy(), new Vector2(300, 4300));
            add_character(new enemy(), new Vector2(300, 4200));
            add_character(new enemy(), new Vector2(220, 3500));
            add_character(new enemy(), new Vector2(300, 3200));
            add_character(new enemy(), new Vector2(300, 3201));
            add_character(new enemy(), new Vector2(300, 2400));
            add_character(new enemy(), new Vector2(300, 2500));
            add_character(new enemy(), new Vector2(260, 2000));
            add_character(new enemy(), new Vector2(300, 1500));
            add_character(new enemy(), new Vector2(400, 1400));
            add_character(new enemy(), new Vector2(300, 1000));
            add_character(new enemy(), new Vector2(300, 1000));
            add_character(new enemy(), new Vector2(300, 800));
            add_character(new enemy(), new Vector2(210, 400));
        }

        
    }
}