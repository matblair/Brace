using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.PhysicsEngine
{
    public class Contact
    {
        Vector3 normal;
        float distance;
        public PhysicsBody x;
        public PhysicsBody y;



        public Contact(SpheresBody target,SpheresBody body,Vector3 direction,float distance)
        {
            // TODO: Complete member initialization
            this.x = target;
            this.y = body;
            normal = direction;
            this.distance = distance;

        }

        public Contact(TerrainBody target, Vector3 targetPoint, SpheresBody body, Vector3 lowestPoint)
        {
            // TODO: Complete member initialization
            x = target;
            y = body;
            normal = targetPoint - lowestPoint;
            distance = normal.Length();
            normal = normal / distance;

        }
    }
}
