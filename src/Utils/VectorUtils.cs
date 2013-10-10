using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.Utils
{
    class VectorUtils
    {
        
    

        //returns the magnitude squared
        public static float mag2(Vector3 a)
        {
            return (a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        }
    }
}
