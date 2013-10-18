using Brace.Physics;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    abstract public class Unit : Actor, ITrackable
    {
        private Model model;
        public PhysicsModel pObject;


        public Unit(Vector3 position, Vector3 rotation, Model model) : base(position, rotation)
        {
            this.model = model;
            InitializePhysicsObject();
        }

        public abstract override void Update(GameTime gametime);
        protected abstract void InitializePhysicsObject();

        public void DestroyPhysicsObject()
        {
            BraceGame.get().physicsWorld.RemoveBody(pObject);
        }


        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect)
        {
            Matrix world = Matrix.RotationX(rot.X) * Matrix.RotationY(rot.Y) * Matrix.RotationZ(rot.Z) * Matrix.Translation(position);
            model.Draw(context, world, view, projection, effect);
        }

        public Vector3 ViewDirection()
        {
            // Might need to negate rot.X
            return Vector3.TransformCoordinate(-Vector3.UnitZ, Matrix.RotationAxis(Vector3.UnitY, rot.X));
        }

        public Vector3 BodyLocation()
        {
            return position;
        }

        public Vector3 EyeLocation()
        {
            return position;
        }

        
    }
}
