using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class WorldCreator
    {
        public WorldCreator()
        {

        }

        MapData mapData;
        public void CreateWorld(int width = 256, int height = 256)
        {
            mapData = new MapData(width, height);

        }
    }
}
