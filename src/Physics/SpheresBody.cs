using Brace.Physics;
using Brace.Utils;

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.Physics
{
    public class SpheresBody : PhysicsBody
    {
        public List<Sphere> spheres;
      
        public SpheresBody(PhysicsModel model, bool passive) : base(BodyType.stationary, model)
        {
            if(!passive) 
            {
                this.bodyType = BodyType.dynamic;
            }
            spheres = new List<Sphere>();
            spheres.Add(new Sphere(Vector3.Zero,1));
        }


    }
}
