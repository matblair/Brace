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
        public GameObject(Vector3 position, Vector3 rotation, RenderModel rObject, PhysicsModel pObject)
        {
            this.pos = position;
            this.rot = rotation;
            this.rObject = rObject;
            this.pObject = pObject;
            solid = true;
            paused = true;
            visible = true;

        }
        private Boolean solid;
        private Boolean paused;
        private Boolean visible;


        private Vector3 pos;
        private Vector3 rot;

        public RenderModel rObject;
        public PhysicsModel pObject;

        public abstract void Update(GameTime gametime);
        public void Draw(GraphicsDeviceManager graphics)
        {
        }


    }
}
