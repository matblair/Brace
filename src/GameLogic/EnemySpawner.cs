using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class EnemySpawner : Actor
    {
        Player target;
        EnemySpawner(Player target)
            : base(Vector3.Zero, Vector3.Zero) 
        {
            this.target = target;

        }
        public override void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect)
        {
            return;
        }
    }
}
