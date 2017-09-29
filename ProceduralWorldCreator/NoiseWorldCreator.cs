using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinkerWorX.AccidentalNoiseLibrary;

namespace ProceduralWorldCreator
{
    public class NoiseWorldCreator : WorldCreator
    {
        public int TerrainOctaves { get; set; } = 6;
        public double TerrainFrequency { get; set; } = 1.25;

        public override void CreateWorld()
        {
            mapData = new MapData(Width, Height);
            mapData.SeaLevel = SeaLevel;

            ImplicitFractal HeightMap = new ImplicitFractal(FractalType.Multi, BasisType.Simplex, InterpolationType.Quintic)
            {
                Octaves = TerrainOctaves,
                Frequency = TerrainFrequency,
                Seed = random.Next(0, Int32.MaxValue)
            };

            float[,] heightMap;
            float minHeight, maxHeight;

            GetHeightData(HeightMap, out heightMap, out minHeight, out maxHeight);
            LoadTiles(heightMap, minHeight, maxHeight);
        }

        private void GetHeightData(ImplicitFractal HeightMap, out float[,] heightMap, out float minHeight, out float maxHeight)
        {
            heightMap = new float[Width, Height];
            minHeight = float.MaxValue;
            maxHeight = float.MinValue;
            // Get all the values to be used and track max/min to normalize when assigning to tiles
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    // X-AXIS WRAPPING
                    //// noise range
                    //float x1 = 0, x2 = 1;
                    //float y1 = 0, y2 = 1;
                    //float dx = x2 - x1;
                    //float dy = y2 - y1;

                    //// Sample nosie at smaller intervals
                    //float s = x / (float)Width;
                    //float t = y / (float)Height;

                    //// Calculate 3D coordinates
                    //float nx = (float)(x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI));
                    //float ny = (float)(x1 + Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI));
                    //float nz = t;

                    //float heightValue = (float)HeightMap.Get(nx, ny, nz);

                    //// keep track of max and min
                    //if (heightValue > maxHeight) maxHeight = heightValue;
                    //if (heightValue < minHeight) minHeight = heightValue;

                    //heightMap[x, y] = heightValue;

                    // X AND Y AXIS WRAPPING
                    // Noise range
                    float x1 = 0, x2 = 2;
                    float y1 = 0, y2 = 2;
                    float dx = x2 - x1;
                    float dy = y2 - y1;

                    // Sample noise at smaller intervals
                    float s = x / (float)Width;
                    float t = y / (float)Height;

                    // Calculate our 4D coordinates
                    float nx = (float)(x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI));
                    float ny = (float)(y1 + Math.Cos(t * 2 * Math.PI) * dy / (2 * Math.PI));
                    float nz = (float)(x1 + Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI));
                    float nw = (float)(y1 + Math.Sin(t * 2 * Math.PI) * dy / (2 * Math.PI));

                    float heightValue = (float)HeightMap.Get(nx, ny, nz, nw);

                    // keep track of the max and min values found
                    if (heightValue > maxHeight) maxHeight = heightValue;
                    if (heightValue < minHeight) minHeight = heightValue;

                    heightMap[x, y] = heightValue;
                }
            }
        }

        private void LoadTiles(float[,] heightMap, float minHeight, float maxHeight)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Tile tile = new Tile
                    {
                        X = x,
                        Y = y
                    };

                    tile.HeightValue = (heightMap[x, y] - minHeight) / (maxHeight - minHeight);

                    mapData.Tiles[x, y] = tile;
                }
            }
        }
    }
}
