using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class MapData
    {
        public float[,] HeightData;
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }

        public MapData(int width, int height)
        {
            HeightData = new float[width, height];
            MinHeight = float.MaxValue;
            MaxHeight = float.MinValue;
        }
    }
}
