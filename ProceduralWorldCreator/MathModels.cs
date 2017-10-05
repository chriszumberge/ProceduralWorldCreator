using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class Vector2
    {
        Point _first;
        Point _second;

        public Point First => _first;
        public Point Second => _second;

        public float X => _second.X - _first.X;
        public float Y => _second.Y - _first.Y;

        public Vector2(Point first, Point second)
        {
            _first = first;
            _second = second;
        }

        public Vector2(Point point)
        {
            _first = Point.Zero;
            _second = point;
        }

        public Vector2(float x, float y) : this(new Point(x, y)) { }

        public float Magnitude => (float)Math.Sqrt(Math.Pow((Second.X - First.X), 2) + Math.Pow((Second.Y - First.Y), 2));

        public Vector2 UnitVector => new Vector2(Point.Zero, new Point(X / Magnitude, Y / Magnitude));

        public static Vector2 operator * (Vector2 vector, float multiplier) => new Vector2(vector.First, new Point(vector.Second.X * multiplier, vector.Second.Y * multiplier));
        public static Vector2 operator * (float multiplier, Vector2 vector) => new Vector2(vector.First, new Point(vector.Second.X * multiplier, vector.Second.Y * multiplier));
        public static Vector2 operator / (Vector2 vector, float divisor) => new Vector2(vector.First, new Point(vector.Second.X / divisor, vector.Second.Y / divisor));
        public static Vector2 operator + (Vector2 v1, Vector2 v2) => new Vector2(
                                                                            new Point(v1.First.X + v2.First.X, v1.First.Y + v2.First.Y), 
                                                                            new Point(v1.Second.X + v2.Second.X, v1.Second.Y + v2.Second.Y));
        public static Vector2 operator - (Vector2 v1, Vector2 v2) => new Vector2(
                                                                            new Point(v1.First.X - v2.First.X, v1.First.Y - v2.First.Y),
                                                                            new Point(v1.Second.X - v2.Second.X, v1.Second.Y - v2.Second.Y));

        public static Vector2 Zero => new Vector2(Point.Zero, Point.Zero);
        public static Vector2 Up => new Vector2(Point.Zero, new Point(0, 1));
        public static Vector2 Down => new Vector2(Point.Zero, new Point(0, -1));
        public static Vector2 Right => new Vector2(Point.Zero, new Point(1, 0));
        public static Vector2 Left => new Vector2(Point.Zero, new Point(-1, 0));
    }

    public class Point
    {
        float _x;
        float _y;
        public float X => _x;
        public float Y => _y;

        public Point(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public static Vector2 operator - (Point p1, Point p2) => new Vector2(p2, p1);
        public static Point operator + (Point p, Vector2 v) => new Point(p.X + v.X, p.Y + v.Y);
        public static Point operator * (Point p, int i) => new Point(p.X * i,p.Y * i);

        public static Point Zero => new Point(0, 0);
    }
}
