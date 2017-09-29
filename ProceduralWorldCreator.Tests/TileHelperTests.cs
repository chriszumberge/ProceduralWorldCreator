using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProceduralWorldCreator;
using System.Collections.Generic;

namespace ProceduralWorldCreator.Tests
{
    [TestClass]
    public class TileHelperTests
    {
        [TestMethod]
        public void TestGetNeighbors_Centered()
        {
            // Arrange
            int maxX = 3;
            int maxY = 3;
            Tile[,] tiles = new Tile[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var tile = new Tile
                    {
                        X = x,
                        Y = y
                    };
                    tiles[x, y] = tile;
                }
            }

            // Act
            List<Tile> neighbors = TileHelpers.GetSurrounding(tiles, maxX, maxY, tiles[1, 1]);

            // Assert
            Assert.AreEqual(8, neighbors.Count);
        }

        [TestMethod]
        public void TestGetNeighborsWraps_PositiveX()
        {
            // Arrange
            int maxX = 3;
            int maxY = 3;
            Tile[,] tiles = new Tile[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var tile = new Tile
                    {
                        X = x,
                        Y = y
                    };
                    tiles[x, y] = tile;
                }
            }

            // Act
            List<Tile> neighbors = TileHelpers.GetSurrounding(tiles, maxX, maxY, tiles[2, 1]);

            // Assert
            Assert.AreEqual(8, neighbors.Count);
        }

        [TestMethod]
        public void TestGetNeighborsWraps_NegativeX()
        {
            // Arrange
            int maxX = 3;
            int maxY = 3;
            Tile[,] tiles = new Tile[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var tile = new Tile
                    {
                        X = x,
                        Y = y
                    };
                    tiles[x, y] = tile;
                }
            }

            // Act
            List<Tile> neighbors = TileHelpers.GetSurrounding(tiles, maxX, maxY, tiles[0, 1]);

            // Assert
            Assert.AreEqual(8, neighbors.Count);
        }

        [TestMethod]
        public void TestGetNeighbors_DoesNotWrap_PositiveY()
        {
            // Arrange
            int maxX = 3;
            int maxY = 3;
            Tile[,] tiles = new Tile[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var tile = new Tile
                    {
                        X = x,
                        Y = y
                    };
                    tiles[x, y] = tile;
                }
            }

            // Act
            List<Tile> neighbors = TileHelpers.GetSurrounding(tiles, maxX, maxY, tiles[1, 2]);

            // Assert
            Assert.AreEqual(8, neighbors.Count);
        }

        [TestMethod]
        public void TestGetNeighbors_DoesNotWrap_NegativeY()
        {
            // Arrange
            int maxX = 3;
            int maxY = 3;
            Tile[,] tiles = new Tile[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var tile = new Tile
                    {
                        X = x,
                        Y = y
                    };
                    tiles[x, y] = tile;
                }
            }

            // Act
            List<Tile> neighbors = TileHelpers.GetSurrounding(tiles, maxX, maxY, tiles[1, 0]);

            // Assert
            Assert.AreEqual(8, neighbors.Count);
        }
    }
}
