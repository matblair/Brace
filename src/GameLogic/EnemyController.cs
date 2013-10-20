using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class EnemyController : Controller
    {

        public EnemyController(Enemy enemy) : base(enemy){}

        public override void Update(GameTime gameTime)
        {
            Player player = BraceGame.get().getPlayer();
            ((Enemy)target).Move(new Vector2(player.position.X,player.position.Z));
        }
    }
}
