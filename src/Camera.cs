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
        private static float MOVE_SPEED=60f, PAN_SPEED=10f, ORIENTATION_SPEED=0.5f;

        // Support for different view types
        public enum ViewType { FirstPerson, TopDown, Follow };
        public ViewType CurrentViewType { get; private set; }

        // Object to track
        private ITrackable tracking;

        // Vectors related to View
        private Vector3 targetPosition, targetLookingAt, targetUp;
        private Vector3 position, lookingAt, up;

        // View and proj matricies
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Camera(BraceGame game, ITrackable track)
        {
            // General
            this.game = game;

            // View setup
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
            SetTarget(track);
            SetViewType(ViewType.Follow);
        }

        // Update the camera and associated things
        public void Update(GameTime gameTime)
        {
            long delta = gameTime.ElapsedGameTime.Milliseconds;

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
                    targetPosition = targetLookingAt + 30 * Vector3.UnitY;
                    targetUp = Vector3.UnitX;
                    break;

                case (ViewType.Follow):
                    targetLookingAt = tracking.EyeLocation();
                    targetPosition = targetLookingAt + 3 * Vector3.UnitY - 10 * tracking.ViewDirection();
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
                posDir.Normalize();
                position += posDir * MOVE_SPEED * delta / 1000f;
            }

            Vector3 lookDir = targetLookingAt - lookingAt;
            if (lookDir.Length() < PAN_SPEED * delta / 1000f)
            {
                lookingAt = targetLookingAt;
            }
            else
            {
                lookDir.Normalize();
                lookingAt += lookDir * PAN_SPEED * delta / 1000f;
            }

            Vector3 upDir = targetUp - up;
            if (upDir.Length() < ORIENTATION_SPEED * delta / 1000f)
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
    }
}