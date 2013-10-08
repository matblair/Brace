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
        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.TotalGameTime.TotalSeconds - prevTime;
            if (previousMouseState == null)
            {
                previousMouseState = currentMouseState;
                return;
            }
            var dx = currentMouseState.X - 0.5f;
            var dy = currentMouseState.Y - 0.5f;
            dx *= mouseSens;
            dy *= mouseSens;

            Brace.get().view.addPitch(dy);
            Brace.get().view.addYaw(dx);
            //when passing in the distance to move
            //we times the movementSpeed with dt this is a time scale
            //so if its a slow frame u move more then a fast frame
            //so on a slow computer you move just as fast as on a fast computer
            if (Brace.get().input._keyboard.IsKeyDown(Keys.W))//move forward
            {
                Brace.get().view.walkForward(movementSpeed * dt);
            }
            if (Brace.get().input._keyboard.IsKeyDown(Keys.S))//move backwards
            {
                Brace.get().view.walkBackwards(movementSpeed * dt);
            }
            prevTime = (float)gameTime.TotalGameTime.TotalSeconds;
            previousMouseState = currentMouseState;

        }
    }
}
