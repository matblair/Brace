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
        public Vector3 position { get; private set; }

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
            lightPntPos = tracking.EyeLocation() + 1.5f * Vector3.UnitY + 1.5f * tracking.ViewDirection();
        }

        // Set a new target object to track
        public void SetTarget(ITrackable track)
        {
            this.tracking = track;
        }

    }
}
