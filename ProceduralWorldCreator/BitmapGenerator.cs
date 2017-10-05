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
            var maxHeight = tileList.Max(x => x.HeightValue); ;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    float value = tiles[x, y].HeightValue;

                    // Set color range, 0 = black, 1 = white
                    //Color color = ColorHelpers.Lerp(Color.Black, Color.White, value);
                    Color color = ColorHelpers.Lerp(Color.Black, Color.White, minHeight, maxHeight, value);
                    //color = color;
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
            float forest = sealevel + (2 * landLevel / 3);
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
                    //if (value <= deepWater)
                    //{
                    //    //color = Color.FromArgb(0, 0, 127);
                    //    color = ColorHelpers.Lerp(Color.Black, Color.White, deepWater);
                    //}
                    //else if (value <= shallowWater)
                    //{
                    //    //color = Color.FromArgb(25, 25, 150);
                    //    color = ColorHelpers.Lerp(Color.Black, Color.White, shallowWater);
                    //}
                    //else if (value <= sand)
                    //{
                    //    //color = Color.FromArgb(240, 240, 64);
                    //    color = ColorHelpers.Lerp(Color.Black, Color.White, sand);
                    //}
                    //else if (value <= grass)
                    //{
                    //    //color = Color.FromArgb(50, 220, 20);
                    //    color = ColorHelpers.Lerp(Color.Black, Color.White, grass);
                    //}
                    //else if (value <= forest)
                    //{
                    //    //color = Color.FromArgb(16, 160, 0);
                    //    color = ColorHelpers.Lerp(Color.Black, Color.White, forest);
                    //}
                    //else if (value <= rock)
                    //{
                    //    //color = Color.FromArgb(127, 127, 127);
                    //    color = ColorHelpers.Lerp(Color.Black, Color.White, rock);
                    //}
                    //else
                    //{
                    //    //color = Color.FromArgb(255, 255, 255);
                    //    color = ColorHelpers.Lerp(Color.Black, Color.White, snow);
                    //}

                    // Clamps to 7 different color values
                    int roundedValue = (int)Math.Floor(value * 7);
                    color = ColorHelpers.Lerp(Color.Black, Color.White, 0.0f, 7.0f, roundedValue);

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

        public static Bitmap GetHeatMap(int width, int height, Tile[,] tiles)
        {
            var bitmap = new Bitmap(width, height);

            float ColdestValue = 0.05f;
            float ColderValue = 0.18f;
            float ColdValue = 0.4f;
            float WarmValue = 0.6f;
            float WarmerValue = 0.8f;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var value = tiles[x, y].HeatValue;
                    //Color color = ColorHelpers.Lerp(Color.Blue, Color.Red, value);
                    //bitmap.SetPixel(x, y, color);

                    Color color;
                    if (value <= ColdestValue)
                    {
                        color = Color.FromArgb(0, 255, 255);
                    }
                    else if (value <= ColderValue)
                    {
                        color = Color.FromArgb(170, 255, 255);
                    }
                    else if (value <= ColdValue)
                    {
                        color = Color.FromArgb(0, 229, 133);
                    }
                    else if (value <= WarmValue)
                    {
                        color = Color.FromArgb(255, 255, 100);
                    }
                    else if (value <= WarmerValue)
                    {
                        color = Color.FromArgb(255, 100, 0);
                    }
                    else // Warmest
                    {
                        color = Color.FromArgb(241, 12, 0);
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public static Bitmap GetMoistureMap(int width, int height, Tile[,] tiles)
        {
            var bitmap = new Bitmap(width, height);

            float DryerValue = 0.27f;
            float DryValue = 0.4f;
            float WetValue = 0.6f;
            float WetterValue = 0.8f;
            float WettestValue = 0.9f;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var value = tiles[x, y].MoistureValue;
                    //Color color = ColorHelpers.Lerp(Color.Blue, Color.Red, value);
                    //bitmap.SetPixel(x, y, color);

                    Color color;
                    if (value < DryerValue) // Dryest
                    {
                        color = Color.FromArgb(255, 139, 17);
                    }
                    else if (value < DryValue) // Dryer
                    {
                        color = Color.FromArgb(245, 245, 23);
                    }
                    else if (value < WetValue) // Dry
                    {
                        color = Color.FromArgb(80, 255, 0);
                    }
                    else if (value < WetterValue) // Wet
                    {
                        color = Color.FromArgb(85, 255, 255);
                    }
                    else if (value < WettestValue) // Wetter
                    {
                        color = Color.FromArgb(20, 70, 255);
                    }
                    else // Wettest
                    {
                        color = Color.FromArgb(0, 0, 100);
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public static Bitmap GetRiverMap(int width, int height, Tile[,] tiles, float seaLevel)
        {
            var bitmap = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Tile tile = tiles[x, y];

                    Color color;
                    if (tile.IsRiver)
                    {
                        color = Color.FromArgb(30, 120, 200);
                    }
                    else
                    {
                        float value = tile.HeightValue;
                        if (value <= seaLevel)
                        {
                            color = Color.Black;
                        }
                        else
                        {
                            color = Color.White;
                        }
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        enum BiomeType
        {
            Desert,
            Savanna,
            TropicalRainforest,
            Grassland,
            Woodland,
            SeasonalForest,
            TemperateRainforest,
            BorealForest,
            Tundra,
            Ice
        }
        static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
            //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
        };

        static Color DeepWater = Color.FromArgb(15, 30, 80);
        static Color ShallowWater = Color.FromArgb(15, 40, 90);

        static Color IceWater = Color.FromArgb(210, 255, 252);
        static Color ColdWater = Color.FromArgb(119, 156, 213);
        static Color RiverWater = Color.FromArgb(65, 110, 179);

        static Color Ice = Color.White;
        static Color Desert = Color.FromArgb(238, 218, 130);
        static Color Savanna = Color.FromArgb(177, 209, 110);
        static Color TropicalRainforest = Color.FromArgb(66, 123, 25);
        static Color Tundra = Color.FromArgb(96, 131, 112);
        static Color TemperateRainforest = Color.FromArgb(29, 73, 40);
        static Color Grassland = Color.FromArgb(164, 255, 99);
        static Color SeasonalForest = Color.FromArgb(73, 100, 35);
        static Color BorealForest = Color.FromArgb(95, 115, 62);
        static Color Woodland = Color.FromArgb(139, 175, 90);

        public static Bitmap GetBiomeMap(int width, int height, Tile[,] tiles, float seaLevel)
        {
            var bitmap = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Tile tile = tiles[x, y];

                    Color color;
                    if (tile.HeightValue > seaLevel)
                    {
                        if (tile.IsRiver)
                        {
                            float heatValue = tile.HeatValue;
                            int heatType = GetHeatType(heatValue);

                            if (heatType == (int)HeatType.Coldest)
                            {
                                color = ColorHelpers.Lerp(IceWater, ColdWater, heatValue / ColdestValue);
                            }
                            else if (heatType == (int)HeatType.Colder)
                            {
                                color = ColorHelpers.Lerp(ColdWater, RiverWater, (heatValue - ColdestValue) / (ColderValue - ColdestValue));
                            }
                            else if (heatType == (int)HeatType.Cold)
                            {
                                color = ColorHelpers.Lerp(RiverWater, ShallowWater, (heatValue - ColderValue) / (ColdValue - ColderValue));
                            }
                            else
                            {
                                color = ShallowWater;
                            }
                        }
                        else
                        {
                            BiomeType biomeType = GetBiomeType(tile);

                            switch(biomeType)
                            {
                                case BiomeType.Ice:
                                    color = Ice;
                                    break;
                                case BiomeType.BorealForest:
                                    color = BorealForest;
                                    break;
                                case BiomeType.Desert:
                                    color = Desert;
                                    break;
                                case BiomeType.Grassland:
                                    color = Grassland;
                                    break;
                                case BiomeType.SeasonalForest:
                                    color = SeasonalForest;
                                    break;
                                case BiomeType.Tundra:
                                    color = Tundra;
                                    break;
                                case BiomeType.Savanna:
                                    color = Savanna;
                                    break;
                                case BiomeType.TemperateRainforest:
                                    color = TemperateRainforest;
                                    break;
                                case BiomeType.TropicalRainforest:
                                    color = TropicalRainforest;
                                    break;
                                case BiomeType.Woodland:
                                    color = Woodland;
                                    break;
                                default:
                                    color = Color.Black;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // Deep
                        if (tile.HeatValue <= (seaLevel / 2))
                        {
                            color = DeepWater;
                        }
                        // Shallow
                        else
                        {
                            color = ShallowWater;
                        }
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        static float DryerValue = 0.27f;
        static float DryValue = 0.4f;
        static float WetValue = 0.6f;
        static float WetterValue = 0.8f;
        static float WettestValue = 0.9f;

        static float ColdestValue = 0.05f;
        static float ColderValue = 0.18f;
        static float ColdValue = 0.4f;
        static float WarmValue = 0.6f;
        static float WarmerValue = 0.8f;

        static BiomeType GetBiomeType(Tile tile)
        {
            int moistureType, heatType;
            float moistureValue = tile.MoistureValue;
            float heatValue = tile.HeatValue;

            moistureType = GetMoistureType(moistureValue);
            heatType = GetHeatType(heatValue);

            return BiomeTable[moistureType, heatType];
        }

        private static int GetHeatType(float heatValue)
        {
            int heatType;
            if (heatValue <= ColdestValue)
                heatType = 0;
            else if (heatValue <= ColderValue)
                heatType = 1;
            else if (heatValue <= ColdValue)
                heatType = 2;
            else if (heatValue <= WarmValue)
                heatType = 3;
            else if (heatValue <= WarmerValue)
                heatType = 4;
            else // Warmest
                heatType = 5;
            return heatType;
        }

        private static int GetMoistureType(float moistureValue)
        {
            int moistureType;
            if (moistureValue < DryerValue) // Dryest
                moistureType = 0;
            else if (moistureValue < DryValue) // Dryer
                moistureType = 1;
            else if (moistureValue < WetValue) // Dry
                moistureType = 2;
            else if (moistureValue < WetterValue) // Wet
                moistureType = 3;
            else if (moistureValue < WettestValue) // Wetter
                moistureType = 4;
            else // Wettest
                moistureType = 5;
            return moistureType;
        }

        public enum HeatType
        {
            Coldest,
            Colder,
            Cold,
            Warm,
            Warmer,
            Warmest
        }

        public enum MoistureType
        {
            Dryest,
            Dryer,
            Dry,
            Wet,
            Wetter,
            Wettest
        }
    }
}
