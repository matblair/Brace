using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src
{
    class OriginTrackable : ITrackable
    {
        public Vector3 ViewDirection()
        {
            return Vector3.UnitX;
        }

        public Vector3 Location()
        {
            return Vector3.UnitY * 10;
        }
    }
}
