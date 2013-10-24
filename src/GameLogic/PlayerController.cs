using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class PlayerController : Controller
    {
        private bool shooting = false;
        private Vector2 shootingDirection;
        private Vector2 moveLocation;

        public PlayerController(Player player) : base(player)
        { 
        }

        public override void Update(GameTime gameTime)
        {
            if (BraceGame.get().input.isShooting())
            {
                shooting = true;
                shootingDirection = BraceGame.get().input.shotDirection();
                ((Player)target).ChargeArrow(gameTime);
            }

            else if (shooting == true)
            {
                ((Player)target).ShootArrow(shootingDirection);
                shooting = false;
            }
            float turnAmmount = gameTime.ElapsedGameTime.Milliseconds / (50 * (float)Math.PI * 2);
            if (BraceGame.get().input.TurningRight())
            {
                ((Player)target).rot.X += turnAmmount;
            }
            if (BraceGame.get().input.TurningLeft())
            {
                ((Player)target).rot.X -= turnAmmount;
            }
            
            if (BraceGame.get().input.WalkingForward())
            {
                ((Player)target).WalkForward();
            }
            else
            {

                
                moveLocation = BraceGame.get().input.moveTo();
                ((Player)target).Move(moveLocation);
            }
        }
    }
}
