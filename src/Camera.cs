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

        //3d vector to store the camera's position in
        private Vector3 position;
        //the rotation around the Y axis of the camera
        private float yaw = 0.0f;
        //the rotation around the X axis of the camera
        private float pitch = 0.0f;

        public Vector3 getPosition()
        {
            return position;
        }
        public Matrix lookThrough()
        {
            Matrix cameraRotation = Matrix.RotationX(pitch) * Matrix.RotationY(yaw);

            Vector3 cameraOriginalTarget = new Vector3(0, -1, 0);
            Vector3 cameraRotatedTarget = Vector3.TransformCoordinate(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = position + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = Vector3.UnitZ;
            Vector3 cameraRotatedUpVector = Vector3.TransformCoordinate(cameraOriginalUpVector, cameraRotation);


            Matrix viewMatrix = Matrix.LookAtLH(position, cameraFinalTarget, cameraRotatedUpVector);

            return viewMatrix;
        }
        public Camera(float x, float y, float z)
        {
            //instantiate position Vector3f to the x y z params.
            position = new Vector3(x, y, z);
        }
        //increment the camera's current yaw rotation
        public void addYaw(float amount)
        {
            //increment the yaw by the amount param
            yaw += amount;
        }

        //increment the camera's current yaw rotation
        public void addPitch(float amount)
        {
            //increment the pitch by the amount param
            pitch += amount;
        }
        //moves the camera forward relative to its current rotation (yaw)
        public void walkForward(float distance)
        {
            Matrix cameraRotation = Matrix.Identity * Matrix.RotationX(pitch) * Matrix.RotationY(yaw);
            Vector3 cameraOriginalTarget = new Vector3(0, -1, 0);
            Vector3 cameraRotatedTarget = Vector3.TransformCoordinate(cameraOriginalTarget, cameraRotation);
            position += distance * cameraRotatedTarget;
        }

        //moves the camera backward relative to its current rotation (yaw)
        public void walkBackwards(float distance)
        {
            Matrix cameraRotation = Matrix.Identity * Matrix.RotationX(pitch) * Matrix.RotationY(yaw);
            Vector3 cameraOriginalTarget = new Vector3(0, -1, 0);
            Vector3 cameraRotatedTarget = Vector3.TransformCoordinate(cameraOriginalTarget, cameraRotation);
            position -= distance * cameraRotatedTarget;
        }

        //strafes the camera left relitive to its current rotation (yaw)
        public void strafeLeft(float distance)
        {
            position.X -= distance * (float)Math.Sin(yaw - Math.PI / 4);
            position.Z += distance * (float)Math.Cos(yaw - Math.PI / 4);
        }

        //strafes the camera right relitive to its current rotation (yaw)
        public void strafeRight(float distance)
        {
            position.X -= distance * (float)Math.Sin(yaw + Math.PI / 4);
            position.Z += distance * (float)Math.Cos(yaw + Math.PI / 4);
        }
    }
}