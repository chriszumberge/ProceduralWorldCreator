using System;

namespace ProceduralWorldCreator
{
    public abstract class WorldCreator
    {
        protected Random random = new Random();
        public int Width { get; set; } = 256;
        public int Height { get; set; } = 256;

        public float SeaLevel { get; set; } = 0.5f;

        protected MapData mapData;
        public MapData MapData => mapData;

        public abstract void CreateWorld();
    }
}