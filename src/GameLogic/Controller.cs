using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brace.GameLogic
{
    public abstract class Controller
    {
        public Controller(Unit target) {
            this.target = target;
        }
        protected Unit target;
        public abstract void Update(GameTime dt);
    }
}
