using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public static class TileHelpers
    {
        public static List<Tile> GetSurrounding(Tile[,] tiles, int width, int height, Tile tile)
        {
            int midX = tile.X;
            int midY = tile.Y;

            int minX = tile.X - 1;
            int maxX = tile.X + 1;
            if (minX < 0) minX = width - 1;
            if (maxX >= width) maxX = maxX - width;

            int minY = tile.Y - 1;
            int maxY = tile.Y + 1;
            // Non-wrapping Y
            //if (minY < 0) minY = 0;
            //if (maxY >= height) maxY = height - 1;
            // Wrapping Y
            if (minY < 0) minY = height - 1;
            if (maxY >= height) maxY = maxY - height;

            List<Tile> surrounding = tiles.AsList().Where(t => 
                (t.X == minX && t.Y == minY) || (t.X == midX && t.Y == minY) || (t.X == maxX && t.Y == minY) ||
                (t.X == minX && t.Y == midY) ||                                 (t.X == maxX && t.Y == midY) ||
                (t.X == minX && t.Y == maxY) || (t.X == midX && t.Y == maxY) || (t.X == maxX && t.Y == maxY)
                ).ToList();

            return surrounding;
        }

        public static List<Tile> GetNeighbors(Tile[,] tiles, int width, int height, Tile tile)
        {
            int midX = tile.X;
            int midY = tile.Y;

            int minX = tile.X - 1;
            int maxX = tile.X + 1;
            if (minX < 0) minX = width - 1;
            if (maxX >= width) maxX = maxX - width;

            int minY = tile.Y - 1;
            int maxY = tile.Y + 1;
            // Non-wrapping Y
            //if (minY < 0) minY = 0;
            //if (maxY >= height) maxY = height - 1;
            // Wrapping Y
            if (minY < 0) minY = height - 1;
            if (maxY >= height) maxY = maxY - height;

            //List<Tile> neighbors = tiles.AsList().Where(t => !(t == tile) &&
            //    (t.Y >= minY && t.Y <= maxY) &&
            //    (t.X >= minX && t.X <= maxX)).ToList();

            List<Tile> neighbors = tiles.AsList().Where(t =>
                                                (t.X == midX && t.Y == minY) ||
                (t.X == minX && t.Y == midY) ||                                 (t.X == maxX && t.Y == midY) ||
                                                (t.X == midX && t.Y == maxY) 
                ).ToList();

            return neighbors;
        }
    }
}
