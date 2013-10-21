using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Brace
{
    class TrackingLight
    {

        // Object to track
        private ITrackable tracking;

        // Vectors related to View
        private Vector3 targetPosition;
        private readonly float SPEED = 10;

        // Our world lighting setups Will obviously need to be changed at a later 
        // date in order to properly having moving light sources etc.
        public Vector4 lightPntCol = new Vector4(1.0f, 0.8f, 0.1f, 1.0f);
        public Vector4 lightAmbCol = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public Vector3 lightPntPos { get; private set; }

        public TrackingLight(ITrackable track)
        {
            //Set our item to track.
            SetTarget(track);

            //Our initial position
            lightPntPos = tracking.EyeLocation();
        }

        // Update the camera and associated things
        public void Update(GameTime gameTime)
        {
            int delta = gameTime.ElapsedGameTime.Milliseconds;
            targetPosition = tracking.EyeLocation() + 1.5f * Vector3.UnitY + 4.5f * tracking.ViewDirection();
            Vector3 dir = Vector3.Subtract(targetPosition, lightPntPos);

            if (dir.Length() * dir.Length() < SPEED * delta / 1000f)
            {
                lightPntPos = targetPosition;
            }
            else
            {
                lightPntPos = Vector3.Add(lightPntPos, dir * SPEED * delta / 1000f);
            }
        }

        // Set a new target object to track
        public void SetTarget(ITrackable track)
        {
            this.tracking = track;
        }

    }
}
