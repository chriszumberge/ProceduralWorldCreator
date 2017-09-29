using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public static class ArrayExtensions
    {
        public static List<T> AsList<T>(this T[,] array)
        {
            return array.Cast<T>().ToList();
        }
    }
}
