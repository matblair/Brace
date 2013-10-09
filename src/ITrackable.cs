using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src
{
    public interface ITrackable
    {
        Vector3 ViewDirection();
        Vector3 BodyLocation();
        Vector3 EyeLocation();
    }
}
