using Brace.GameLogic;
using Brace.Physics;
using Brace.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class Projectile : Unit
    {
        Vector3 direction;
        private readonly int SPEED = 10;
        int damage;
        public Projectile(Vector3 position, Vector3 direction, int damage)
            : base(position, Vector3.Zero, Assets.cube)
        {
            
            this.direction = direction;
            this.direction.Normalize();
            this.damage = damage;

        }
        public override void Update(SharpDX.Toolkit.GameTime gametime)
        {
            position = pObject.position;
            Move();
            CheckCollision();
        }

        private void CheckCollision()
        {
            
        }

        private void Move()
        {
            pObject.ApplyImpulse(direction * SPEED);
        }

        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 1, 0.2f, 0.2f, position, Vector3.Zero, bodyDef, this);
            pObject.bodyDefinition.bodyType = BodyType.passive;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
        private void Destroy()
        {
            BraceGame.get().physicsWorld.RemoveBody(pObject);
        }
    }
}
