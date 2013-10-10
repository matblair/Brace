using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src.PhysicsEngine
{
    class Terrain : PhysicsBody
    {
        List<Vector3> points;
        Vector3 up;
        Terrain(List<Vector3> points, Vector3 up)
        {
            bodyType = BodyType.terrain;
            this.points = points;
            this.up = up;
        }
    }
}
