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
        private readonly float CHASEDIST = 10;
        private EnemyBehaviour behaviour;

        public EnemyController(Enemy enemy)
            : base(enemy)
        {
            behaviour = new EnemyBehaviour();
        }
        private float DistanceFromPlayerSquared() {
            Player player = BraceGame.get().getPlayer();
            Vector2 thisLoc = new Vector2(target.position.X, target.position.Z);
            Vector2 playerLoc = new Vector2(player.position.X, player.position.Z);
            return Vector2.DistanceSquared(thisLoc, playerLoc);
        }

        public override void Update(GameTime gameTime)
        {
            Player player = BraceGame.get().getPlayer();
            Vector2 thisLoc = new Vector2(target.position.X, target.position.Z);
            Vector2 playerLoc = new Vector2(player.position.X, player.position.Z);

            if (DistanceFromPlayerSquared() > 100 * CHASEDIST * CHASEDIST)
            {
                target.doomed = true;
                target.DestroyPhysicsObject();
            }
            if (DistanceFromPlayerSquared() > CHASEDIST * CHASEDIST)
            {
                target.Move(Vector2.Add(thisLoc, behaviour.Wander() * 10));
            }
            else
            {
                target.Move(playerLoc);
            }

            foreach (Contact contact in target.pObject.contacts)
            {
                if (typeof(Player) == contact.x.parent.extraData.GetType() || typeof(Player) == contact.y.parent.extraData.GetType())
                {
                    ((Enemy)target).Attack(BraceGame.get().getPlayer());
                    ((Enemy)target).die();
                    break;
                }
            }
        }
    }
}
