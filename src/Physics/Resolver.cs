﻿using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src.Physics
{
    class Resolver
    {
        public static void step(GameTime gameTime, GameObject[] actors){
            moveObjects();
            checkForCollisions();
            resolveCollisions();
        }

        private static void resolveCollisions()
        {
            throw new NotImplementedException();
        }

        private static void moveObjects()
        {
            throw new NotImplementedException();
        }

        private static void checkForCollisions()
        {
            throw new NotImplementedException();
        }
    }
}
