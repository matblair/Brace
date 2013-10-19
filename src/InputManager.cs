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
        private Camera camera;

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

        private readonly int playerBoundBox = 100;

        private bool isAiming;
        private Vector2 aimDirection;
        private bool spinAttack;

        public InputManager(BraceGame game)
        {
            camera = game.Camera;

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
            // TODO REMOVE THIS WHEN BETTER SOLUTION IS IMPLEMENTED
            if (camera == null)
            {
                camera = BraceGame.get().Camera;
            }
            if (camera.CurrentViewType != Camera.ViewType.TopDown)
            {
                camera.SetViewType(Camera.ViewType.TopDown);
            }

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
            return aimDirection;
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

       

        // Gesture events
        void OnTapped(object sender, TappedEventArgs e)
        {
            Debug.WriteLine("Tapped event");
        }

        void OnHolding(object sender, HoldingEventArgs e)
        {
            Debug.WriteLine("Holding event");
            return;
        }

        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Debug.WriteLine("Manipulation started");
        }

        void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            
        }

        Vector2 getVectorToPointer(Windows.Foundation.Point p)
        {
            // Direction of pointer on screen from center
            float x = (float)(window.Bounds.Width / 2 - p.X);
            float y = (float)(window.Bounds.Height / 2 - p.Y);
            return new Vector2(x, y);
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Debug.WriteLine("Manipulation completed");
        }

        // Raw touch input events
        void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            Debug.WriteLine("Pointer pressed");
            gestureRecogniser.ProcessDownEvent(args.CurrentPoint);
            
            Vector2 point = getVectorToPointer(args.CurrentPoint.Position);
            double x = point.X;
            double y = point.Y;

            double h = window.Bounds.Height;
            double w = window.Bounds.Width;
            
            double halfOfBoundingBox = playerBoundBox / 2;

            if (x > -halfOfBoundingBox && x < halfOfBoundingBox)
            {
                Debug.WriteLine("world");
                if (y > -halfOfBoundingBox && y < halfOfBoundingBox)
                {
                    isAiming = true;

                    Vector3 vd = camera.GetCameraTarget().ViewDirection();
                    aimDirection = new Vector2(vd.X, vd.Z);
                    Debug.WriteLine("------------------------ IS AIMING");
                }
            }
        }

        void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecogniser.ProcessMoveEvents(args.GetIntermediatePoints());

            if (isAiming)
            {
                UpdateAim(args.CurrentPoint.Position);
            }
        }

        void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecogniser.ProcessUpEvent(args.CurrentPoint);

            if (isAiming)
            {
                Debug.WriteLine("SHOT FIRED GET DOWN! (" + aimDirection.X + ", " + aimDirection.Y + ")");
            }
            isAiming = false;
        }

        void UpdateAim(Windows.Foundation.Point p)
        {
            if (this.camera.CurrentViewType == Camera.ViewType.FirstPerson)
            {
                // Set aim depending on the camera target view direction
                Vector3 fps = camera.GetCameraTarget().ViewDirection();
                aimDirection = new Vector2(fps.X, fps.Z);
            }
            else if (this.camera.CurrentViewType == Camera.ViewType.TopDown)
            {
                // Set aim depending on the position of the pointer from the center of the screen
                Vector2 pointer = getVectorToPointer(p);
                aimDirection = pointer;
                Debug.WriteLine("Top view aim set: (" + pointer.X + ", " + pointer.Y + ")");
            }
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
