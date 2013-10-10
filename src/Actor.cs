﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using Project1.src;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;

    abstract public class Actor
    {
        private Boolean solid;
        private Boolean paused;
        private Boolean visible;

        public Vector3 pos { get; private set; }
        public Vector3 rot { get; private set; }
        public BasicEffect basicEffect;

        public PhysicsModel pObject;

        public Actor(Vector3 position, Vector3 rotation, PhysicsModel pObject)
        {
            this.pos = position;
            this.rot = rotation;
            this.pObject = pObject;
            solid = true;
            paused = true;
            visible = true;
        }

        public abstract void Update(GameTime gametime);
        public abstract void Draw(GraphicsDevice context, Matrix view, Matrix projection);
    }
}