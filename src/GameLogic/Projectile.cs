using Brace.GameLogic;
using Brace.Physics;
using Brace.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.src.GameLogic
{
    class Projectile : Unit
    {
        Vector3 direction;
        Projectile(Vector3 position, Vector3 direction)
            : base(position, direction, Assets.cube)
        {
            
            this.direction = direction;
            this.direction.Normalize();

        }
        public override void Update(SharpDX.Toolkit.GameTime gametime)
        {
            Move();
            CheckCollision();
        }

        private void CheckCollision()
        {
            List<Contact> contacts = pObject.contacts;
            foreach (Contact contact in contacts)
            {
                               
            }
        }

        private void Move()
        {
            pObject.ApplyForce(direction);
        }

        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 1, 0.2f, 0.2f, position, Vector3.Zero, bodyDef, this);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
        private void Destroy()
        {
            BraceGame.get().physicsWorld.RemoveBody(pObject);
        }
    }
}
