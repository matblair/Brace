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
        private readonly int playerBoundBox = 50;

        private bool isAiming;
        private bool spinAttack;
        private bool slashing;

        public InputManager(BraceGame game)
        {
            // Initialise the mouse and keyboard
            keys = new KeyboardManager(game);
            mouse = new MouseManager(game);

            // Set the accelerometer
            this.accelerometer = Accelerometer.GetDefault();

            // Set up gesture recogniser
            window = Window.Current.CoreWindow;
            gestureRecogniser = new Windows.UI.Input.GestureRecognizer();

            this.gestureRecogniser.ShowGestureFeedback = false;

            gestureRecogniser.GestureSettings =
                                                GestureSettings.Tap |
                                                GestureSettings.Hold |
                                                GestureSettings.ManipulationTranslateX |
                                                GestureSettings.ManipulationTranslateY |
                                                GestureSettings.CrossSlide;

            gestureRecogniser.Tapped += OnTapped;
            gestureRecogniser.Holding += OnHolding;
            gestureRecogniser.ManipulationStarted += OnManipulationStarted;
            gestureRecogniser.ManipulationUpdated += OnManipulationUpdated;
            gestureRecogniser.ManipulationCompleted += OnManipulationCompleted;


            window.PointerPressed += OnPointerPressed;
            window.PointerMoved += OnPointerMoved;
            window.PointerReleased += OnPointerReleased;

            // Set the flags
            isAiming = false;
            spinAttack = false;
        }

        public void Update()
        {
            keyboardState = keys.GetState();
            mouseState = mouse.GetState();
        }

        // interface

        public bool isShooting()
        {
            return isAiming;
        }

        public Vector2 shotDirection()
        {
            Vector2 v = new Vector2(0, 0);
            return v;
        }

        public bool performSpinAttack()
        {
            return spinAttack;
        }

        public bool isMoving()
        {
            return false;
        }

        public Vector2 moveTo()
        {
            Vector2 v = new Vector2 (0,0);
            return v;
        }

        // 2d stuff
        public bool performSlashAttack()
        {
            return slashing;
        }

        

        // Gesture events
        void OnTapped(object sender, TappedEventArgs e)
        {
            //Debug.WriteLine("Tapped event");
        }

        void OnHolding(object sender, HoldingEventArgs e)
        {
            //Debug.WriteLine("Holding event");
            BraceGame game = BraceGame.get();
            double h = window.Bounds.Height;
            double w = window.Bounds.Width;
            double x = e.Position.X;
            double y = e.Position.Y;
            double halfOfBoundingBox = playerBoundBox / 2;
            if (x < w / 2 - halfOfBoundingBox && x > w / 2 + halfOfBoundingBox)
            {
                
                if (y < h / 2 - halfOfBoundingBox && y > h / 2 + halfOfBoundingBox)
                {
                    isAiming = true;
                    //Debug.WriteLine("IS AIMING!!!");
                }
            }
        }

        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //Debug.WriteLine("Manipulation started");
        }

        void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            if (isAiming)
            {
                // calculate direction of shot
            }
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            //Debug.WriteLine("Manipulation completed");
        }

        // Raw touch input events
        void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecogniser.ProcessDownEvent(args.CurrentPoint);
        }

        void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecogniser.ProcessMoveEvents(args.GetIntermediatePoints());
        }

        void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecogniser.ProcessUpEvent(args.CurrentPoint);
            isAiming = false;
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
