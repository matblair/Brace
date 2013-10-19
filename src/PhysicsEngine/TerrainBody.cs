using Brace.PhysicsEngine;
using Brace.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.PhysicsEngine
{
    public class TerrainBody : PhysicsBody
    {
        public float[,] points;
        public float xzScale;
        public Vector3 up;
        public TerrainBody(PhysicsModel parent,float[,] points, float xzScale) : base(BodyType.terrain, parent)
        {
            bodyType = BodyType.terrain;
            this.points = points;
            this.up = new Vector3(0, 1, 0);
            this.xzScale = xzScale;
        }
    }
}
