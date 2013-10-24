using Brace.Physics;
using Brace.Utils;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class Enemy : Unit
    {
        EnemyController controller;

        private readonly int DAMAGE = 1;
        private readonly int ATTACKCOOLDOWN = 1000;
        private int attackCounter;

        private readonly int MAXSPEED = 5;
        private readonly int MAXHEALTH = 1;
        private int health;

        public Enemy(Vector3 position, Vector3 rotation)
           : base(position, rotation, Assets.cube, null)
        {
            health = MAXHEALTH;
            controller = new EnemyController(this);
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
        }

        public void Attack(Player target)
        {
            if (attackCounter > ATTACKCOOLDOWN)
            {
                target.lowerHealth(DAMAGE);
                attackCounter = 0;
            }
            return;
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

        internal void lowerHealth(int damage)
        {
            health -= damage;
            if (health < 0)
            {
                BraceGame.get().AddActor(new HealthOrb(position));
                die();
            }
        }

        public void die()
        {
            DestroyPhysicsObject();
            doomed = true;
        } 
    }
}
