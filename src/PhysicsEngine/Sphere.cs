using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project1.src.PhysicsEngine
{
    public class Sphere
    {
        public Vector3 position;
        public float radius;
        Sphere(Vector3 position, float radius)
        {
            
            this.position = position;
            this.radius = radius;
        }
    }
}
