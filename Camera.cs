using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class Camera
    {
        private Project1Game game;
        private int mouseWheelLastOffset = 0;

        // Positioning related variables
        private float heightAngle, zoomDist, angle;
        private float heightAngleLower = (float)Math.PI / 8;
        private float heightAngleUpper = (float)Math.PI * 3 / 8;
        private float distLower = 1;
        private float distUpper = 5;

        // Other view variables
        private Vector3 lookingAt, up;
        private Matrix view, projection;

        // Camera movement speeds
        private float panSpeed = 0.003f, heightPanSpeed = 0.001f, zoomSpeed = 0.002f;

        public Camera(Project1Game game, Vector3 position, Vector3 lookingAt, Vector3 up)
        {
            this.game = game;
            this.lookingAt = lookingAt;
            this.up = up;

            zoomDist = Bound(position.Length(), distLower, distUpper);
            angle = (float)(Math.Atan2(position.Y, position.X));
            Vector2 xy = new Vector2(position.X, position.Y);
            heightAngle = Bound((float)Math.Acos(xy.Length() / position.Length()), heightAngleLower, heightAngleUpper);

            view = Matrix.LookAtLH(position, lookingAt, up);
            projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = game.KeyboardState;
            MouseState mouseState = game.MouseState;
            int wheelScrollDist;
            int delta = gameTime.ElapsedGameTime.Milliseconds;
            
            int panDirection = 0, heightDirection = 0;

            // Panning around Z-axis
            if (keyboardState.IsKeyDown(Keys.Left))
                panDirection--;
            if (keyboardState.IsKeyDown(Keys.Right))
                panDirection++;

            // Panning up and down
            if (keyboardState.IsKeyDown(Keys.Up))
                heightDirection++;
            if (keyboardState.IsKeyDown(Keys.Down))
                heightDirection--;

            // Changing distance
            wheelScrollDist = mouseState.WheelDelta - mouseWheelLastOffset;
            mouseWheelLastOffset = mouseState.WheelDelta;
            zoomDist = Bound(zoomDist - wheelScrollDist * zoomSpeed, distLower, distUpper);

            angle += panDirection * panSpeed * delta;
            heightAngle = Bound(heightAngle + heightDirection * heightPanSpeed * delta, heightAngleLower, heightAngleUpper);

            // Calculate new position
            Vector3 position = Vector3.TransformCoordinate(Vector3.UnitX, Matrix.Scaling(zoomDist) * Matrix.RotationY(-heightAngle) * Matrix.RotationZ(-angle));

            // Update view
            view = Matrix.LookAtLH(position, lookingAt, up);

            // Make sure projection is up to date
            projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
        }

        // Bound the value x to [lower, upper] and return the result
        private float Bound(float x, float lower, float upper)
        {
            if (x > upper)
                return (upper > lower) ? upper : lower;
            else
                return (x > lower) ? x : lower;
        }

        public Matrix View()
        {
            return view;
        }

        public Matrix Projection()
        {
            return projection;
        }
    }
}