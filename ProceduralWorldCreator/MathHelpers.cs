using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public static class MathHelpers
    {
        public static float Solve_Y_ForLineBetweenTwoPoints(float x1, float y1, float x2, float y2, float x)
        {
            double x1_sqrd = Math.Pow(x1, 2);
            double x2_sqrd = Math.Pow(x2, 2);
            double y1_sqrd = Math.Pow(y1, 2);
            double y2_sqrd = Math.Pow(y2, 2);

            double numerator = x2_sqrd + y2_sqrd - x1_sqrd - y1_sqrd + (((2 * x2) - (2 * x1)) * x);
            double denomenator = ((2 * y2) - (2 * y1));

            double y = numerator / denomenator;
            return (float)y;
        }

        public static float Solve_X_ForLineBetweenTwoPoints(float x1, float y1, float x2, float y2, float y)
        {
            double x1_sqrd = Math.Pow(x1, 2);
            double x2_sqrd = Math.Pow(x2, 2);
            double y1_sqrd = Math.Pow(y1, 2);
            double y2_sqrd = Math.Pow(y2, 2);

            double numerator = x2_sqrd + y2_sqrd - x1_sqrd - y1_sqrd + (((2 * y2) - (2 * y1)) * y);
            double denomenator = ((2 * x2) - (2 * x1));

            double x = numerator / denomenator;
            return (float)x;
        }

        public static float GetDistanceBetweenTwoPoints(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public static float GetDistanceBetweenTwoPoints(Point p1, Point p2)
        {
            return GetDistanceBetweenTwoPoints(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static Point Project(Point lineStart, Point lineEnd, Point point)
        {
            // Normalizing to standard equation variable names
            Point A = lineStart; Point B = lineEnd; Point P = point;

            Vector2 AP = P - A;
            Vector2 AB = B - A;
            var result = A + (Dot(AP, AB) / Dot(AB, AB)) * AB;
            return result;
        }

        public static float GetAngleBetweenVectors(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Acos(Dot(v1, v2) / (v1.Magnitude * v2.Magnitude));
        }

        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return (float)((v1.X * v2.Y) + (v1.X * v2.Y));
        }
    }
}
