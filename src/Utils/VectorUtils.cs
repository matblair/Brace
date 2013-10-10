using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src.Utils
{
    class VectorUtils
    {
        public static Vector3 add(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3 sub(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3 mul(Vector3 a, float factor)
        {
            return new Vector3(a.X * factor, a.Y * factor, a.Z * factor);

        }
        public static Vector3 div(Vector3 a, float factor)
        {
            return new Vector3(a.X / factor, a.Y / factor, a.Z / factor);

        }
        public static float dotProduct(Vector3 a,Vector3 b) 
        {
            return (a.X * b.X + a.Y * b.Y + a.Z * b.Z);
        }
        //returns the magnitude squared
        public static float mag2(Vector3 a)
        {
            return (a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        }
    }
}
