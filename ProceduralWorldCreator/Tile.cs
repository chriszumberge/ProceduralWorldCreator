﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public class Tile
    {
        public float HeightValue { get; set; }
        public int X, Y;

        public int? PlateId { get; set; }

        public Tile()
        {

        }
    }
}
