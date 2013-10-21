using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Brace
{
    public class Camera
    {
        // General useful things
        private BraceGame game;
        private static float MOVE_SPEED=60f, PAN_SPEED=30f, ORIENTATION_SPEED=2f;

        // Support for different view types
        public enum ViewType { FirstPerson, TopDown, Follow };
        public ViewType CurrentViewType { get; private set; }

        // Object to track
        private ITrackable tracking;

        // Vectors related to View
        private Vector3 targetPosition, targetLookingAt, targetUp;
        public Vector3 lookingAt { get; private set; }
        private Vector3 up;
        public Vector3 position { get; private set; }

        // View and proj matricies
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        // Input manager
        private InputManager input;

        public Camera(BraceGame game, ITrackable track)
        {
            // General
            this.game = game;

            // View setup
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 500.0f);
            SetTarget(track);
            SetViewType(ViewType.TopDown);

            this.position = 50 * Vector3.UnitY;
            this.up = Vector3.UnitX;
            this.lookingAt = Vector3.Zero;

            input = game.input;
        }

        // Update the camera and associated things
        public void Update(GameTime gameTime)
        {
            long delta = gameTime.ElapsedGameTime.Milliseconds;

            CurrentViewType = input.ViewType;

            // Calculate the view. This will eventually be changed to be elastic
            switch (CurrentViewType)
            {
                case (ViewType.FirstPerson):
                    targetPosition = tracking.EyeLocation();
                    targetLookingAt = targetPosition + tracking.ViewDirection();
                    targetUp = Vector3.UnitY;
                    break;

                case (ViewType.TopDown):
                    targetLookingAt = tracking.EyeLocation();
                    targetPosition = targetLookingAt + 50 * Vector3.UnitY;
                    targetUp = Vector3.UnitZ;
                    break;

                case (ViewType.Follow):
                    targetLookingAt = tracking.EyeLocation();
                    targetPosition = targetLookingAt + 7 * Vector3.UnitY - 10 * tracking.ViewDirection();
                    targetUp = Vector3.UnitY;
                    break;

                default:
                    break;
            }

            // Calculate the view
            Vector3 posDir = targetPosition - position;
            if (posDir.Length() < MOVE_SPEED * delta / 1000f)
            {
                position = targetPosition;
            }
            else
            {
                position += posDir / posDir.Length() * (float)Math.Log(posDir.Length() + 1) * MOVE_SPEED * delta / 1000f;
            }

            Vector3 lookDir = targetLookingAt - lookingAt;
            if (lookDir.Length() < PAN_SPEED * delta / 1000f)
            {
                lookingAt = targetLookingAt;
            }
            else
            {
                lookingAt += lookDir * lookDir.Length() * delta / 1000f;
            }

            Vector3 upDir = targetUp - up;
            if (Math.Pow(upDir.Length(), 2) < ORIENTATION_SPEED * delta / 1000f)
            {
                up = targetUp;
            }
            else
            {
                upDir.Normalize();
                up += upDir * ORIENTATION_SPEED * delta / 1000f;
            }

            View = Matrix.LookAtLH(position, lookingAt, up);
        }

        // Set a new target object to track
        public void SetTarget(ITrackable track)
        {
            this.tracking = track;
        }

        // Set the type of camera view
        public void SetViewType(ViewType type)
        {
            this.CurrentViewType = type;
        }

        public ITrackable GetCameraTarget()
        {
            return tracking;
        }
    }
}