using Brace.GameLogic;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class UnitController : Controller
    {
        public UnitController(Unit target)
            : base(target)
        {

        }
        public override void Update(GameTime dt)
        {
            float timeInSeconds = dt.ElapsedGameTime.Milliseconds;
            timeInSeconds /= 1000;
            if (BraceGame.get().input.WalkingForward())
            {
                target.pObject.ApplyImpulse(50*Vector3.UnitX * timeInSeconds);
            }
            if (BraceGame.get().input.WalkingBack())
            {
                target.pObject.ApplyImpulse(50*-Vector3.UnitX * timeInSeconds);
            }
            if (BraceGame.get().input.WalkingLeft())
            {
                target.pObject.ApplyImpulse(50*-Vector3.UnitZ* timeInSeconds);
            }
            if (BraceGame.get().input.WalkingRight())
            {
                target.pObject.ApplyImpulse(50*Vector3.UnitZ* timeInSeconds);
            }
            if (BraceGame.get().input.LookingDown())
            {
                target.rot-= Vector3.UnitY * timeInSeconds;
            }
            if (BraceGame.get().input.LookingUp())
            {
                target.rot += Vector3.UnitY * timeInSeconds;
            }
            if (BraceGame.get().input.LookingLeft())
            {
                target.rot-= Vector3.UnitX * timeInSeconds;
            }
            if (BraceGame.get().input.LookingRight())
            {
                target.rot+= Vector3.UnitX * timeInSeconds;
            }
          
        }
    }
}
