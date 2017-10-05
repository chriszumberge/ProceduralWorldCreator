using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class Tile
    {
        public int X, Y;

        public Tile Left;
        public Tile Right;
        public Tile Top;
        public Tile Bottom;
        public bool IsCollidable;
        public bool FloodFilled;


        public bool IsPlateCenter { get; set; } = false;
        public int? PlateId { get; set; }
        public bool IsPlateBorder { get; set; } = false;
        public float PlateHeight { get; set; }
        public float GlobalDriftStrength { get; set; } = 0;
        public Vector2 GlobalDriftDirection { get; set; } = Vector2.Zero;

        public float PlateRotationStrength { get; set; } = 0;
        public Vector2 PlateRotationDirection { get; set; } = Vector2.Zero;

        // Directions should already be unit vectors, meaning relative moment will have a start point of 0, 0
        public Vector2 RelativeMovement => (PlateRotationDirection * PlateRotationStrength) + (GlobalDriftDirection * GlobalDriftStrength);
        public float Pressure { get; set; } = 0;


        public float HeightValue { get; set; } = 0;

        public float HeatValue { get; set; } = 0;

        public float MoistureValue { get; set; } = 0;

        public List<River> Rivers = new List<River>();
        public int RiverSize { get; set; }
        public bool IsRiver { get; set; } = false;

        public Tile()
        {

        }

        public int GetRiverNeighborCount(River river)
        {
            int count = 0;
            if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
                count++;
            if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
                count++;
            if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
                count++;
            if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
                count++;
            return count;
        }

        public Direction GetLowestNeighbor()
        {
            if (Left.HeightValue < Right.HeightValue && Left.HeightValue < Top.HeightValue && Left.HeightValue < Bottom.HeightValue)
                return Direction.Left;
            else if (Right.HeightValue < Left.HeightValue && Right.HeightValue < Top.HeightValue && Right.HeightValue < Bottom.HeightValue)
                return Direction.Right;
            else if (Top.HeightValue < Left.HeightValue && Top.HeightValue < Right.HeightValue && Top.HeightValue < Bottom.HeightValue)
                return Direction.Right;
            else if (Bottom.HeightValue < Left.HeightValue && Bottom.HeightValue < Top.HeightValue && Bottom.HeightValue < Right.HeightValue)
                return Direction.Right;
            else
                return Direction.Bottom;
        }

        public void SetRiverPath(River river)
        {
            if (!IsCollidable)
                return;

            if (!Rivers.Contains(river))
            {
                Rivers.Add(river);
            }
        }

        private void SetRiverTile(River river)
        {
            SetRiverPath(river);
            //HeightType = HeightType.River;
            IsRiver = true;
            //HeightValue = 0;
            IsCollidable = false;
        }

        public void DigRiver(River river, int size)
        {
            SetRiverTile(river);
            RiverSize = size;

            if (size == 1)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
            }

            if (size == 2)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
                Top.SetRiverTile(river);
                Top.Left.SetRiverTile(river);
                Top.Right.SetRiverTile(river);
                Left.SetRiverTile(river);
                Left.Bottom.SetRiverTile(river);
            }

            if (size == 3)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
                Top.SetRiverTile(river);
                Top.Left.SetRiverTile(river);
                Top.Right.SetRiverTile(river);
                Left.SetRiverTile(river);
                Left.Bottom.SetRiverTile(river);
                Right.Right.SetRiverTile(river);
                Right.Right.Bottom.SetRiverTile(river);
                Bottom.Bottom.SetRiverTile(river);
                Bottom.Bottom.Right.SetRiverTile(river);
            }

            if (size == 4)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
                Top.SetRiverTile(river);
                Top.Right.SetRiverTile(river);
                Left.SetRiverTile(river);
                Left.Bottom.SetRiverTile(river);
                Right.Right.SetRiverTile(river);
                Right.Right.Bottom.SetRiverTile(river);
                Bottom.Bottom.SetRiverTile(river);
                Bottom.Bottom.Right.SetRiverTile(river);
                Left.Bottom.Bottom.SetRiverTile(river);
                Left.Left.Bottom.SetRiverTile(river);
                Left.Left.SetRiverTile(river);
                Left.Left.Top.SetRiverTile(river);
                Left.Top.SetRiverTile(river);
                Left.Top.Top.SetRiverTile(river);
                Top.Top.SetRiverTile(river);
                Top.Top.Right.SetRiverTile(river);
                Top.Right.Right.SetRiverTile(river);
            }
        }
    }
}
