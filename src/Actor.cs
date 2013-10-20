using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Brace
{
    using SharpDX.Toolkit.Graphics;
    using Brace.Physics;

    abstract public class Actor
    {
        
        public Boolean doomed;
        public Vector3 position { get; set; }
        public Vector3 rot { get; set; }

        public Actor(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rot = rotation;
        }
    
        public abstract void Update(GameTime gametime);
        public abstract void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect);
        
    }
}
