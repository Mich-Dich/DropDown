using System;
using System.Collections.Generic;
using System.Xml.Linq;
using OpenTK.Mathematics;

namespace Core.util {
    public class level_parser {
        public static LevelData ParseLevel(string tmxFilePath, string tsxFilePath) {
            MapData mapData = ParseTMX(tmxFilePath);
            TilesetData tilesetData = ParseTSX(tsxFilePath);

            return new LevelData {
                Map = mapData,
                Tileset = tilesetData
            };
        }

        private static MapData ParseTMX(string tmxFilePath) {
            XDocument doc = XDocument.Load(tmxFilePath);
            XElement mapElement = doc.Element("map");

            int width = Convert.ToInt32(mapElement.Attribute("width").Value);
            int height = Convert.ToInt32(mapElement.Attribute("height").Value);
            int tileWidth = Convert.ToInt32(mapElement.Attribute("tilewidth").Value);
            int tileHeight = Convert.ToInt32(mapElement.Attribute("tileheight").Value);

            List<TilesetData> tilesets = new List<TilesetData>();
            foreach (XElement tilesetElement in mapElement.Elements("tileset")) {
                string source = tilesetElement.Attribute("source").Value;
                int firstgid = Convert.ToInt32(tilesetElement.Attribute("firstgid").Value);
                tilesets.Add(new TilesetData { Source = source, FirstGid = firstgid });
            }

            List<LayerData> layers = new List<LayerData>();
            foreach (XElement layerElement in mapElement.Elements("layer")) {
                string name = layerElement.Attribute("name").Value;
                LayerData layerData = new LayerData { Name = name, Tiles = new int[width, height] };

                XElement dataElement = layerElement.Element("data");
                string[] gidStrings = dataElement.Value.Split(',');
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        int gid = Convert.ToInt32(gidStrings[y * width + x]);
                        // Automatically adjust the tile ID by subtracting 1 from each non-zero ID
                        layerData.Tiles[x, y] = gid > 0 ? gid - 1 : gid;
                    }
                }
                layers.Add(layerData);
            }

            return new MapData {
                Width = width,
                Height = height,
                TileWidth = tileWidth,
                TileHeight = tileHeight,
                Tilesets = tilesets,
                Layers = layers
            };
        }

        private static TilesetData ParseTSX(string tsxFilePath) {
            XDocument doc = XDocument.Load(tsxFilePath);
            XElement tilesetElement = doc.Element("tileset");

            string name = tilesetElement.Attribute("name").Value;
            int tileWidth = Convert.ToInt32(tilesetElement.Attribute("tilewidth").Value);
            int tileHeight = Convert.ToInt32(tilesetElement.Attribute("tileheight").Value);
            int tileCount = Convert.ToInt32(tilesetElement.Attribute("tilecount").Value);
            int columns = Convert.ToInt32(tilesetElement.Attribute("columns").Value);
            string imageSource = tilesetElement.Element("image").Attribute("source").Value;
            int imageWidth = Convert.ToInt32(tilesetElement.Element("image").Attribute("width").Value);
            int imageHeight = Convert.ToInt32(tilesetElement.Element("image").Attribute("height").Value);

            // New: Parse custom properties for each tile
            Dictionary<int, bool> collidableTiles = new Dictionary<int, bool>();
            foreach (XElement tileElement in tilesetElement.Elements("tile")) {
                int id = Convert.ToInt32(tileElement.Attribute("id").Value);
                XElement propertiesElement = tileElement.Element("properties");
                if (propertiesElement != null) {
                    foreach (XElement property in propertiesElement.Elements("property")) {
                        if (property.Attribute("name").Value == "collidable" && property.Attribute("value").Value == "true") {
                            collidableTiles[id] = true;
                        }
                    }
                }
            }

            return new TilesetData {
                Name = name,
                TileWidth = tileWidth,
                TileHeight = tileHeight,
                TileCount = tileCount,
                Columns = columns,
                ImageSource = imageSource,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight,
                CollidableTiles = collidableTiles // Add this property to your TilesetData class
            };
        }
    }

    public class LevelData {
        public MapData Map { get; set; }
        public TilesetData Tileset { get; set; }
    }

    public class TilesetData {
        public string Source { get; set; }
        public int FirstGid { get; set; }
        public string Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int TileCount { get; set; }
        public int Columns { get; set; }
        public string ImageSource { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        // New property to store collidable tile IDs
        public Dictionary<int, bool> CollidableTiles { get; set; } = new Dictionary<int, bool>();
    }

    public class MapData {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public List<TilesetData> Tilesets { get; set; }
        public List<LayerData> Layers { get; set; }

        // New properties for actual level dimensions in pixels
        public int LevelPixelWidth => Width * TileWidth;
        public int LevelPixelHeight => Height * TileHeight;
    }

    public class LayerData {
        public string Name { get; set; }
        public int[,] Tiles { get; set; }
    }
}