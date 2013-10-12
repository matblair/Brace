using SharpDX;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Input;

namespace Brace
{
    public class InputManager
    {
        private KeyboardManager keys;
        private MouseManager mouse;
        private Accelerometer accelerometer;
        private GestureRecognizer gestureRecogniser;
        private Keys lookLeftKey = Keys.Left;
        private Keys lookDownKey = Keys.Down;
        private Keys lookRightKey = Keys.Right;
        private Keys lookUpKey = Keys.Up;
        private Keys walkLeftKey = Keys.A;
        private Keys walkBackKey = Keys.S;
        private Keys walkRightKey = Keys.D;
        private Keys walkForwardKey = Keys.W;
        
        public KeyboardState keyboardState { get; private set; }
        public MouseState mouseState { get; private set; }
        
        public InputManager(BraceGame game)
        {
            keys = new KeyboardManager(game);
            mouse = new MouseManager(game);
            accelerometer = Accelerometer.GetDefault();
            gestureRecogniser = new Windows.UI.Input.GestureRecognizer();
            
        

        }

        public void Update()
        {
            keyboardState = keys.GetState();
            mouseState = mouse.GetState();
            
        }
        public bool WalkingForward()
        {
            if(keyboardState.IsKeyDown(walkForwardKey)) 
            {
                return true;
            }
            return false;
        }
        public bool WalkingRight()
        {
            if (keyboardState.IsKeyDown(walkRightKey))
            {
                return true;
            }
            return false;
        }
        public bool WalkingBack()
        {
            if (keyboardState.IsKeyDown(walkBackKey))
            {
                return true;
            }
            return false;
        }
        public bool WalkingLeft()
        {
            if (keyboardState.IsKeyDown(walkLeftKey))
            {
                return true;
            }
            return false;
        }
        public bool LookingUp()
        {
            if (keyboardState.IsKeyDown(lookUpKey))
            {
                return true;
            }
            return false;
        }


        public bool LookingRight()
        {
            if (keyboardState.IsKeyDown(lookRightKey))
            {
                return true;
            }
            return false;
        }
        public bool LookingDown()
        {
            if (keyboardState.IsKeyDown(lookDownKey))
            {
                return true;
            }
            return false;
        }
        public bool LookingLeft()
        {
            if (keyboardState.IsKeyDown(lookLeftKey))
            {
                return true;
            }
            return false;
        }


    }
}
