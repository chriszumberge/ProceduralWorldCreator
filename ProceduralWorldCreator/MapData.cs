using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class MapData
    {
        public Tile[,] Tiles;

        // Keep track of min/max for normalization
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }

        public MapData(int width, int height)
        {
            Tiles = new Tile[width, height];
            MinHeight = float.MaxValue;
            MaxHeight = float.MinValue;
        }
    }
}
