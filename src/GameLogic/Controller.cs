using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brace.src.GameLogic
{
    class Controller
    {
        Unit target;
        public abstract void update(GameTime dt);
    }
}
