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
        
        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }
        
        public InputManager(BraceGame game)
        {
            keys = new KeyboardManager(game);
            mouse = new MouseManager(game);
            accelerometer = Accelerometer.GetDefault();
            gestureRecogniser = new Windows.UI.Input.GestureRecognizer();

        }

        public void Update()
        {
            KeyboardState = keys.GetState();
            MouseState = mouse.GetState();

            mouse.SetPosition(new Vector2(0.5f, 0.5f));
        }

    }
}
