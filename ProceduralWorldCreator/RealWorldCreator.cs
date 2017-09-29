using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class RealWorldCreator : WorldCreator
    {
        int? _numberOfPlates = null;
        // Allows the user to define the number of plates at object creation, else it's generated on first get
        public int NumberOfPlates
        {
            get
            {
                if (!_numberOfPlates.HasValue)
                {
                    int numPlates = Width * Height / 4000;
                    _numberOfPlates = numPlates < 4 ? 4 : numPlates;
                }
                return _numberOfPlates.Value;
            }
            set { _numberOfPlates = value; }
        }

        public RealWorldCreator() { }

        public override void CreateWorld()
        {
            mapData = new MapData(Width, Height);
            mapData.SeaLevel = SeaLevel;

            InitializeMapData();

            InitializeTectonicPlates();

            AssignInitialPlateProperties();

            EvaluateTectonics();
        }

        void InitializeMapData()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = new Tile();
                    tile.X = x;
                    tile.Y = y;
                    MapData.Tiles[x, y] = tile;
                }
            }
        }

        void InitializeTectonicPlates()
        {
            // TODO ensure plate can't be chosen twice
            for (int i = 0; i < NumberOfPlates; i++)
            {
                int randomX = random.Next(Width);
                int randomY = random.Next(Height);
                mapData.Tiles[randomX, randomY].PlateId = i;
                mapData.Tiles[randomX, randomY].IsPlateCenter = true;
            }

            List<Tile> plateEdgeTiles = mapData.Tiles.AsList().Where(t => t.PlateId.HasValue).ToList();
            List<Tile> nextPlateEdgeTiles = new List<Tile>();

            while (plateEdgeTiles.Count > 0)
            {
                foreach (Tile plateEdgeTile in plateEdgeTiles)
                {
                    if (plateEdgeTile.PlateId.HasValue)
                    {
                        //int floodAmount = random.Next(1, random.Next(2, 5));
                        //int floodAmount = 1;
                        int floodAmount = random.Next(2) + 1;

                        List<Tile> nextLocalEdgeTiles = new List<Tile>();
                        List<Tile> localEdgeTiles = new List<Tile> { plateEdgeTile };

                        for (int i = 0; i < floodAmount; i++)
                        {
                            foreach (Tile localEdgeTile in localEdgeTiles)
                            {
                                List<Tile> neighbors = TileHelpers.GetNeighbors(mapData.Tiles, Width, Height, localEdgeTile);
                                //List<Tile> neighbors = TileHelpers.GetSurrounding(mapData.Tiles, Width, Height, localEdgeTile);
                                foreach (Tile neighbor in neighbors.Where(x => !x.PlateId.HasValue))
                                {
                                    if (!neighbor.PlateId.HasValue)
                                    {
                                        neighbor.PlateId = localEdgeTile.PlateId.Value;
                                        nextLocalEdgeTiles.Add(neighbor);
                                    }
                                }
                            }
                            localEdgeTiles.Clear();
                            localEdgeTiles.AddRange(nextLocalEdgeTiles);
                            nextLocalEdgeTiles.Clear();
                        }
                        nextPlateEdgeTiles.AddRange(localEdgeTiles);
                    }
                }
                plateEdgeTiles.Clear();
                plateEdgeTiles.AddRange(nextPlateEdgeTiles);
                nextPlateEdgeTiles.Clear();
            }
            // DEBUG TEST, should be 0
            Console.WriteLine(mapData.Tiles.AsList().Count(x => !x.PlateId.HasValue));

            // Assign border tiles
            // TODO smartly combine this with above so we don't have to iterate full list again?
            foreach (Tile tile in mapData.Tiles.AsList())
            {
                List<Tile> neighbors = TileHelpers.GetNeighbors(mapData.Tiles, Width, Height, tile);

                if (neighbors.Any(n => n.PlateId != tile.PlateId))
                {
                    tile.IsPlateBorder = true;
                }
            }
        }

        void AssignInitialPlateProperties()
        {
            int northWorldAxisX = random.Next(Width);
            int northWorldAxisY = random.Next(Height / 2);
            int southWorldAxisY = (Height / 2) + northWorldAxisY;
            int southWorldAxisX = Width - northWorldAxisX;
            if (southWorldAxisX < 0) southWorldAxisX = Width - southWorldAxisX;
            if (southWorldAxisX >= Width) southWorldAxisX = southWorldAxisX - Width;

            Point northPole = new Point(northWorldAxisX, northWorldAxisY);
            Point southPole = new Point(southWorldAxisX, southWorldAxisY);
            Vector2 equatorPointer = ((southPole - northPole) / 2);
            Point equatorPoint = new Point(equatorPointer.X, equatorPointer.Y);

            float equatorSlopeY1 = MathHelpers.Solve_Y_ForLineBetweenTwoPoints(northPole.X, northPole.Y, southPole.X, southPole.Y, 0);
            float equatorSlopeY2 = MathHelpers.Solve_Y_ForLineBetweenTwoPoints(northPole.X, northPole.Y, southPole.X, southPole.Y, 1);
            float equatorSlope = (equatorSlopeY2 - equatorSlopeY1) / (1 - 0);

            //float distanceBetweenPoles = (float)Math.Sqrt(Math.Pow((northWorldAxisX - southWorldAxisX), 2) + Math.Pow((northWorldAxisY - southWorldAxisY), 2));


            for (int i = 0; i < NumberOfPlates; i++)
            {
                // Between -2 and 2 by generating a number >= 0 && < 5, then subtracting 2
                //int plateHeight = random.Next(5) - 2;
                //float plateHeight = random.Next(5) / 4.0f;
                float plateHeight = (float)random.NextDouble();

                float plateSpinSpeedIncrement = (float)random.NextDouble() / 10;
                bool plateSpinClockwise = random.Next(0, 2) == 1 ? true : false;

                var plateTiles = mapData.Tiles.AsList().Where(t => t.PlateId == i);

                var plateCenter = plateTiles.First(x => x.IsPlateCenter);
                Point plateCenterPoint = new Point(plateCenter.X, plateCenter.Y);

                foreach (Tile tile in plateTiles)
                {
                    // set height
                    tile.PlateHeight = (float)plateHeight;
                    tile.HeightValue = (float)plateHeight;

                    Point tilePoint = new Point(tile.X, tile.Y);

                    // TODO, this needs to be changed to use spherical coordinates
                    // set Global Drift

                    // TODO, real maths might transform the axis.. normal line from pole to equator, rotate so it's straight, then project the tile to that line,
                    //       y value compared to distance of line would tell gradient

                    // Find closest pole, then find distance between closest pole and equator
                    float distToNorth = Math.Abs(MathHelpers.GetDistanceBetweenTwoPoints(northWorldAxisX, northWorldAxisY, tile.X, tile.Y));
                    float distToSouth = Math.Abs(MathHelpers.GetDistanceBetweenTwoPoints(southWorldAxisX, southWorldAxisY, tile.X, tile.Y));

                    Point referencePole = (distToNorth < distToSouth) ? northPole : southPole;

                    // Lienar gradient for Global Drift from pole to equator.. dist will be 0 - 1 so just use that
                    Point projectedPt = MathHelpers.Project(referencePole, equatorPoint, new Point(tile.X, tile.Y));
                    float distToEquator = (projectedPt - referencePole).Magnitude / (equatorPoint - referencePole).Magnitude;

                    tile.GlobalDriftStrength = distToEquator;

                    // Get vector parallel to equator for direction.. rotation will be clockwise around axis, so x - 1 is next point
                    //Point nextTilePoint = new Point((1 / equatorSlope) + tilePoint.X, tilePoint.Y + 1);
                    float nextTilePointX = tilePoint.X - 1;
                    float nextTilePointY = equatorSlope * (nextTilePointX - tilePoint.X) + tilePoint.Y;
                    Point nextTilePoint = new Point(nextTilePointX, nextTilePointY);
                    tile.GlobalDriftDirection = (nextTilePoint - tilePoint).UnitVector;

                    tile.PlateRotationStrength = plateSpinSpeedIncrement * MathHelpers.GetDistanceBetweenTwoPoints(plateCenterPoint, tilePoint);
                    Vector2 vectorFromCenterToPoint = tilePoint - plateCenterPoint;

                    Vector2 perpendicularVector = new Vector2(vectorFromCenterToPoint.Second, vectorFromCenterToPoint.First * -1);
                    if (!plateSpinClockwise)
                    {
                        perpendicularVector = perpendicularVector * -1;
                    }

                    tile.PlateRotationDirection = perpendicularVector.UnitVector;
                }
            }
        }

        void EvaluateTectonics()
        {
            // TEST EXAMPLES
            // PUSHING
            //mapData.Tiles = new Tile[1, 2]
            //{
            //    {
            //        new Tile { X = 0, Y = 0, PlateId = 0, GlobalDriftDirection = Vector2.Left, GlobalDriftStrength = 0.25f, PlateRotationDirection = Vector2.Down, PlateRotationStrength = 1.0f, IsPlateBorder = true},
            //        new Tile { X = 0, Y = 1, PlateId = 1, GlobalDriftDirection = Vector2.Left, GlobalDriftStrength = 0.25f, PlateRotationDirection = Vector2.Up, PlateRotationStrength = 1.0f, IsPlateBorder = true}
            //    },
            //};
            // SEPARATING
            //mapData.Tiles = new Tile[1, 2]
            //{
            //    {
            //        new Tile { X = 0, Y = 0, PlateId = 0, GlobalDriftDirection = Vector2.Left, GlobalDriftStrength = 0.25f, PlateRotationDirection = Vector2.Up, PlateRotationStrength = 1.0f, IsPlateBorder = true},
            //        new Tile { X = 0, Y = 1, PlateId = 1, GlobalDriftDirection = Vector2.Left, GlobalDriftStrength = 0.25f, PlateRotationDirection = Vector2.Down, PlateRotationStrength = 1.0f, IsPlateBorder = true}
            //    },
            //};
            // SHEARING
            //mapData.Tiles = new Tile[1, 2]
            //{
            //    {
            //        new Tile { X = 0, Y = 0, PlateId = 0, GlobalDriftDirection = Vector2.Left, GlobalDriftStrength = 0.25f, PlateRotationDirection = Vector2.Left, PlateRotationStrength = 1.0f, IsPlateBorder = true},
            //        new Tile { X = 0, Y = 1, PlateId = 1, GlobalDriftDirection = Vector2.Left, GlobalDriftStrength = 0.25f, PlateRotationDirection = Vector2.Right, PlateRotationStrength = 1.0f, IsPlateBorder = true}
            //    },
            //};


            // Calculate Pressure
            List<Tile> boundaryTiles = mapData.Tiles.AsList().Where(x => x.IsPlateBorder).ToList();

            foreach (Tile boundaryTile in boundaryTiles)
            {
                var neighbors = TileHelpers.GetNeighbors(mapData.Tiles, Width, Height, boundaryTile).Where(t => t.PlateId != boundaryTile.PlateId);

                foreach (Tile neighbor in neighbors)
                {
                    // if boundary relative movement plus neighbor relative movement is less than boundary's original, then they're acting on eachother..
                    // otherwise they're moving the same direction.. can't compare magnitude since that doesn't take direction into account
                    Vector2 added = boundaryTile.RelativeMovement.UnitVector + neighbor.RelativeMovement.UnitVector;
                    if (added.X < boundaryTile.RelativeMovement.UnitVector.X || added.Y < boundaryTile.RelativeMovement.UnitVector.Y)
                    {
                        Vector2 unitToNeighbor = (new Point(boundaryTile.X, boundaryTile.Y) - new Point(neighbor.X, neighbor.Y)).UnitVector;

                        // if vector to neighbor aligns with pressure vector, they're pushing against eachother, otherwise separating
                        float directionalMagnitude = (unitToNeighbor + boundaryTile.RelativeMovement).Magnitude;
                        
                        if (directionalMagnitude > 1)
                        {
                            boundaryTile.Pressure += (boundaryTile.RelativeMovement + neighbor.RelativeMovement).Magnitude;
                        }
                        else
                        {
                            boundaryTile.Pressure -= (boundaryTile.RelativeMovement + neighbor.RelativeMovement).Magnitude;
                        }
                    }
                }
                // Capping effect
                if (boundaryTile.Pressure > 10) boundaryTile.Pressure = 10;
                if (boundaryTile.Pressure < -10) boundaryTile.Pressure = -10;

                // TEMPORARY FOR OUTPUT..  normalize pressure to height values
                //boundaryTile.HeightValue = ((boundaryTile.Pressure + 10) / 20);
                //boundaryTile.HeightValue = (((boundaryTile.Pressure + 10) / 10) - 0.5f);
            }

            //List<float> pressures = boundaryTiles.Select(x => x.Pressure).ToList();
            //List<float> sortedPressures = pressures.OrderBy(x => x).ToList();

            //List<float> bottomHalf = sortedPressures.GetRange(0, sortedPressures.Count / 2);
            //List<float> topHalf = sortedPressures.GetRange(sortedPressures.Count / 2, sortedPressures.Count - 1 - (sortedPressures.Count / 2));

            //List<float> First = bottomHalf.GetRange(0, bottomHalf.Count / 2);
            //List<float> Second = bottomHalf.GetRange(bottomHalf.Count / 2, bottomHalf.Count - 1 - (bottomHalf.Count / 2));
            //List<float> Third = topHalf.GetRange(0, topHalf.Count / 2);
            //List<float> Fourth = topHalf.GetRange(topHalf.Count / 2, topHalf.Count - 1 - (topHalf.Count / 2));

            //float maxPressure = pressures.Max();
            //float minPressure = pressures.Min();
            //float averagePressure = pressures.Average();
        }
    }
}
