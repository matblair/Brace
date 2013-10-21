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
        private readonly float MAX_PITCH_SPEED = 8;

        public Boolean doomed;
        public Vector3 position { get; set; }
        public Vector3 rot;

        public Actor(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rot = rotation;
        }
    
        public abstract void Update(GameTime gametime);
        public abstract void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect);

        // Assumes a frame rate of 60 FPS
        public void SetRotPitch(float angle)
        {
            while (angle - rot.X < -Math.PI)
            {
                angle += 2 * (float)Math.PI;
            }
            while (angle - rot.X > Math.PI)
            {
                angle -= 2 * (float)Math.PI;
            }

            float diff = angle - rot.X;
            if (Math.Abs(diff) < MAX_PITCH_SPEED / 60f)
            {
                rot.X = angle;
            }
            else
            {
                rot.X += diff * MAX_PITCH_SPEED / 60f;
            }

            if (rot.X < 0)
            {
                rot.X += 2 * (float)Math.PI;
            }
            else if (rot.X > Math.PI * 2)
            {
                rot.X -= 2 * (float)Math.PI;
            }
        }
    }
}
