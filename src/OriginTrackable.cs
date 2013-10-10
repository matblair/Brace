using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace
{
    class OriginTrackable : ITrackable
    {
        public Vector3 ViewDirection()
        {
            return Vector3.UnitX;
        }

        public Vector3 EyeLocation()
        {
            return BodyLocation() + Vector3.UnitY;
        }

        public Vector3 BodyLocation()
        {
            return Vector3.UnitY * 10;
        }
    }
}
