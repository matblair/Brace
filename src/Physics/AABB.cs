using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Brace.Physics
{
    /// <summary>
    /// Summary description for AABB.
    /// </summary>
    public class AABB
    {
        public Vector3 p0, p1;

        public AABB()
        {
            p0 = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            p1 = -1 * p0;
        }

        public AABB(Vector3 p0, Vector3 p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public void Expand(Vector3 p)
        {
            p0.X = Math.Min(p0.X, p.X);
            p0.Y = Math.Min(p0.Y, p.Y);
            p0.Z = Math.Min(p0.Z, p.Z);
            p1.X = Math.Max(p1.X, p.X);
            p1.Y = Math.Max(p1.Y, p.Y);
            p1.Z = Math.Max(p1.Z, p.Z);
        }

        public bool Contains(Vector3 p)
        {
            return
                p0.X <= p.X &&
                p0.Y <= p.Y &&
                p0.Z <= p.Z &&
                p1.X >= p.X &&
                p1.Y >= p.Y &&
                p1.Z >= p.Z;
        }

        public bool TestIntersection(AABB other)
        {
            // TODO
            return false;
        }
    }
}


