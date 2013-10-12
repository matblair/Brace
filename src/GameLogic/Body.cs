using Brace.PhysicsEngine;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    public class Body : Unit 
    {
        MouseState previousMouseState;
        MouseState currentMouseState;
        float mouseSens = 5;
        float prevTime=0;
        float movementSpeed = 5;


        public Body(Vector3 position, Vector3 rotation) : base(position,rotation,null)
        {
            solid = false;
            visible = false;
            BraceGame.get().Camera.SetTarget(this);
            BraceGame.get().Camera.SetViewType(Camera.ViewType.FirstPerson);
        }

        public override void Update(GameTime gameTime)
        {
            

            
        }

        public override void Draw(SharpDX.Toolkit.Graphics.GraphicsDevice context, Matrix view, Matrix projection)
        {
        }


    }
}
