using Brace.Physics;
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

    
        public Unit(Vector3 position, Vector3 rotation, Model model) : base(position, rotation)
        {
            this.model = model;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect)
        {
            Matrix world = Matrix.RotationX(rot.X) * Matrix.RotationY(rot.Y) * Matrix.RotationZ(rot.Z) * Matrix.Translation(pos);
            model.Draw(context, world, view, projection, effect);
        }

        public Vector3 ViewDirection()
        {
            // Might need to negate rot.X
            return Vector3.TransformCoordinate(-Vector3.UnitZ, Matrix.RotationAxis(Vector3.UnitY, rot.X));
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
