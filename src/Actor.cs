using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Brace
{
    using SharpDX.Toolkit.Graphics;
    using Brace.PhysicsEngine;

    abstract public class Actor
    {
        protected Boolean solid;
        protected Boolean paused;
        protected Boolean visible;

        public Vector3 pos { get; set; }
        public Vector3 rot { get; set; }
        public Effect basicEffect;

        public PhysicsModel pObject;

        public Actor(Vector3 position, Vector3 rotation)
        {
            this.pos = position;
            this.rot = rotation;
            solid = true;
            paused = true;
            visible = true;
        }
    
        public abstract void Update(GameTime gametime);
        public abstract void Draw(GraphicsDevice context, Matrix view, Matrix projection);
    }
}
