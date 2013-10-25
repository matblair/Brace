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

    public class TrackingLight : Light
    {

        // Object to track
        public ITrackable tracking;

        // Vectors related to View
        private Vector3 targetPosition;
        private readonly float SPEED = 10;

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

        public TrackingLight(ITrackable track, Vector4 pntColour, Vector3 offset, float Ka, float Kd, float Ks, float specN, float fallOffTop)
            : base(pntColour, Ka, Kd, Ks, specN, fallOffTop)
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
            
            
            Vector4 col = GetColour() * intensityVector;
            shadingLight.x = lightPntPos.X;
            shadingLight.y = lightPntPos.Y;
            shadingLight.z = lightPntPos.Z;
            shadingLight.r = col.X;
            shadingLight.g = col.Y;
            shadingLight.b = col.Z;
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
