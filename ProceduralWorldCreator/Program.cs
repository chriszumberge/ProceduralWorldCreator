using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProceduralWorldCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var creator = new RealWorldCreator()
            //{
            //    Height = 100,
            //    Width = 100,
            //    NumberOfPlates = 12
            //};

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //creator.CreateWorld();

            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);

            //Bitmap tectonicMap = BitmapGenerator.GetTectonicMap(creator.Width, creator.Height, creator.MapData.Tiles);
            //BitmapHelpers.ShowBitmap(tectonicMap);

            //Bitmap heightMap = BitmapGenerator.GetHeightMap(creator.Width, creator.Height, creator.MapData.Tiles);
            //BitmapHelpers.ShowBitmap(heightMap);

            //Bitmap landMap = BitmapGenerator.GetLandMap(creator.Width, creator.Height, creator.MapData);
            //BitmapHelpers.ShowBitmap(landMap);

            var creator = new NoiseWorldCreator()
            {
                Height = 256,
                Width = 256,
                SeaLevel = 0.4f,
                TerrainFrequency = 1.25, // smaller number means more continents, larger number means lots of clustered islands
                TerrainOctaves = 6 // defines the "jaggedness" of the land
            };

            creator.CreateWorld();

            Bitmap coloredHeightMap = BitmapGenerator.GetColoredHeightMap(creator.Width, creator.Height, creator.MapData);
            BitmapHelpers.ShowBitmap(coloredHeightMap);


            var ids = creator.MapData.Tiles.AsList().Select(x => x.PlateId).Distinct().ToList();
        }

        
    }
}
