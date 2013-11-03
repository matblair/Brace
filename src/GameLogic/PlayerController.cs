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
            if (BraceGame.get().input.hasOrientationSupport)
            {
                player.hasRotationSupport = true;
            }
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

            //If we are using on screen controls for fps.
            if (!BraceGame.get().input.hasOrientationSupport)
            {
                //Calculate the turn amount
                float turnAmmount = gameTime.ElapsedGameTime.Milliseconds / (50 * (float)Math.PI * 2);
                //Then turn byt whichever direction is being pressed.
                if (BraceGame.get().input.TurningRight())
                {
                    ((Player)target).rot.X += turnAmmount;
                }
                if (BraceGame.get().input.TurningLeft())
                {
                    ((Player)target).rot.X -= turnAmmount;
                }
            }
            //else if(BraceGame.get().input.hasOrientationSupport && BraceGame.get().Camera.CurrentViewType==Camera.ViewType.FirstPerson)
            //{
                
            //}
            
            //If we are walking forward.
            if (BraceGame.get().input.WalkingForward())
            {
                ((Player)target).WalkForward();
            }
            //Otherwise we are in top down so move to that location
            else if(BraceGame.get().Camera.CurrentViewType == Camera.ViewType.TopDown)
            {
                moveLocation = BraceGame.get().input.moveTo();
                ((Player)target).Move(moveLocation);
            }
        }
    }
}
