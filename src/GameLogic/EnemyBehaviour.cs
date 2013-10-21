using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    public class EnemyBehaviour
    {
        static Random rand = new Random();
        private Vector2 dir;
        private readonly float RADIUS = 1;
        private readonly float DIST = 10;

        public EnemyBehaviour()
        {
            double theta = rand.NextDouble(0, Math.PI * 2);
            dir = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
        }

        public Vector2 Wander()
        {
            double theta = rand.NextDouble(0, Math.PI * 2);
            dir = Vector2.Add(dir * DIST, RADIUS * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)));
            dir.Normalize();
            return dir;
        }
    }
}
