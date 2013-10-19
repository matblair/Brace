using Brace.Physics;
using Brace.Utils;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class Player : Unit
    {
        private int chargeTime;

        private readonly int SWORDATTACKSPEED = 800;
        private readonly int SWORDDAMAGE = 800;

        
        private readonly int MINIMUMCHARGETIME = 800;
        private readonly int MAXIMUMCHARGETIME = 1600;
        private readonly int MAXARROWDAMAGE = 70;
        private readonly int MINARROWDAMAGE = 10;


        private readonly int MAXSPEED = 1;

        private int health;
        private readonly int MAXHEALTH = 100;





        PlayerController controller;
        public Player(Vector3 position, Vector3 rotation)
            : base(position, rotation, Assets.cube)
        {
            controller = new PlayerController(this);
            chargeTime = 0;
            health = MAXHEALTH;

        }
        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 1, 0.2f, 0.2f, position, Vector3.Zero, bodyDef, this);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
        public override void Update(GameTime gameTime)
        {
            position = pObject.position;
            controller.Update(gameTime);
        }


        private void ShootArrow(Vector2 direction)
        {
            if (chargeTime < MINIMUMCHARGETIME)
            {
            }
            else if (chargeTime > MAXIMUMCHARGETIME)
            {
                BraceGame.get().AddActor(new Projectile(position, new Vector3(pObject.velocity.X, 0, pObject.velocity.Y) * 1, MAXARROWDAMAGE));
                chargeTime = 0;
            }
            else
            {
                int actualDamage = (MAXARROWDAMAGE - MINARROWDAMAGE) * (chargeTime - MINIMUMCHARGETIME) / (MAXIMUMCHARGETIME - MINIMUMCHARGETIME) + MINARROWDAMAGE;
                BraceGame.get().AddActor(new Projectile(position + new Vector3(direction.X, 0, direction.Y)*1, new Vector3(direction.X, 0, direction.Y), actualDamage));
                chargeTime = 0;
            }
            

        }
        private void ChargeArrow(GameTime gameTime)
        {
            chargeTime += gameTime.ElapsedGameTime.Milliseconds;
        }
        private void Move(Vector2 destination)
        {
            Vector2 direction = destination - new Vector2(position.X,position.Z);
            direction.Normalize();
            Vector2 targetSpeed = new Vector2(direction.X * MAXSPEED, direction.X * MAXSPEED);
            Vector3 force = pObject.mass * new Vector3(targetSpeed.X, 0, targetSpeed.Y);
            pObject.ApplyImpulse(force);
        }


        internal void lowerHealth(int damage)
        {
            health -= damage;
            if (health < 0)
            {
                die();
            }
        }

        private void die()
        {
            throw new NotImplementedException();
        }

    }
}
