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

        public int HeatOctaves { get; set; } = 4;
        public double HeatFrequency { get; set; } = 3.0;

        public int MoistureOctaves { get; set; } = 4;
        public double MoistureFrequency { get; set; } = 3.0;

        public WorldTemperature WorldTemperature { get; set; } = WorldTemperature.Average;
        public WorldMoisture WorldMoisture { get; set; } = WorldMoisture.Moderate;

        public int? Seed { get; set; }

        public override void CreateWorld()
        {
            int seed;
            if (Seed.HasValue)
                seed = Seed.Value;
            else
                seed = random.Next(0, Int32.MaxValue);

            //int seed = 256;
            mapData = new MapData(Width, Height);

            // Let world moisture affect sea level
            if (WorldMoisture == WorldMoisture.VeryDry)
                SeaLevel /= 2;
            else if (WorldMoisture == WorldMoisture.Dry)
                SeaLevel /= 1.5f;
            else if (WorldMoisture == WorldMoisture.Moderate)
                SeaLevel /= 1;
            else if (WorldMoisture == WorldMoisture.Moist)
                SeaLevel *= 1.2f;
            else if (WorldMoisture == WorldMoisture.VeryDry)
                SeaLevel *= 1.5f;

            if (SeaLevel > 1) SeaLevel = 0.95f;
            mapData.SeaLevel = SeaLevel;

            // Update Heat and Moisture Octaves/Frequency
            MoistureOctaves = (int)(TerrainOctaves * 0.66f);
            HeatOctaves = (int)(TerrainOctaves * 0.66f);
            MoistureFrequency = TerrainFrequency;
            HeatFrequency = TerrainFrequency;


            float[,] heightMap, heatMap, moistureMap;
            float minHeight, maxHeight, minHeat, maxHeat, minMoisture, maxMoisture;

            ImplicitModuleBase HeightMap, HeatMap, MoistureMap;

            Initialize(seed, out HeightMap, out HeatMap, out MoistureMap);

            GetMapData(HeightMap, out heightMap, out minHeight, out maxHeight, HeatMap, out heatMap, out minHeat, out maxHeat,
                MoistureMap, out moistureMap, out minMoisture, out maxMoisture);

            LoadTiles(heightMap, minHeight, maxHeight, heatMap, minHeat, maxHeat, moistureMap, minMoisture, maxMoisture);

            UpdateNeighbors();

            GenerateRivers();
            BuildRiverGroups();
            DigRiverGroups();
            AdjustMoistureMap(moistureMap);

            //UpdateBitmasks();
            FloodFill();

        }

        private void Initialize(int seed, out ImplicitModuleBase HeightMap, out ImplicitModuleBase HeatMap, out ImplicitModuleBase MoistureMap)
        {
            FractalType randomFractalType = (FractalType)random.Next(0, 5);
            BasisType randomBasisType = (BasisType)random.Next(0, 4); // exclude white

            // HEIGHT
            // Multi, Billow, Fractional Brownian, Hybrid Multi, Ridged Multi
            // Simplex, Gradient, Gradient Value, Value
            //ImplicitFractal heightFractal = new ImplicitFractal(FractalType.Multi, BasisType.Simplex, InterpolationType.Quintic)
            ImplicitFractal heightFractal = new ImplicitFractal(randomFractalType, randomBasisType, InterpolationType.Quintic)
            {
                Octaves = TerrainOctaves,
                Frequency = TerrainFrequency,
                Seed = seed
            };
            HeightMap = heightFractal;

            // HEAT
            ImplicitGradient heatGradient = new ImplicitGradient(1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            ImplicitFractal heatFractal = new ImplicitFractal(randomFractalType, randomBasisType, InterpolationType.Quintic)
            {
                Octaves = HeatOctaves,
                Frequency = HeatFrequency,
                Seed = seed
            };
            HeatMap = new ImplicitCombiner(CombinerType.Multiply);
            ((ImplicitCombiner)HeatMap).AddSource(heatGradient);
            ((ImplicitCombiner)HeatMap).AddSource(heatFractal);

            // MOISTURE
            MoistureMap = new ImplicitFractal(randomFractalType, randomBasisType, InterpolationType.Quintic)
            {
                Octaves = MoistureOctaves,
                Frequency = MoistureFrequency,
                Seed = seed
            };
        }

        private void GetMapData(ImplicitModuleBase HeightMap, out float[,] heightMap, out float minHeight, out float maxHeight,
                                ImplicitModuleBase HeatMap, out float[,] heatMap, out float minHeat, out float maxHeat,
                                ImplicitModuleBase MoistureMap, out float[,] moistureMap, out float minMoisture, out float maxMoisture)
        {
            heightMap = new float[Width, Height];
            heatMap = new float[Width, Height];
            moistureMap = new float[Width, Height];
            minHeight = float.MaxValue;
            maxHeight = float.MinValue;
            minHeat = float.MaxValue;
            maxHeat = float.MinValue;
            minMoisture = float.MaxValue;
            maxMoisture = float.MinValue;
            // Get all the values to be used and track max/min to normalize when assigning to tiles
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    #region X-AXIS WRAPPING
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
                    #endregion

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
                    float heatValue = (float)HeatMap.Get(nx, ny, nz, nw);
                    float moistureValue = (float)MoistureMap.Get(nx, ny, nz, nw);

                    // keep track of the max and min values found
                    if (heightValue > maxHeight) maxHeight = heightValue;
                    if (heightValue < minHeight) minHeight = heightValue;

                    if (heatValue > maxHeat) maxHeat = heatValue;
                    if (heatValue < minHeat) minHeat = heatValue;

                    if (moistureValue > maxMoisture) maxMoisture = moistureValue;
                    if (moistureValue < minMoisture) minMoisture = moistureValue;

                    heightMap[x, y] = heightValue;
                    heatMap[x, y] = heatValue;
                    moistureMap[x, y] = moistureValue;
                }
            }
        }

        private void UpdateNeighbors()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Tile t = mapData.Tiles[x, y];

                    t.Top = GetTop(t);
                    t.Bottom = GetBottom(t);
                    t.Left = GetLeft(t);
                    t.Right = GetRight(t);
                }
            }
        }

        public int RiverCount { get; set; } = 40;
        public float MinRiverHeight { get; set; } = 0.6f;
        public int MaxRiverAttempts { get; set; } = 1000;
        public int MinRiverTurns { get; set; } = 18;
        public int MinRiverLength { get; set; } = 20;
        public int MaxRiverIntersections { get; set; } = 2;
        List<River> Rivers = new List<River>();
        List<RiverGroup> RiverGroups = new List<RiverGroup>();

        void GenerateRivers()
        {
            int attempts = 0;
            int rivercount = RiverCount;
            Rivers = new List<River>();

            // Generate some rivers
            while (rivercount > 0 && attempts < MaxRiverAttempts)
            {
                // get a random tile
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);
                Tile tile = mapData.Tiles[x, y];

                // validate the tile
                if (!tile.IsCollidable) continue;
                if (tile.Rivers.Count > 0) continue;

                if (tile.HeightValue > MinRiverHeight)
                {
                    // Tile is good to start river from
                    River river = new River(rivercount);

                    // Figure out the direction this river will try to flow
                    river.CurrentDirection = tile.GetLowestNeighbor();

                    // Recursively find a path to water
                    FindPathToWater(tile, river.CurrentDirection, ref river);

                    // Validate the generated river 
                    if (river.TurnCount < MinRiverTurns || river.Tiles.Count < MinRiverLength || river.Intersections > MaxRiverIntersections)
                    {
                        //Validation failed - remove this river
                        for (int i = 0; i < river.Tiles.Count; i++)
                        {
                            Tile t = river.Tiles[i];
                            t.Rivers.Remove(river);
                        }
                    }
                    else if (river.Tiles.Count >= MinRiverLength)
                    {
                        //Validation passed - Add river to list
                        Rivers.Add(river);
                        tile.Rivers.Add(river);
                        rivercount--;
                    }
                }
                attempts++;
            }
        }

        private void AddMoisture(float[,] moistureData, Tile t, int radius)
        {
            int startx = MathHelpers.Mod(t.X - radius, Width);
            int endx = MathHelpers.Mod(t.X + radius, Width);
            Vector2 center = new Vector2(t.X, t.Y);
            int curr = radius;

            while (curr > 0)
            {

                int x1 = MathHelpers.Mod(t.X - curr, Width);
                int x2 = MathHelpers.Mod(t.X + curr, Width);
                int y = t.Y;

                AddMoisture(moistureData, mapData.Tiles[x1, y], 0.025f / (center - new Vector2(x1, y)).Magnitude);

                for (int i = 0; i < curr; i++)
                {
                    AddMoisture(moistureData, mapData.Tiles[x1, MathHelpers.Mod(y + i + 1, Height)], 0.025f / (center - new Vector2(x1, MathHelpers.Mod(y + i + 1, Height))).Magnitude);
                    AddMoisture(moistureData, mapData.Tiles[x1, MathHelpers.Mod(y - (i + 1), Height)], 0.025f / (center - new Vector2(x1, MathHelpers.Mod(y - (i + 1), Height))).Magnitude);

                    AddMoisture(moistureData, mapData.Tiles[x2, MathHelpers.Mod(y + i + 1, Height)], 0.025f / (center - new Vector2(x2, MathHelpers.Mod(y + i + 1, Height))).Magnitude);
                    AddMoisture(moistureData, mapData.Tiles[x2, MathHelpers.Mod(y - (i + 1), Height)], 0.025f / (center - new Vector2(x2, MathHelpers.Mod(y - (i + 1), Height))).Magnitude);
                }
                curr--;
            }
        }

        private void AddMoisture(float[,] moistureData, Tile t, float amount)
        {
            moistureData[t.X, t.Y] += amount;
            t.MoistureValue += amount;
            if (t.MoistureValue > 1)
                t.MoistureValue = 1;
        }

        private void AdjustMoistureMap(float[,] moistureData)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {

                    Tile t = mapData.Tiles[x, y];

                    if (t.IsRiver)
                    {
                        AddMoisture(moistureData, t, (int)60);
                    }
                }
            }
        }

        private void DigRiverGroups()
        {
            for (int i = 0; i < RiverGroups.Count; i++)
            {

                RiverGroup group = RiverGroups[i];
                River longest = null;

                //Find longest river in this group
                for (int j = 0; j < group.Rivers.Count; j++)
                {
                    River river = group.Rivers[j];
                    if (longest == null)
                        longest = river;
                    else if (longest.Tiles.Count < river.Tiles.Count)
                        longest = river;
                }

                if (longest != null)
                {
                    //Dig out longest path first
                    DigRiver(longest);

                    for (int j = 0; j < group.Rivers.Count; j++)
                    {
                        River river = group.Rivers[j];
                        if (river != longest)
                        {
                            DigRiver(river, longest);
                        }
                    }
                }
            }
        }


        private void BuildRiverGroups()
        {
            //loop each tile, checking if it belongs to multiple rivers
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Tile t = mapData.Tiles[x, y];

                    if (t.Rivers.Count > 1)
                    {
                        // multiple rivers == intersection
                        RiverGroup group = null;

                        // Does a rivergroup already exist for this group?
                        for (int n = 0; n < t.Rivers.Count; n++)
                        {
                            River tileriver = t.Rivers[n];
                            for (int i = 0; i < RiverGroups.Count; i++)
                            {
                                for (int j = 0; j < RiverGroups[i].Rivers.Count; j++)
                                {
                                    River river = RiverGroups[i].Rivers[j];
                                    if (river.ID == tileriver.ID)
                                    {
                                        group = RiverGroups[i];
                                    }
                                    if (group != null) break;
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }

                        // existing group found -- add to it
                        if (group != null)
                        {
                            for (int n = 0; n < t.Rivers.Count; n++)
                            {
                                if (!group.Rivers.Contains(t.Rivers[n]))
                                    group.Rivers.Add(t.Rivers[n]);
                            }
                        }
                        else   //No existing group found - create a new one
                        {
                            group = new RiverGroup();
                            for (int n = 0; n < t.Rivers.Count; n++)
                            {
                                group.Rivers.Add(t.Rivers[n]);
                            }
                            RiverGroups.Add(group);
                        }
                    }
                }
            }
        }

        private void DigRiver(River river)
        {
            int counter = 0;

            // How wide are we digging this river?
            int size = random.Next(1, 5);
            river.Length = river.Tiles.Count;

            // randomize size change
            int two = river.Length / 2;
            int three = two / 2;
            int four = three / 2;
            int five = four / 2;

            int twomin = two / 3;
            int threemin = three / 3;
            int fourmin = four / 3;
            int fivemin = five / 3;

            // randomize lenght of each size
            int count1 = random.Next(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + random.Next(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + random.Next(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + random.Next(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                    else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                    else if (count3 > 0) { count3--; count4--; extra--; }
                    else if (count4 > 0) { count4--; extra--; }
                }
            }

            // Dig it out
            for (int i = river.Tiles.Count - 1; i >= 0; i--)
            {
                Tile t = river.Tiles[i];

                if (counter < count1)
                {
                    t.DigRiver(river, 4);
                }
                else if (counter < count2)
                {
                    t.DigRiver(river, 3);
                }
                else if (counter < count3)
                {
                    t.DigRiver(river, 2);
                }
                else if (counter < count4)
                {
                    t.DigRiver(river, 1);
                }
                else
                {
                    t.DigRiver(river, 0);
                }
                counter++;
            }
        }

        // Dig river based on a parent river vein
        private void DigRiver(River river, River parent)
        {
            int intersectionID = 0;
            int intersectionSize = 0;

            // determine point of intersection
            for (int i = 0; i < river.Tiles.Count; i++)
            {
                Tile t1 = river.Tiles[i];
                for (int j = 0; j < parent.Tiles.Count; j++)
                {
                    Tile t2 = parent.Tiles[j];
                    if (t1 == t2)
                    {
                        intersectionID = i;
                        intersectionSize = t2.RiverSize;
                    }
                }
            }

            int counter = 0;
            int intersectionCount = river.Tiles.Count - intersectionID;
            int size = random.Next(intersectionSize, 5);
            river.Length = river.Tiles.Count;

            // randomize size change
            int two = river.Length / 2;
            int three = two / 2;
            int four = three / 2;
            int five = four / 2;

            int twomin = two / 3;
            int threemin = three / 3;
            int fourmin = four / 3;
            int fivemin = five / 3;

            // randomize length of each size
            int count1 = random.Next(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + random.Next(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + random.Next(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + random.Next(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                    else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                    else if (count3 > 0) { count3--; count4--; extra--; }
                    else if (count4 > 0) { count4--; extra--; }
                }
            }

            // adjust size of river at intersection point
            if (intersectionSize == 1)
            {
                count4 = intersectionCount;
                count1 = 0;
                count2 = 0;
                count3 = 0;
            }
            else if (intersectionSize == 2)
            {
                count3 = intersectionCount;
                count1 = 0;
                count2 = 0;
            }
            else if (intersectionSize == 3)
            {
                count2 = intersectionCount;
                count1 = 0;
            }
            else if (intersectionSize == 4)
            {
                count1 = intersectionCount;
            }
            else
            {
                count1 = 0;
                count2 = 0;
                count3 = 0;
                count4 = 0;
            }

            // dig out the river
            for (int i = river.Tiles.Count - 1; i >= 0; i--)
            {

                Tile t = river.Tiles[i];

                if (counter < count1)
                {
                    t.DigRiver(river, 4);
                }
                else if (counter < count2)
                {
                    t.DigRiver(river, 3);
                }
                else if (counter < count3)
                {
                    t.DigRiver(river, 2);
                }
                else if (counter < count4)
                {
                    t.DigRiver(river, 1);
                }
                else
                {
                    t.DigRiver(river, 0);
                }
                counter++;
            }
        }

        private void FindPathToWater(Tile tile, Direction direction, ref River river)
        {
            if (tile.Rivers.Contains(river))
                return;

            // check if there is already a river on this tile
            if (tile.Rivers.Count > 0)
                river.Intersections++;

            river.AddTile(tile);

            // get neighbors
            Tile left = GetLeft(tile);
            Tile right = GetRight(tile);
            Tile top = GetTop(tile);
            Tile bottom = GetBottom(tile);

            float leftValue = int.MaxValue;
            float rightValue = int.MaxValue;
            float topValue = int.MaxValue;
            float bottomValue = int.MaxValue;

            // query height values of neighbors
            if (left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left))
                leftValue = left.HeightValue;
            if (right.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right))
                rightValue = right.HeightValue;
            if (top.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top))
                topValue = top.HeightValue;
            if (bottom.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom))
                bottomValue = bottom.HeightValue;

            // if neighbor is existing river that is not this one, flow into it
            if (bottom.Rivers.Count == 0 && !bottom.IsCollidable)
                bottomValue = 0;
            if (top.Rivers.Count == 0 && !top.IsCollidable)
                topValue = 0;
            if (left.Rivers.Count == 0 && !left.IsCollidable)
                leftValue = 0;
            if (right.Rivers.Count == 0 && !right.IsCollidable)
                rightValue = 0;

            // override flow direction if a tile is significantly lower
            if (direction == Direction.Left)
                if (Math.Abs(rightValue - leftValue) < 0.1f)
                    rightValue = int.MaxValue;
            if (direction == Direction.Right)
                if (Math.Abs(rightValue - leftValue) < 0.1f)
                    leftValue = int.MaxValue;
            if (direction == Direction.Top)
                if (Math.Abs(topValue - bottomValue) < 0.1f)
                    bottomValue = int.MaxValue;
            if (direction == Direction.Bottom)
                if (Math.Abs(topValue - bottomValue) < 0.1f)
                    topValue = int.MaxValue;

            // find mininum
            float min = (float)Math.Min(Math.Min(Math.Min(leftValue, rightValue), topValue), bottomValue);

            // if no minimum found - exit
            if (min == int.MaxValue)
                return;

            //Move to next neighbor
            if (min == leftValue)
            {
                if (left.IsCollidable)
                {
                    if (river.CurrentDirection != Direction.Left)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Left;
                    }
                    FindPathToWater(left, direction, ref river);
                }
            }
            else if (min == rightValue)
            {
                if (right.IsCollidable)
                {
                    if (river.CurrentDirection != Direction.Right)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Right;
                    }
                    FindPathToWater(right, direction, ref river);
                }
            }
            else if (min == bottomValue)
            {
                if (bottom.IsCollidable)
                {
                    if (river.CurrentDirection != Direction.Bottom)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Bottom;
                    }
                    FindPathToWater(bottom, direction, ref river);
                }
            }
            else if (min == topValue)
            {
                if (top.IsCollidable)
                {
                    if (river.CurrentDirection != Direction.Top)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Top;
                    }
                    FindPathToWater(top, direction, ref river);
                }
            }
        }

        List<TileGroup> Waters = new List<TileGroup>();
        List<TileGroup> Lands = new List<TileGroup>();

        private void FloodFill()
        {
            // Use a stack instead of recursion
            Stack<Tile> stack = new Stack<Tile>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    Tile t = mapData.Tiles[x, y];

                    //Tile already flood filled, skip
                    if (t.FloodFilled) continue;

                    // Land
                    if (t.IsCollidable)
                    {
                        TileGroup group = new TileGroup();
                        group.Type = TileGroupType.Land;
                        stack.Push(t);

                        while (stack.Count > 0)
                        {
                            FloodFill(stack.Pop(), ref group, ref stack);
                        }

                        if (group.Tiles.Count > 0)
                            Lands.Add(group);
                    }
                    // Water
                    else
                    {
                        TileGroup group = new TileGroup();
                        group.Type = TileGroupType.Water;
                        stack.Push(t);

                        while (stack.Count > 0)
                        {
                            FloodFill(stack.Pop(), ref group, ref stack);
                        }

                        if (group.Tiles.Count > 0)
                            Waters.Add(group);
                    }
                }
            }
        }


        private void FloodFill(Tile tile, ref TileGroup tiles, ref Stack<Tile> stack)
        {
            // Validate
            if (tile.FloodFilled)
                return;
            if (tiles.Type == TileGroupType.Land && !tile.IsCollidable)
                return;
            if (tiles.Type == TileGroupType.Water && tile.IsCollidable)
                return;

            // Add to TileGroup
            tiles.Tiles.Add(tile);
            tile.FloodFilled = true;

            // floodfill into neighbors
            Tile t = GetTop(tile);
            if (!t.FloodFilled && tile.IsCollidable == t.IsCollidable)
                stack.Push(t);
            t = GetBottom(tile);
            if (!t.FloodFilled && tile.IsCollidable == t.IsCollidable)
                stack.Push(t);
            t = GetLeft(tile);
            if (!t.FloodFilled && tile.IsCollidable == t.IsCollidable)
                stack.Push(t);
            t = GetRight(tile);
            if (!t.FloodFilled && tile.IsCollidable == t.IsCollidable)
                stack.Push(t);
        }

        //private void UpdateBitmasks()
        //{
        //    for (var x = 0; x < Width; x++)
        //    {
        //        for (var y = 0; y < Height; y++)
        //        {
        //            mapData.Tiles[x, y].UpdateBitmask();
        //        }
        //    }
        //}

        private void LoadTiles(float[,] heightMap, float minHeight, float maxHeight,
                               float[,] heatMap, float minHeat, float maxHeat,
                               float[,] moistureMap, float minMoisture, float maxMoisture)
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

                    var sealevel = mapData.SeaLevel;

                    // HEIGHT
                    float heightValue = heightMap[x, y];
                    // Normalize the height value
                    heightValue = (heightValue - minHeight) / (maxHeight - minHeight);
                    tile.HeightValue = heightValue;
                    if (tile.HeightValue <= sealevel)
                    {
                        tile.IsCollidable = false;
                    }
                    else
                    {
                        tile.IsCollidable = true;
                    }

                    var landLevel = (1.0f - sealevel);
                    // MOISTURE
                    // Adjust Moisture Map based on Height
                    if (heightValue <= (sealevel / 2.0f))
                    {
                        moistureMap[tile.X, tile.Y] += 8f * tile.HeightValue;
                    }
                    else if (heightValue <= sealevel)
                    {
                        moistureMap[tile.X, tile.Y] += 3f * tile.HeightValue;
                    }
                    else if (heightValue <= (sealevel + (landLevel / 6)))
                    {
                        moistureMap[tile.X, tile.Y] += 1f * tile.HeightValue;
                    }
                    else if (heightValue <= (sealevel + (landLevel / 2)))
                    {
                        moistureMap[tile.X, tile.Y] += 0.2f * tile.HeightValue;
                    }
                    else if (heightValue <= (sealevel + (2 * landLevel / 3)))
                    {
                        moistureMap[tile.X, tile.Y] += 0f * tile.HeightValue;
                    }
                    else if (heightValue <= (sealevel + (5 * landLevel / 6)))
                    {
                        moistureMap[tile.X, tile.Y] -= 0.2f * tile.HeightValue;
                    }
                    else
                    {
                        moistureMap[tile.X, tile.Y] -= 0.4f * tile.HeightValue;
                    }

                    float moistureValue = moistureMap[x, y];
                    // Normalize the moisture value
                    moistureValue = (moistureValue - minMoisture) / (maxMoisture - minMoisture);

                    if (WorldMoisture == WorldMoisture.VeryDry)
                        moistureValue *= 0.33f;
                    else if (WorldMoisture == WorldMoisture.Dry)
                        moistureValue *= 0.66f;
                    else if (WorldMoisture == WorldMoisture.Moderate)
                        moistureValue *= 1.0f;
                    else if (WorldMoisture == WorldMoisture.Moist)
                        moistureValue = (moistureValue * 0.66f) + 0.33f;
                    else if (WorldMoisture == WorldMoisture.VeryMoist)
                        moistureValue = (moistureValue * 0.33f) + 0.66f;

                    tile.MoistureValue = moistureValue;

                    // HEAT
                    // Adjust Heat Map based on Height - Higher == colder
                    if (heightValue <= (sealevel + (landLevel / 2)))
                    {
                        heatMap[tile.X, tile.Y] += 0.01f * tile.HeightValue;
                    }
                    else if (heightValue <= (sealevel + (2 * landLevel / 3)))
                    {
                        heatMap[tile.X, tile.Y] -= 0.1f * tile.HeightValue;
                    }
                    else if (heightValue <= (sealevel + (5 * landLevel / 6)))
                    {
                        heatMap[tile.X, tile.Y] -= 0.25f * tile.HeightValue;
                    }
                    else
                    {
                        heatMap[tile.X, tile.Y] -= 0.4f * tile.HeightValue;
                    }

                    // Set Heat
                    float heatValue = heatMap[x, y];
                    // Normalize Heat Value
                    heatValue = (heatValue - minHeat) / (maxHeat - minHeat);

                    if (WorldTemperature == WorldTemperature.Frigid)
                        heatValue *= 0.33f;
                    else if (WorldTemperature == WorldTemperature.Cold)
                        heatValue *= 0.66f;
                    else if (WorldTemperature == WorldTemperature.Average)
                        heatValue *= 1.0f;
                    else if (WorldTemperature == WorldTemperature.Warm)
                        heatValue = (heatValue * 0.66f) + 0.33f;
                    else if (WorldTemperature == WorldTemperature.Hot)
                        heatValue = (heatValue * 0.33f) + 0.66f;

                    tile.HeatValue = heatValue;

                    // TODO assign biomes here?

                    mapData.Tiles[x, y] = tile;
                }
            }
        }

        private Tile GetTop(Tile t)
        {
            return mapData.Tiles[t.X, MathHelpers.Mod(t.Y - 1, Height)];
        }
        private Tile GetBottom(Tile t)
        {
            return mapData.Tiles[t.X, MathHelpers.Mod(t.Y + 1, Height)];
        }
        private Tile GetLeft(Tile t)
        {
            return mapData.Tiles[MathHelpers.Mod(t.X - 1, Width), t.Y];
        }
        private Tile GetRight(Tile t)
        {
            return mapData.Tiles[MathHelpers.Mod(t.X + 1, Width), t.Y];
        }
    }
}
