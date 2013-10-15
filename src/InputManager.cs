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
           
            return keyboardState.IsKeyDown(walkForwardKey);
        }
        public bool WalkingRight()
        {
            return keyboardState.IsKeyDown(walkRightKey);
        }
        public bool WalkingBack()
        {
            
            return keyboardState.IsKeyDown(walkBackKey);
        }
        public bool WalkingLeft()
        {
            
            return keyboardState.IsKeyDown(walkLeftKey);
        }
        public bool LookingUp()
        {
            
            return keyboardState.IsKeyDown(lookUpKey);
        }


        public bool LookingRight()
        {
            
            return keyboardState.IsKeyDown(lookRightKey);
        }
        public bool LookingDown()
        {
            
            return keyboardState.IsKeyDown(lookDownKey);
        }
        public bool LookingLeft()
        {
            
            return keyboardState.IsKeyDown(lookLeftKey);
        }


    }
}
