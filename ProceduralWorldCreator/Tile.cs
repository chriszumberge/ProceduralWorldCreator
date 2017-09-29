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

        public float HeightValue { get; set; }

        public Tile()
        {

        }
    }
}
