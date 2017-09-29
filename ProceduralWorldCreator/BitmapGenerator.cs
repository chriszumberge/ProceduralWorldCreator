using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public static class BitmapGenerator
    {

        static Color GetRandomColor(Random random)
        {
            var letters = "0123456789ABCDEF";
            var colorStr = "#";
            for (var i = 0; i < 6; i++)
            {
                colorStr += letters[random.Next(16)];
            }
            return System.Drawing.ColorTranslator.FromHtml(colorStr);
        }

        public static Bitmap GetTectonicMap(int width, int height, Tile[,] tiles)
        {
            return BitmapGenerator.GetTectonicMap(width, height, tiles, tiles.AsList().Select(x => x.PlateId).Distinct().Count());
        }

        public static Bitmap GetTectonicMap(int width, int height, Tile[,] tiles, int numPlates)
        {
            Random random = new Random();
            Dictionary<int, Color> plateColorDictionary = new Dictionary<int, Color>();
            for (int i = 0; i < numPlates; i++)
            {
                plateColorDictionary.Add(i, GetRandomColor(random));
            }

            var bitmap = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Tile tile = tiles[x, y];
                    int plateId = tile.PlateId.Value;
                    if (!tile.IsPlateCenter)
                    {
                        Color color = plateColorDictionary[plateId];
                        
                        if (tile.IsPlateBorder)
                        {
                            color = ColorHelpers.Lerp(Color.Black, color, 0.4f);
                        }

                        bitmap.SetPixel(x, y, color);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, Color.Red);
                    }
                }
            }

            return bitmap;
        }

        public static Bitmap GetHeightMap(int width, int height, Tile[,] tiles)
        {
            var bitmap = new Bitmap(width, height);

            var tileList = tiles.AsList();
            var minHeight = tileList.Min(x => x.HeightValue);
            var maxHeight = tileList.Max(x => x.HeightValue);;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    float value = tiles[x, y].HeightValue;

                    // Set color range, 0 = black, 1 = white
                    //Color color = ColorHelpers.Lerp(Color.Black, Color.White, value);
                    Color color = ColorHelpers.Lerp(Color.Black, Color.White, minHeight, maxHeight, value);
                    //pixels[x + y * width] = color;
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public static Bitmap GetColoredHeightMap(int width, int height, MapData mapData)
        {
            var bitmap = new Bitmap(width, height);

            var tileList = mapData.Tiles.AsList();
            var minHeight = tileList.Min(X => X.HeightValue);
            var maxHeight = tileList.Max(x => x.HeightValue);
            var sealevel = mapData.SeaLevel;

            float deepWater = sealevel / 2.0f;
            float shallowWater = sealevel;
            float landLevel = (1.0f - sealevel);
            float sand = sealevel + (landLevel / 6);
            float grass = sealevel + (landLevel / 2);
            float forest = sealevel + ( 2 * landLevel / 3);
            float rock = sealevel + (5 * landLevel / 6);
            float snow = sealevel + landLevel;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    float value = mapData.Tiles[x, y].HeightValue;
                    // normalize
                    value = (value - minHeight) / (maxHeight - minHeight);

                    Color color;
                    if (value <= deepWater)
                    {
                        color = Color.FromArgb(0, 0, 127);
                    }
                    else if (value <= shallowWater)
                    {
                        color = Color.FromArgb(25, 25, 150);
                    }
                    else if (value <= sand)
                    {
                        color = Color.FromArgb(240, 240, 64);
                    }
                    else if (value <= grass)
                    {
                        color = Color.FromArgb(50, 220, 20);
                    }
                    else if (value <= forest)
                    {
                        color = Color.FromArgb(16, 160, 0);
                    }
                    else if (value <= rock)
                    {
                        color = Color.FromArgb(127, 127, 127);
                    }
                    else
                    {
                        color = Color.FromArgb(255, 255, 255);
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }


        public static Bitmap GetLandMap(int width, int height, MapData mapData)
        {
            var bitmap = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    float value = mapData.Tiles[x, y].HeightValue;

                    Color color;
                    if (value < mapData.SeaLevel)
                    {
                        color = Color.Blue;
                    }
                    else
                    {
                        color = Color.White;
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }
    }
}
