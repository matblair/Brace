using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Project1.src;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
namespace Project1
{


    public class Camera
    {
        // General useful things
        private Brace game;

        // Support for different view types
        public static enum ViewType { FirstPerson, TopDown };
        private ViewType currentViewType;

        // Object to track
        private ITrackable tracking;

        // Vectors related to View
        private Vector3 targetPosition, targetLookingAt, targetUp;
        private Vector3 position, lookingAt, up;

        // View and proj matricies
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Camera(Brace game, ITrackable track)
        {
            // General
            this.game = game;

            // View setup
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
            SetTarget(track);
            SetViewType(ViewType.FirstPerson);
        }

        // Update the camera and associated things
        public void Update(GameTime gameTime)
        {
            // Calculate the view. This will eventually be changed to be elastic
            switch (currentViewType)
            {
                case (ViewType.FirstPerson):
                    targetPosition = tracking.Location();
                    targetLookingAt = targetPosition + tracking.ViewDirection();
                    targetUp = Vector3.UnitY;
                    break;

                case (ViewType.TopDown):
                    targetLookingAt = tracking.Location();
                    targetPosition = targetLookingAt + 10 * Vector3.UnitY;
                    targetUp = Vector3.UnitX;
                    break;

                default:
                    break;
            }

            // Calculate the view. This will eventually be changed to be elastic
            position = targetPosition;
            lookingAt = targetLookingAt;
            up = targetUp;

            View = Matrix.LookAtLH(position, lookingAt, up);
        }

        // Set a new target object to track
        public void SetTarget(ITrackable track)
        {
            this.tracking = track;
        }

        // Set the type of camera view
        public void SetViewType(this ViewType type)
        {
            this.currentViewType = type;
        }
    }
}