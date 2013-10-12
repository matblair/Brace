﻿using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brace.GameLogic
{
    public abstract class Controller
    {
        Unit target;
        public abstract void update(GameTime dt);
    }
}