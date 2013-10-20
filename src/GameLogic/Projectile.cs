using Brace.GameLogic;
using Brace.Physics;
using Brace.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class Projectile : Unit
    {
        Vector3 direction;
        private readonly int DEATHCOUNT = 600;
        private int deathCounter = 0;
        int damage;
        public Projectile(Vector3 position, Vector3 direction, int damage)
            : base(position, direction, Assets.cube, Assets.cubeTexture)
        {
            
            this.direction = direction;
            this.direction.Normalize();
            this.damage = damage;
            pObject.velocity = direction * damage;

        }
        public override void Update(SharpDX.Toolkit.GameTime gametime)
        {
            position = pObject.position;
            if (dying())
            {
                deathCounter += gametime.ElapsedGameTime.Milliseconds;
                if (deathCounter > DEATHCOUNT)
                {
                    die();
                }
            }
            else
            {
                CheckCollision();
            }
            
            
        }

        private bool dying()
        {
            Vector2 speed = new Vector2(pObject.velocity.X, pObject.velocity.Z);
            return (speed.Length()<3);
        }
        private void die()
        {
            DestroyPhysicsObject();
            doomed = true;
        }
        private void CheckCollision()
        {
           

            foreach (Contact contact in pObject.contacts)
            {
                if (contact.x.parent.extraData.Equals(this))
                {

                    if (contact.y.parent.extraData.GetType() == typeof(Enemy))
                    {
                        ((Enemy)contact.y.parent.extraData).lowerHealth(damage);
                        die();
                    }
                }
                else
                {
                    if (contact.x.parent.extraData.GetType() == typeof(Enemy))
                    {
                        ((Enemy)contact.x.parent.extraData).lowerHealth(damage);
                        die();
                    }

                }
            }
        }


        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 0, 0.7f, 0, position, Vector3.Zero, bodyDef);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
    }
}
