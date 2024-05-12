
namespace Core.util {

    using System.Xml.Linq;

    public class Level_Parser {

        /// <summary>
        /// Parses a level from TMX and TSX files, and constructs a Level_Data object containing map and tileset information.
        /// </summary>
        /// <param name="tmxFilePath">The file path to the TMX file containing map data.</param>
        /// <param name="tsxFilePath">The file path to the TSX file containing tileset data.</param>
        /// <returns>A Level_Data object representing the parsed level data.</returns>
        public static Level_Data Parse_Level(string tmxFilePath, string tsxFilePath) {

            Map_Data mapData = Parse_TMX(tmxFilePath);
            Tileset_Data tilesetData = Parse_TSX(tsxFilePath);

            return new Level_Data {
                Map = mapData,
                Tileset = tilesetData
            };
        }

        /// <summary>
        /// Parses TMX file data to extract map-related information.
        /// </summary>
        /// <param name="tmxFilePath">The file path to the TMX file.</param>
        /// <returns>A Map_Data object containing parsed map information.</returns>
        private static Map_Data Parse_TMX(string tmxFilePath) {

            XDocument doc = XDocument.Load(tmxFilePath);
            XElement? mapElement = doc.Element("map");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int width       = Convert.ToInt32(mapElement.Attribute("width").Value);
            int height      = Convert.ToInt32(mapElement.Attribute("height").Value);
            int tileWidth   = Convert.ToInt32(mapElement.Attribute("tilewidth").Value);
            int tileHeight  = Convert.ToInt32(mapElement.Attribute("tileheight").Value);

            List<Tileset_Data> tilesets = new List<Tileset_Data>();
            foreach (XElement tilesetElement in mapElement.Elements("tileset")) {
                
                string source = tilesetElement.Attribute("source").Value;
                int firstgid = Convert.ToInt32(tilesetElement.Attribute("firstgid").Value);
                tilesets.Add(new Tileset_Data { Source = source, FirstGid = firstgid });
            }

            List<Layer_Data> layers = new List<Layer_Data>();
            foreach (XElement layerElement in mapElement.Elements("layer")) {

                string name = layerElement.Attribute("name").Value;
                Layer_Data layerData = new Layer_Data { Name = name, Tiles = new int[width, height] };

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                XElement dataElement = layerElement.Element("data");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                string[] gidStrings = dataElement.Value.Split(',');
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        
                        int gid = Convert.ToInt32(gidStrings[y * width + x]);
                        layerData.Tiles[x, y] = gid > 0 ? gid - 1 : gid;        // Automatically adjust the tile ID by subtracting 1 from each non-zero ID
                    }
                }
                layers.Add(layerData);
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return new Map_Data {
                
                Width = width,
                Height = height,
                TileWidth = tileWidth,
                TileHeight = tileHeight,
                Tilesets = tilesets,
                Layers = layers
            };
        }

        /// <summary>
        /// Parses TSX file data to extract tileset-related information.
        /// </summary>
        /// <param name="tsxFilePath">The file path to the TSX file.</param>
        /// <returns>A Tileset_Data object containing parsed tileset information.</returns>
        private static Tileset_Data Parse_TSX(string tsxFilePath) {

            XDocument doc = XDocument.Load(tsxFilePath);
            XElement? tilesetElement = doc.Element("tileset");

            if(tilesetElement == null)
                throw new Exception("Could not load level content from TSX file");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string name             = tilesetElement.Attribute("name").Value;
            int tileWidth           = Convert.ToInt32(tilesetElement.Attribute("tilewidth").Value);
            int tileHeight          = Convert.ToInt32(tilesetElement.Attribute("tileheight").Value);
            int tileCount           = Convert.ToInt32(tilesetElement.Attribute("tilecount").Value);
            int columns             = Convert.ToInt32(tilesetElement.Attribute("columns").Value);
            string imageSource      = tilesetElement.Element("image").Attribute("source").Value;
            int imageWidth          = Convert.ToInt32(tilesetElement.Element("image").Attribute("width").Value);
            int imageHeight         = Convert.ToInt32(tilesetElement.Element("image").Attribute("height").Value);

            // New: Parse custom properties for each tile
            Dictionary<int, bool> collidableTiles = new Dictionary<int, bool>();
            foreach (XElement tileElement in tilesetElement.Elements("tile")) {

                int id = Convert.ToInt32(tileElement.Attribute("id").Value);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                XElement propertiesElement = tileElement.Element("properties");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (propertiesElement == null)
                    continue;
                
                foreach(XElement property in propertiesElement.Elements("property")) {
                    if(property.Attribute("name").Value == "collidable" && property.Attribute("value").Value == "true")
                        collidableTiles[id] = true;
                    
                }
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return new Tileset_Data {

                Name = name,
                TileWidth = tileWidth,
                TileHeight = tileHeight,
                TileCount = tileCount,
                Columns = columns,
                ImageSource = imageSource,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight,
                CollidableTiles = collidableTiles // Add this property to your Tileset_Data class
            };
        }
    }

    public class Level_Data {

        public required Map_Data Map { get; set; }
        public required Tileset_Data Tileset { get; set; }
    }

    public class Tileset_Data {

        public string? Source { get; set; }
        public int FirstGid { get; set; }
        public string? Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int TileCount { get; set; }
        public int Columns { get; set; }
        public string? ImageSource { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        // New property to store collidable tile IDs
        public Dictionary<int, bool> CollidableTiles { get; set; } = new Dictionary<int, bool>();
    }

    public class Map_Data {

        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public required List<Tileset_Data> Tilesets { get; set; }
        public required List<Layer_Data> Layers { get; set; }

        // New properties for actual level dimensions in pixels
        public int LevelPixelWidth => Width * TileWidth;
        public int LevelPixelHeight => Height * TileHeight;
    }

    public class Layer_Data {

        public required string Name { get; set; }
        public required int[,] Tiles { get; set; }
    }
}