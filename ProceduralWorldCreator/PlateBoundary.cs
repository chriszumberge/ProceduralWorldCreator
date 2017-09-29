using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralWorldCreator
{
    public enum PlateBoundary
    {
        TransformBoundary,
        DivergentBoundary,
        OceanicContinentalConvergentBoundary, // volcanos
        ContinentalContinentalConvergentBoundary, // mountain ranges
        OceanicOceanicConvergentBoundary // islands
    }
}
