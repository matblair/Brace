using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;


public struct PointLight
{
    public float Ka;
    public float Kd;
    public float Ks;
    public float fallOffTop;
    public float specN;
    public float x;
    public float y;
    public float z;
    public float r;
    public float g;
    public float b;
};

namespace Brace
{
    public class Light
    {
        // Our shading information
        public PointLight shadingLight;
        // Our world lighting setups Will obviously need to be changed at a later 
        // date in order to properly having moving light sources etc.
        public Vector4 lightPntCol;
        public Vector3 lightPntPos;
        public Vector4 intensityVector = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        //Whether or not the light is on or off
        protected bool isVisible;

        public Light()
        {

        }

        public Light(Vector3 position, Vector4 pntColour, float Ka, float Kd, float Ks, float specN, float fallOffTop)
        {
            this.lightPntPos = position;
            this.lightPntCol = pntColour;
            shadingLight.x = position.X;
            shadingLight.y = position.Y;
            shadingLight.z = position.Z;

            shadingLight.r = pntColour.X;
            shadingLight.g = pntColour.Y;
            shadingLight.b = pntColour.Z;
            shadingLight.Ka = Ka;
            shadingLight.Kd = Kd;
            shadingLight.Ks = Ks;
            shadingLight.specN = specN;
            shadingLight.fallOffTop = fallOffTop;

        }

        public Light( Vector4 pntColour, float Ka, float Kd, float Ks, float specN, float fallOffTop)
        {
            this.lightPntPos = Vector3.Zero;
            this.lightPntCol = pntColour;
            shadingLight.x = 0;
            shadingLight.y = 0;
            shadingLight.z = 0;

            shadingLight.r = pntColour.X;
            shadingLight.g = pntColour.Y;
            shadingLight.b = pntColour.Z;
            shadingLight.Ka = Ka;
            shadingLight.Kd = Kd;
            shadingLight.Ks = Ks;
            shadingLight.specN = specN;
            shadingLight.fallOffTop = fallOffTop;
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

          // Update the camera and associated things
        public virtual void Update(GameTime gameTime)
        {
        }

        public static PointLight getNullLight()
        {
            PointLight nullLight;
            nullLight.x = 0;
            nullLight.y = 0;
            nullLight.z = 0;

            nullLight.r = 0;
            nullLight.g = 0;
            nullLight.b = 0;
            nullLight.Ka = 0;
            nullLight.Kd = 0;
            nullLight.Ks = 0;
            nullLight.specN = 0;
            nullLight.fallOffTop = 0;
            return nullLight;
        }
    }
}
