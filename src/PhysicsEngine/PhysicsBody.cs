using Brace.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brace.PhysicsEngine
{
    public class PhysicsBody
    {
        public BodyType bodyType;
        public PhysicsModel parent;
        public PhysicsBody(BodyType a, PhysicsModel p)
        {
            parent = p;
            bodyType = a;
        }
    }
}
