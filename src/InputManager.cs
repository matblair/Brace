using SharpDX;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Input;

namespace Project1.src
{
    public class InputManager
    {
        private KeyboardManager keys;
        private MouseManager mouse;
        private Accelerometer accelerometer;
        private GestureRecognizer gestureRecogniser;
        
        public KeyboardState _keyboard;
        public MouseState _mouse;
        

        public InputManager()
        {
            keys = new KeyboardManager(Brace.get());
            mouse = new MouseManager(Brace.get());
            accelerometer = Accelerometer.GetDefault();
            gestureRecogniser = new Windows.UI.Input.GestureRecognizer();

        }
        public void Update()
        {
            _keyboard = keys.GetState();
            _mouse = mouse.GetState();

            mouse.SetPosition(new Vector2(0.5f, 0.5f));
        }

    }
}
