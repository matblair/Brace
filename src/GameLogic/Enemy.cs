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

        private readonly int MAXSPEED = 1;
        private readonly int MAXHEALTH = 100;

        Enemy(Vector3 position, Vector3 rotation)
            : base(position, rotation, Assets.cube)
        {
            controller = new EnemyController(this);
        }
        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 1, 0.2f, 0.2f, position, Vector3.Zero, bodyDef,this);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
        public override void Update(GameTime gameTime)
        {
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
        public void Move(Vector2 destination)
        {
            Vector2 direction = destination - new Vector2(position.X, position.Z);
            direction.Normalize();
            Vector2 targetSpeed = new Vector2(direction.X * MAXSPEED, direction.X * MAXSPEED);
            Vector3 force = pObject.mass * new Vector3(targetSpeed.X, 0, targetSpeed.Y);
            pObject.ApplyImpulse(force);
        }
    }
}
