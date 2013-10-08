using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using Project1.src;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;

    abstract public class GameObject
    {
        private Boolean solid;
        private Boolean paused;
        private Boolean visible;

        private Vector3 pos;
        private Vector3 rot;

        public Model rObject;
        public PhysicsModel pObject;

        public GameObject(Vector3 position, Vector3 rotation, Model rObject, PhysicsModel pObject)
        {
            this.pos = position;
            this.rot = rotation;
            this.rObject = rObject;
            this.pObject = pObject;
            solid = true;
            paused = true;
            visible = true;

        }

        public abstract void Update(GameTime gametime);

        public void Draw(GraphicsDevice context, Matrix view, Matrix projection)
        {
            Matrix world = Matrix.RotationX(rot.X) * Matrix.RotationY(rot.Y) * Matrix.RotationZ(rot.Z) * Matrix.Translation(pos);
            rObject.Draw(context, world, view, projection);
        }


    }
}
