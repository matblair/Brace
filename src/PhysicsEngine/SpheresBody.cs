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
    class SpheresBody : PhysicsBody
    {
        public Vector3 position;
        public List<Sphere> spheres;
        SpheresBody(bool type)
        {
            if (type)
            {
                bodyType = BodyType.dynamic;
            }
            else
            {
                bodyType = BodyType.passive;
            }
        }
    }
}
