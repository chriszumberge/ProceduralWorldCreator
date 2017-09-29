using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class WorldCreator
    {
        Random random = new Random();
        public int Width { get; set; } = 256;
        public int Height { get; set; } = 256;

        int? _numberOfPlates = null;
        // Allows the user to define the number of plates at object creation, else it's generated on first get
        public int NumberOfPlates
        {
            get
            {
                if (!_numberOfPlates.HasValue)
                {
                    _numberOfPlates = Width * Height / 4000;
                }
                return _numberOfPlates.Value;
            }
            set { _numberOfPlates = value; }
        }

        public WorldCreator() { }

        MapData mapData;
        public MapData MapData => mapData;

        public void CreateWorld()
        {
            mapData = new MapData(Width, Height);

            InitializeMapData();

            InitializeTectonics();
        }

        void InitializeMapData()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = new Tile();
                    tile.X = x;
                    tile.Y = y;
                    MapData.Tiles[x, y] = tile;
                }
            }
        }

        void InitializeTectonics()
        {
            for (int i = 0; i < NumberOfPlates; i++)
            {
                int randomX = random.Next(Width);
                int randomY = random.Next(Height);
                mapData.Tiles[randomX, randomY].PlateId = i;
            }

            // flood fill the plates
            // TODO- INSTEAD KEEP TRACK OF ALL THE EDGE ONES AND JUST ITERATE THOSE EACH TIME
            while (mapData.Tiles.AsList().Any(x => !x.PlateId.HasValue))
            {
                Console.WriteLine(mapData.Tiles.AsList().Count(x => !x.PlateId.HasValue));
                for (int i = 0; i < NumberOfPlates; i++)
                {
                    //int flootAmount = random.Next(3);

                    List<Tile> plateTiles = mapData.Tiles.AsList().Where(x => x.PlateId == i).ToList();
                    foreach (Tile plateTile in plateTiles)
                    {
                        List<Tile> plateNeighbors = GetNeighbors(mapData.Tiles, Width, Height, plateTile);
                        foreach (Tile neighbor in plateNeighbors.Where(x => !x.PlateId.HasValue))
                        {
                            if (!neighbor.PlateId.HasValue)
                                neighbor.PlateId = i;
                        }
                    }
                }
            }
        }

        private List<Tile> GetNeighbors(Tile[,] tiles, int width, int height, Tile tile)
        {
            int minX = tile.X - 1;
            int maxX = tile.X + 1;
            if (minX < 0) minX = width - 1 - minX;
            if (maxX >= width) maxX = maxX - width;

            int minY = tile.Y - 1;
            int maxY = tile.Y + 1;
            if (minY < 0) minY = 0;
            if (maxY >= height) maxY = height - 1;

            List<Tile> neighbors = tiles.AsList().Where(t => !(t == tile) &&
                (t.Y >= minY && t.Y <= maxY) &&
                (t.X >= minX && t.X <= maxX)).ToList();
            return neighbors;
        }
    }
}
