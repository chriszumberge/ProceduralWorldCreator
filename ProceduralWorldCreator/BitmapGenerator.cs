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
