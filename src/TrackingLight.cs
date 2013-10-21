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
        
        //Whether or not the light is on or off
        private bool isVisible;
        // Our lights offset from eye location
        Vector3 trackOffset;

        public TrackingLight(ITrackable track, Vector3 offset)
        {
            //Set our item to track.
            SetTarget(track);

            if (track != null)
            {
                //Our initial position
                lightPntPos = tracking.EyeLocation();
                isVisible = true;

            }

            //Set our tracking offset.
            trackOffset = offset;
        }

        public TrackingLight(ITrackable track, Vector4 pntColour, Vector3 offset)
        {
            //Set our item to track.
            SetTarget(track);
            this.lightPntCol = pntColour;
           
            if (track != null)
            {
                //Our initial position
                lightPntPos = tracking.EyeLocation();
                isVisible = true;
            }

            //Set our tracking offset.
            trackOffset = offset;

        }

        // Update the camera and associated things
        public void Update(GameTime gameTime)
        {
            if (tracking != null)
            {
                
                int delta = gameTime.ElapsedGameTime.Milliseconds;
                targetPosition = tracking.EyeLocation() + trackOffset.Y * Vector3.UnitY + trackOffset.Z * tracking.ViewDirection();
                Vector3 dir = Vector3.Subtract(targetPosition, lightPntPos);

                if (dir.Length() * dir.Length() < SPEED * delta / 1000f)
                {
                    lightPntPos = targetPosition;
                }
                else
                {
                    lightPntPos = Vector3.Add(lightPntPos, dir * SPEED * delta / 1000f);
                }

                lightPntPos = tracking.EyeLocation() + trackOffset.Y * Vector3.UnitY + trackOffset.Z * tracking.ViewDirection();

            }
        }

        // Set a new target object to track
        public void SetTarget(ITrackable track)
        {
            this.tracking = track;
        }

        // Snap to a position
        // Set a new target object to track
        public void SnapToTarget(ITrackable track)
        {
            if (track != null)
            {
                //Our initial position
                lightPntPos = track.EyeLocation();
                this.tracking = track;
            }
        }

        public void TurnOff()
        {
            isVisible = false;
        }

        public void TurnOn()
        {
            isVisible = true;
        }
        public Vector4 GetColour()
        {
            if (isVisible)
            {
                return lightPntCol;
            }
            else
            {
                return new Vector4(0, 0, 0, 1);
            }
        }

        public bool IsTracking(ITrackable unit)
        {
            if (this.tracking != null)
            {
                return this.tracking.Equals(unit);
            }
            else
            {
                return false;
            }
        }
    }
}
