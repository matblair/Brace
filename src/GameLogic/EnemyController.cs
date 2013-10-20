using Brace.Physics;
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
            foreach(Contact contact in ((Enemy)target).pObject.contacts) {
                if (typeof(Player) == contact.x.parent.extraData.GetType()||typeof(Player) == contact.y.parent.extraData.GetType())
                {
                    ((Enemy)target).Attack(BraceGame.get().getPlayer());
                    ((Enemy)target).die();
                    break;
                }
            }
        }
    }
}
