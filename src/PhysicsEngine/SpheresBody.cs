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
    public class SpheresBody : PhysicsBody
    {
        public Vector3 position;
        public List<Sphere> spheres;
        public SpheresBody(PhysicsModel parent,BodyType type,Vector3 position,List<Sphere> bodyList) : base(type,parent)
        {
            if (type)
            {
                bodyType = BodyType.dynamic;
            }
            else
            {
                bodyType = BodyType.passive;
            }
            this.position = position;
            spheres = bodyList;
        }
    }
}
