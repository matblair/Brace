using Brace.PhysicsEngine;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace
{
    public class Unit : Actor, ITrackable
    {
        private Model model;

        public Unit(Vector3 position, Vector3 rotation, Model model, PhysicsModel pObject)
            : base (position, rotation, pObject)
        {
            this.model = model;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection)
        {
            Matrix world = Matrix.RotationX(rot.X) * Matrix.RotationY(rot.Y) * Matrix.RotationZ(rot.Z) * Matrix.Translation(pos);
            model.Draw(context, world, view, projection, basicEffect);
        }

        public Vector3 ViewDirection()
        {
            return Vector3.UnitX;
        }

        public Vector3 BodyLocation()
        {
            return pos;
        }

        public Vector3 EyeLocation()
        {
            return pos;
        }
    }
}
