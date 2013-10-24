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
        
        private readonly int MINIMUMCHARGETIME = 100;
        private readonly int MAXIMUMCHARGETIME = 1600;
        public readonly int MAXARROWDAMAGE = 60;
        public readonly int MINARROWDAMAGE = 40;

        private readonly float MAXSPEED = 7;

        public bool isDead = false;
        public float decreasePerMs = 0.0023f;
        private float health;

        private readonly int MAXHEALTH = 100;

        PlayerController controller;

        public Player(Vector3 position, Vector3 rotation)
            : base(position, rotation, Assets.player, null)
        {
            controller = new PlayerController(this);
            chargeTime = 0;
            health = MAXHEALTH;
        }

        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(30, 0.3f, 0.01f, 0.7f, position, Vector3.Zero, bodyDef);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }

        public override void Update(GameTime gameTime)
        {
            position = pObject.position;
            controller.Update(gameTime);

            //Reduce lighting intensity as health goes down. 
            float intensityLost = gameTime.ElapsedGameTime.Milliseconds * decreasePerMs;
            lowerHealth(intensityLost);
            if (health < 0)
            {
                die(gameTime);
            }
        }

        public void ShootArrow(Vector2 direction)
        {
            direction.Normalize();

            if (chargeTime > MAXIMUMCHARGETIME)
            {
                Vector3 arrowIPosition = position + new Vector3(direction.X, 0, direction.Y) * 3;
                Projectile proj = new Projectile(arrowIPosition, new Vector3(direction.X, 0, direction.Y), MAXARROWDAMAGE);
                BraceGame.get().AddActor(proj);
                BraceGame.get().TrackProjectile(proj);

            }
            else if (chargeTime > MINIMUMCHARGETIME)
            {
                int actualDamage = (MAXARROWDAMAGE - MINARROWDAMAGE) * (chargeTime - MINIMUMCHARGETIME) / (MAXIMUMCHARGETIME - MINIMUMCHARGETIME) + MINARROWDAMAGE;
                Vector3 arrowIPosition = position + new Vector3(direction.X, 0, direction.Y)*3;
                Projectile proj = new Projectile(arrowIPosition, new Vector3(direction.X, 0, direction.Y), actualDamage);
                BraceGame.get().AddActor(proj);
                BraceGame.get().TrackProjectile(proj);
            }

            chargeTime = 0;
        }

        public void ChargeArrow(GameTime gameTime)
        {
            chargeTime += gameTime.ElapsedGameTime.Milliseconds;
        }

        public override void Move(Vector2 destination)
        {
            base.Move(destination);
            Vector2 currentLoc = new Vector2(position.X, position.Z);
            Vector2 toTarget = (destination - currentLoc);
            float dist = toTarget.Length();
            if (dist > 0)
            {
                float decel = 0.3f;
                float speed = dist / decel;
                speed = Math.Min(speed, MAXSPEED);
                Vector2 desiredVel = toTarget * speed / dist;
                pObject.ApplyImpulse(new Vector3(desiredVel.X - pObject.velocity.X, 0, desiredVel.Y - pObject.velocity.Z));
            }
            else
            {
                return;
            }
            

        }
        public void WalkForward() 
        {
            pObject.ApplyImpulse(ViewDirection() * MAXSPEED - pObject.velocity);

        }

        internal void lowerHealth(float damage)
        {
            health -= damage;

        }

        private void die(GameTime gameTime)
        {
            isDead = true;
        }

        public Vector4 getIntensityVector(){
            float factor = health/MAXHEALTH;
            Vector4 intensity = new Vector4(factor, factor, factor, 1);
            return intensity;
        }


        internal void reinitialiseHealth(int p)
        {
            this.health = p;
        }

        internal void addHealth(int p)
        {
            health += p;
            return;
        }
    }
}
