using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public static class ColorHelpers
    {
        public static Color Lerp(Color lowColor, Color highColor, float value)
        {
            return Lerp(lowColor, highColor, 0.0f, 1.0f, value);
        }

        public static Color Lerp(Color lowColor, Color highColor, float lowValue, float highValue, float value)
        {
            float ratio = (value - lowValue) / (highValue - lowValue);

            int adjR = Math.Abs((int)((highColor.R - lowColor.R) * ratio));
            int adjG = Math.Abs((int)((highColor.G - lowColor.G) * ratio));
            int adjB = Math.Abs((int)((highColor.B - lowColor.B) * ratio));

            while (adjR > 255 || adjG > 255 || adjB > 255)
            {
                int max = adjR > adjG ? adjR > adjB ? adjR : adjB : adjG > adjB ? adjG : adjB;
                adjR /= max;
                adjG /= max;
                adjB /= max;
            }

            return Color.FromArgb(adjR, adjG, adjB);
        }
    }
}
