using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src
{
    class Unit : Actor
    {
        private Model model;

        Unit(Vector3 position, Vector3 rotation, Model model, PhysicsModel pObject)
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
            model.Draw(context, world, view, projection);
        }
    }
}
