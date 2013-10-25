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
        public Camera Camera;
        private Gyrometer gyro;
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
        private readonly int orientationChangeDelayInMilliseconds = -500;
        private bool isAiming;
        private Vector2 aimDirection;
        public Vector2 moveCoordinate;
        public Camera.ViewType ViewType { get; private set; }
        private Camera.ViewType ViewTypeToLoad;
        private System.DateTimeOffset ViewTypeSetTime;
        private System.DateTimeOffset ChangeViewTime;

        private int turnLeftScreenBoundary;
        private int turnRightScreenBoundary;
        private int walkForwardScreenBoundary;
        private int shootScreenBoundary;

        private bool screenTurnLeftButtonDown;
        private bool screenTurnRightButtonDown;
        private bool screenWalkForwardButtonDown;
        private bool screenShootButtonDown;
        private bool didTapped;

        public InputManager(BraceGame game)
        {
            // Initialise the mouse and keyboard
            keys = new KeyboardManager(game);
            mouse = new MouseManager(game);

            // Set the accelerometer
            accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null)
            {
       
                accelerometer.ReadingChanged += AccelerometerReadingChanged;
                ViewTypeSetTime = System.DateTimeOffset.Now;
            }

            gyro = Gyrometer.GetDefault();
            if( gyro!=null){
                gyro.ReadingChanged += GyrometerReadingChanged;
            }

            // Set up gesture recogniser
            window = Window.Current.CoreWindow;
            gestureRecogniser = new Windows.UI.Input.GestureRecognizer();

            this.gestureRecogniser.ShowGestureFeedback = false;

            gestureRecogniser.GestureSettings = GestureSettings.Tap;
            gestureRecogniser.Tapped += OnTapped;
            didTapped = false;

            window.PointerPressed += OnPointerPressed;
            window.PointerMoved += OnPointerMoved;
            window.PointerReleased += OnPointerReleased;

            // Initialise variables
            isAiming = false;
            screenTurnLeftButtonDown = false;
            screenTurnRightButtonDown = false;
            ViewType = Camera.ViewType.TopDown;
            ViewTypeToLoad = ViewType;

            screenTurnLeftButtonDown = false;
            screenTurnRightButtonDown = false;
            screenWalkForwardButtonDown = false;
            screenShootButtonDown = false;

            int turnBoundary = (int)Math.Floor(window.Bounds.Width / 5);
         
            turnLeftScreenBoundary = turnBoundary;
            turnRightScreenBoundary = (int) window.Bounds.Width - turnBoundary;
            walkForwardScreenBoundary = (int) Math.Floor(window.Bounds.Height / 2);
            shootScreenBoundary = walkForwardScreenBoundary;    // Same but checks the opposite
        }

        public void Update()
        {
            keyboardState = keys.GetState();
            mouseState = mouse.GetState();

            didTapped = false;

            if (keyboardState.IsKeyDown(toggleCameraKey))
            {
                if (ViewType == Camera.ViewType.Follow)
                {
                    ViewType = Camera.ViewType.FirstPerson;
                }
                else if (ViewType == Camera.ViewType.FirstPerson)
                {
                    ViewType = Camera.ViewType.TopDown;
                }
                else if (ViewType == Camera.ViewType.TopDown)
                {
                    ViewType = Camera.ViewType.Follow;
                } 
            }
        }

        private void AccelerometerReadingChanged(object sender, AccelerometerReadingChangedEventArgs args)
        {
            AccelerometerReading reading = args.Reading;
           
            System.DateTimeOffset time = args.Reading.Timestamp;
            System.DateTimeOffset t2 = time.AddMilliseconds(orientationChangeDelayInMilliseconds);
      
            
            if (reading.AccelerationZ < -0.85)
            {   
                if (ViewTypeToLoad != Camera.ViewType.TopDown)
                {
                    // Acceleration reading is suggest different view type than the current view
                    ViewTypeToLoad = Camera.ViewType.TopDown;   // Set the view to load
                    ChangeViewTime = time;                      // Set the time the view was set
                }
            }
            else
            {
                if (ViewTypeToLoad != Camera.ViewType.FirstPerson)
                {
                    // Acceleration reading is suggest different view type than the current view
                    ViewTypeToLoad = Camera.ViewType.FirstPerson;      // Set the view to load
                    ChangeViewTime = time;                             // Set the time the view was set
                }
            }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
            if (ViewType != ViewTypeToLoad && t2 > ChangeViewTime)
            {
                ViewType = ViewTypeToLoad;
            }
        }

        private void GyrometerReadingChanged(object sener, GyrometerReadingChangedEventArgs args)
        {
            if (ViewType == Camera.ViewType.FirstPerson)
            {


                if (args.Reading.AngularVelocityX < 0)
                {
                    screenTurnLeftButtonDown = true;
                }
                else if (args.Reading.AngularVelocityX>  0)
                {
                    screenTurnRightButtonDown = true;
                }
              
            }
            Debug.WriteLine(args.ToString());
      


        }


        // interface

        public bool isShooting()
        {
            return isAiming || screenShootButtonDown;
        }

        public Vector2 shotDirection()
        {
            return aimDirection;
        }

        public Vector2 moveTo()
        {
            return moveCoordinate;
        }

        // Gesture events
        void OnTapped(object sender, TappedEventArgs e)
        {
            didTapped = true;
            if (ViewType == Camera.ViewType.TopDown)
            {
                Vector2 p = getVectorToPointer(e.Position);         // coordinate of tap from center of screen.
                Vector3 target = Camera.lookingAt;
                p.X = -p.X;                                         // Negate make x the same direction as in the world
                double window_w = window.Bounds.Width;
                float dh = Camera.position.Y - target.Y;

                moveCoordinate = (p / ((float)window_w) * 2 * dh);
                moveCoordinate += new Vector2(target.X, target.Z);
            }
        }

        Vector2 getVectorToPointer(Windows.Foundation.Point p)
        {
            // Direction of pointer on screen from center
            float x = (float)(window.Bounds.Width / 2 - p.X);
            float y = (float)(window.Bounds.Height / 2 - p.Y);
            return new Vector2(x, y);
        }

        // Raw touch input events
        void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (ViewType == Camera.ViewType.TopDown && !didTapped)
            {
                Vector2 p = getVectorToPointer(args.CurrentPoint.Position);
                double halfOfBoundingBox = playerBoundBox / 2;

                if ((p.X > -halfOfBoundingBox && p.X < halfOfBoundingBox) && (p.Y > -halfOfBoundingBox && p.Y < halfOfBoundingBox))
                {
                    isAiming = true;

                    // Initialise aim direction to the view direction of the camera target
                    Vector3 vd = Camera.GetCameraTarget().ViewDirection();
                    aimDirection = new Vector2(vd.X, vd.Z);
                }
            }
            else if (ViewType == Camera.ViewType.FirstPerson)
            {
                Windows.Foundation.Point p = args.CurrentPoint.Position;

                if (p.X < turnLeftScreenBoundary)
                {
                    screenTurnLeftButtonDown = true;
                }
                else if (p.X >= turnRightScreenBoundary)
                {
                    screenTurnRightButtonDown = true;
                }
                else if (p.Y <= walkForwardScreenBoundary)
                {
                    screenWalkForwardButtonDown = true;
                }
                else if (p.Y > shootScreenBoundary)
                {
                    screenShootButtonDown = true;
                    // Set aim depending on the camera target view direction
                    Vector3 player = Camera.GetCameraTarget().ViewDirection();
                    aimDirection = new Vector2(player.X, player.Z);
                }
            }
            gestureRecogniser.ProcessDownEvent(args.CurrentPoint);

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
            isAiming = false;
            if (ViewType == Camera.ViewType.FirstPerson)
            {
                screenTurnLeftButtonDown = false;
                screenTurnRightButtonDown = false;
                screenWalkForwardButtonDown = false;
                screenShootButtonDown = false;
            }
            try
            {
                gestureRecogniser.ProcessUpEvent(args.CurrentPoint);
            }
            catch (System.Exception e)
            {
                // Doesn't matter if the exception is thrown, do nothing
            }
        }

        void UpdateAim(Windows.Foundation.Point p)
        {
            if (this.Camera.CurrentViewType == Camera.ViewType.FirstPerson)
            {
                // Set aim depending on the camera target view direction
                Vector3 fps = Camera.GetCameraTarget().ViewDirection();
                aimDirection = new Vector2(fps.X, fps.Z);
            }
            else if (this.Camera.CurrentViewType == Camera.ViewType.TopDown)
            {
                // Set aim depending on the position of the pointer from the center of the screen
                Vector3 target = Camera.lookingAt;
                
                Vector2 pointer = getVectorToPointer(p);
                pointer.X = -pointer.X;                               // Negate make x the same direction as in the world

                pointer = Vector2.Negate(pointer);
                aimDirection = pointer;
            }
        }


        // Walking methods
        public bool WalkingForward()
        {
            return screenWalkForwardButtonDown || keyboardState.IsKeyDown(walkForwardKey) && !keyboardState.IsKeyDown(walkBackKey);
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

        public bool TurningLeft()
        {
            return screenTurnLeftButtonDown             && !screenTurnRightButtonDown
                || keyboardState.IsKeyDown(lookLeftKey) && !keyboardState.IsKeyDown(lookRightKey);
        }

        public bool TurningRight()
        {
            return screenTurnRightButtonDown && !screenTurnLeftButtonDown
                || keyboardState.IsKeyDown(lookRightKey) && !keyboardState.IsKeyDown(lookLeftKey);
        }

        public bool toggleCamera()
        {
            return keyboardState.IsKeyDown(toggleCameraKey) && !keyboardState.IsKeyDown(shiftKey);
        }

        public bool toggleCameraReverse()
        {
            return keyboardState.IsKeyDown(toggleCameraKey) && keyboardState.IsKeyDown(shiftKey);
        }
    }
}
