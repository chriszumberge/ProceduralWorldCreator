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
                    Color color = plateColorDictionary[plateId];

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public static Bitmap GetHeightMap(int width, int height, Tile[,] tiles)
        {
            var bitmap = new Bitmap(width, height);

            //var pixels = new Color[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    float value = tiles[x, y].HeightValue;

                    // Set color range, 0 = black, 1 = white
                    Color color = ColorHelpers.Lerp(Color.Black, Color.White, value);
                    //pixels[x + y * width] = color;
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }
    }
}
