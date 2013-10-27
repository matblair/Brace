using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Brace.Utils
{
    class Assets
    {
        public static Model spaceship;
        public static Model cube;
        public static Model tree;
        public static Model player;
        public static Model bullet;
        public static Model healthPickup;
        public static Model smalltree;

        //Lighting information for varying feature levels 
        public static Vector4 playerColour;
        public static Vector3 playerOffset;
        public static float playerKa;
        public static float playerKs;
        public static float playerKd;
        public static float playerSpecN;
        public static float playerOnTop;

        //The values we use to create the sun
        public static Vector4 sunColour;
        public static Vector3 sunOffset;
        public static float sunKa;
        public static float sunKs;
        public static float sunKd;
        public static float sunSpecN;
        public static float sunOnTop;

        //The values we use to create the missile lamps
        public static Vector4 missileColour;
        public static Vector3 missileOffset;
        public static float missileKa;
        public static float missileKs;
        public static float missileKd;
        public static float missileSpecN;
        public static float missileOnTop;

        //The values we use to create the health lamps
        public static Vector4 healthColour;
        public static Vector3 healthOffSet;
        public static float healthKa;
        public static float healthKs;
        public static float healthKd;
        public static float healthSpecN;
        public static float healthOnTop;

    }
}
