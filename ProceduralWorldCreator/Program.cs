using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            //RunRealWorldCreator();

            //RunNoiseWorldCreator();

            CreateSystem();


        }

        private static void RunNoiseWorldCreator()
        {
            Stopwatch watch = new Stopwatch();
            var creator = new NoiseWorldCreator()
            {
                Seed = 256,
                Height = 256,
                Width = 256,
                // Determined by world moisture?
                SeaLevel = 0.45f,
                // Determined by world age?
                TerrainFrequency = 1.25, // smaller number means more continents, larger number means lots of clustered islands
                TerrainOctaves = 16, // defines the "jaggedness" of the land.. might have to be proportionate to size
                WorldMoisture = WorldMoisture.Moderate,
                WorldTemperature = WorldTemperature.Average
            };

            string worldId = Guid.NewGuid().ToString();

            watch.Start();
            creator.CreateWorld();
            watch.Stop();
            Console.WriteLine(watch.Elapsed);

            Bitmap coloredHeightMap = BitmapGenerator.GetColoredHeightMap(creator.Width, creator.Height, creator.MapData);
            //coloredHeightMap.Save(String.Concat(worldId, "_heightMap.png"));
            BitmapHelpers.ShowBitmap(coloredHeightMap);

            Bitmap heatMap = BitmapGenerator.GetHeatMap(creator.Width, creator.Height, creator.MapData.Tiles);
            //heatMap.Save(String.Concat(worldId, "_heatMap.png"));
            BitmapHelpers.ShowBitmap(heatMap);

            Bitmap moistureMap = BitmapGenerator.GetMoistureMap(creator.Width, creator.Height, creator.MapData.Tiles);
            //moistureMap.Save(String.Concat(worldId, "_moistureMap.png"));
            BitmapHelpers.ShowBitmap(moistureMap);

            Bitmap riverMap = BitmapGenerator.GetRiverMap(creator.Width, creator.Height, creator.MapData.Tiles, creator.MapData.SeaLevel);
            //riverMap.Save(String.Concat(worldId, "_riverMap.png"));
            BitmapHelpers.ShowBitmap(riverMap);

            Bitmap biomeMap = BitmapGenerator.GetBiomeMap(creator.Width, creator.Height, creator.MapData.Tiles, creator.MapData.SeaLevel);
            //biomeMap.Save(String.Concat(worldId, "_biomeMap.png"));
            BitmapHelpers.ShowBitmap(biomeMap);

            var ids = creator.MapData.Tiles.AsList().Select(x => x.PlateId).Distinct().ToList();
        }

        private static void CreateSystem()
        {
            int numWorlds = 7;

            string systemId = Guid.NewGuid().ToString().Substring(0, 4);
            string root = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(Path.Combine(root, $"System_{systemId}"));

            Random random = new Random();
            Stopwatch watch = new Stopwatch();

            for (int i = 0; i < numWorlds; i++)
            {
                string worldId = Guid.NewGuid().ToString().Substring(0, 6);
                var seaLevel = random.NextDouble() * 0.4 + random.NextDouble() * 0.5 + 0.05; // min 0.05, max 0.95, focus of 0.5
                var terrainFrequency = random.NextDouble() + random.NextDouble() + 0.5; // min 0.5, max 2.5, focus of 1.5
                var terrainOctaves = (int)Math.Floor(random.NextDouble() * 5 + random.NextDouble() * 7 + 5); // min 5, max 17, focus of 11

                var worldMoisture = (WorldMoisture)random.Next(0, 5);
                var worldTemperature = (WorldTemperature)random.Next(0, 5);

                Directory.CreateDirectory(Path.Combine(root, $"System_{systemId}", $"Planet_{worldId}"));

                var creator = new NoiseWorldCreator()
                {
                    Height = 256,
                    Width = 256,
                    // Determined by world moisture?
                    SeaLevel = (float)seaLevel,
                    // Determined by world age?
                    TerrainFrequency = terrainFrequency, // smaller number means more continents, larger number means lots of clustered islands
                    TerrainOctaves = terrainOctaves, // defines the "jaggedness" of the land.. might have to be proportionate to size
                    WorldMoisture = worldMoisture,
                    WorldTemperature = worldTemperature
                };

                watch.Start();
                creator.CreateWorld();
                watch.Stop();

                Bitmap coloredHeightMap = BitmapGenerator.GetColoredHeightMap(creator.Width, creator.Height, creator.MapData);
                coloredHeightMap.Save(Path.Combine(root, $"System_{systemId}", $"Planet_{worldId}", String.Concat($"Planet_{worldId}", "_height_map.png")));

                Bitmap heatMap = BitmapGenerator.GetHeatMap(creator.Width, creator.Height, creator.MapData.Tiles);
                heatMap.Save(Path.Combine(root, $"System_{systemId}", $"Planet_{worldId}", String.Concat($"Planet_{worldId}", "_heat_map.png")));

                Bitmap moistureMap = BitmapGenerator.GetMoistureMap(creator.Width, creator.Height, creator.MapData.Tiles);
                moistureMap.Save(Path.Combine(root, $"System_{systemId}", $"Planet_{worldId}", String.Concat($"Planet_{worldId}", "_moisture_map.png")));

                Bitmap riverMap = BitmapGenerator.GetRiverMap(creator.Width, creator.Height, creator.MapData.Tiles, creator.MapData.SeaLevel);
                riverMap.Save(Path.Combine(root, $"System_{systemId}", $"Planet_{worldId}", String.Concat($"Planet_{worldId}", "_river_map.png")));

                Bitmap biomeMap = BitmapGenerator.GetBiomeMap(creator.Width, creator.Height, creator.MapData.Tiles, creator.MapData.SeaLevel);
                biomeMap.Save(Path.Combine(root, $"System_{systemId}", $"Planet_{worldId}", String.Concat($"Planet_{worldId}", "_biome_map.png")));

                using (var writer = new StreamWriter(Path.Combine(root, $"System_{systemId}", $"Planet_{worldId}", "World Parameters.txt")))
                {
                    writer.WriteLine($"Planet {worldId}");
                    writer.WriteLine();
                    writer.WriteLine("--- Randomly Generated Parameters ---");
                    writer.WriteLine($"Sea Level: {seaLevel}");
                    writer.WriteLine($"Terrain Frequency: {terrainFrequency}");
                    writer.WriteLine($"Terrain Octaves: {terrainOctaves}");
                    writer.WriteLine($"World Temperature: {Enum.GetName(typeof(WorldTemperature), worldTemperature)}");
                    writer.WriteLine($"World Moisture: {Enum.GetName(typeof(WorldMoisture), worldMoisture)}");
                    writer.WriteLine();
                    writer.WriteLine($"Generation Time: {watch.Elapsed}");
                }
                watch.Reset();
            }
        }

        private static void RunRealWorldCreator()
        {
            var creator = new RealWorldCreator()
            {
                Height = 100,
                Width = 100,
                NumberOfPlates = 12
            };

            Stopwatch watch = new Stopwatch();
            watch.Start();
            creator.CreateWorld();

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            Bitmap tectonicMap = BitmapGenerator.GetTectonicMap(creator.Width, creator.Height, creator.MapData.Tiles);
            BitmapHelpers.ShowBitmap(tectonicMap);

            Bitmap heightMap = BitmapGenerator.GetHeightMap(creator.Width, creator.Height, creator.MapData.Tiles);
            BitmapHelpers.ShowBitmap(heightMap);

            Bitmap landMap = BitmapGenerator.GetLandMap(creator.Width, creator.Height, creator.MapData);
            BitmapHelpers.ShowBitmap(landMap);
        }

    }
}
