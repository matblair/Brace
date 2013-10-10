using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src.PhysicsEngine
{
    class Contact
    {
        Vector3 normal;
        float distance;
        public PhysicsModel target;
        public PhysicsModel body;
        

        public Contact(PhysicsModel target, PhysicsModel body)
        {
            // TODO: Complete member initialization
            this.target = target;
            this.body = body;
        }
    }
}
