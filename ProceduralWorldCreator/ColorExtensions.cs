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

            int adjR = (int)((highColor.R - lowColor.R) * ratio * 255);
            int adjG = (int)((highColor.G - lowColor.G) * ratio * 255);
            int adjB = (int)((highColor.B - lowColor.B) * ratio * 255);

            return Color.FromArgb(adjR, adjG, adjB);
        }
    }
}
