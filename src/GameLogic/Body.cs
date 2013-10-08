using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src.GameLogic
{
    class Body : Actor
    {
        MouseState previousMouseState;
        MouseState currentMouseState;
        float mouseSens = 5;
        float prevTime=0;
        float movementSpeed = 5;

        Body(Vector3 position, Vector3 rotation, PhysicsModel pObject)
            : base(position, rotation, pObject)
        {

        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SharpDX.Toolkit.Graphics.GraphicsDevice context, Matrix view, Matrix projection)
        {
            throw new NotImplementedException();
        }
    }
}
