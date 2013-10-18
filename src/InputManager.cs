using SharpDX;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Xaml;
using Windows.UI.Input;
using Windows.UI.Core;
using System.Diagnostics;

namespace Brace
{
    public class InputManager
    {
        private KeyboardManager keys;
        private MouseManager mouse;
        private Accelerometer accelerometer;
        private GestureRecognizer gestureRecogniser;
        private CoreWindow window;

        private Keys lookLeftKey = Keys.Left;
        private Keys lookDownKey = Keys.Down;
        private Keys lookRightKey = Keys.Right;
        private Keys lookUpKey = Keys.Up;
        private Keys walkLeftKey = Keys.A;
        private Keys walkBackKey = Keys.S;
        private Keys walkRightKey = Keys.D;
        private Keys walkForwardKey = Keys.W;
        private Keys toggleCameraKey = Keys.Tab;
        private Keys shiftKey = Keys.Shift;
        
        public KeyboardState keyboardState { get; private set; }
        public MouseState mouseState { get; private set; }

        private int taps;

        public InputManager(BraceGame game)
        {
            // Initialise the mouse and keyboard
            keys = new KeyboardManager(game);
            mouse = new MouseManager(game);

            // Set the accelerometer
            this.accelerometer = Accelerometer.GetDefault();

            // Set up gesture recogniser
            window = Window.Current.CoreWindow;
            this.gestureRecogniser = new Windows.UI.Input.GestureRecognizer();

            //this.gestureRecogniser.ShowGestureFeedback = true;

            this.gestureRecogniser.GestureSettings = GestureSettings.Tap;

            window.PointerPressed += OnPointerPressed;
            window.PointerMoved += OnPointerMoved;
            window.PointerReleased += OnPointerReleased;
        }

        public void Update()
        {
            keyboardState = keys.GetState();
            mouseState = mouse.GetState();
        }

        // Gesture methods
        void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            Debug.WriteLine("Pointer pressed");
            taps += 1;
            Debug.WriteLine(taps);
            Debug.WriteLine(args.CurrentPoint.Position.X);
            Debug.WriteLine(args.CurrentPoint.Position.Y);
        }

        void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            Debug.WriteLine("Pointer moved");
        }

        void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
        {

        }

        // Walking methods
        public bool WalkingForward()
        {
            return keyboardState.IsKeyDown(walkForwardKey) && !keyboardState.IsKeyDown(walkBackKey);
        }

        public bool WalkingBack()
        {
            return keyboardState.IsKeyDown(walkBackKey) && !keyboardState.IsKeyDown(walkForwardKey);
        }

        public bool WalkingLeft()
        {
            return keyboardState.IsKeyDown(walkLeftKey) && !keyboardState.IsKeyDown(walkRightKey);
        }

        public bool WalkingRight()
        {
            return keyboardState.IsKeyDown(walkRightKey) && !keyboardState.IsKeyDown(walkLeftKey);
        }

        // Looking methods
        public bool LookingUp()
        {
            return keyboardState.IsKeyDown(lookUpKey) && !keyboardState.IsKeyDown(lookDownKey);
        }

        public bool LookingDown()
        {
            return keyboardState.IsKeyDown(lookDownKey) && !keyboardState.IsKeyDown(lookUpKey);
        }

        public bool LookingLeft()
        {
            return keyboardState.IsKeyDown(lookLeftKey) && !keyboardState.IsKeyDown(lookRightKey);
        }

        public bool LookingRight()
        {
            return keyboardState.IsKeyDown(lookRightKey) && !keyboardState.IsKeyDown(lookLeftKey);
        }

        public bool toggleCamera()
        {
            return keyboardState.IsKeyDown(toggleCameraKey) && !keyboardState.IsKeyDown(shiftKey);
        }

        public bool toggleCameraReverse()
        {
            return keyboardState.IsKeyDown(toggleCameraKey) && keyboardState.IsKeyDown(shiftKey);
        }

        public bool isAttacking()
        {
            // False until I implement the gestures
            return false;
        }
    }
}
