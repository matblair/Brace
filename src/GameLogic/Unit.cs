using Brace.PhysicsEngine;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    abstract public class Unit : Actor , ITrackable
    {
        Controller controller;
        public Unit(Vector3 position, Vector3 rotation, PhysicsModel pObject) 
            : base(position, rotation, pObject)
        {

        }

  
        Vector3 ITrackable.ViewDirection()
        {
            return Vector3.UnitX;
        }

        Vector3 ITrackable.BodyLocation()
        {
            return pos;
        }

        Vector3 ITrackable.EyeLocation()
        {
            return pos;
        }
    }
}
