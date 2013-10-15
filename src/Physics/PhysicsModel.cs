using Brace.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brace.Physics
{
    public class PhysicsModel
    {
        public PhysicsBody bodyDefinition;
        public float invmass;
        public float mass;
        public float linearDamp;
        public Vector3 velocity;
        public Vector3 forces;
        public Vector3 position;
        public float restitution;
        public List<Contact> contacts;
        public float friction;

        public void Initialize(float mass, float restitution, float linearDamp, float friction, Vector3 iPosition, Vector3 iVelocity, PhysicsBody bodyDefinition)
        {
            this.mass = mass;
            this.restitution = restitution;
            position = iPosition;
            velocity = iVelocity;
            this.bodyDefinition = bodyDefinition;
            if (mass == 0)
            {
                invmass = 0;
            }
            else 
            {
                invmass = 1 / mass;
            }
            this.linearDamp = linearDamp;
            this.friction = friction;
            contacts = new List<Contact>();
        }

        public void ApplyForce(Vector3 force)
        {

            if (bodyDefinition.bodyType == BodyType.dynamic)
            {
                forces += (force);
            }
            return;
            
        }
        public void ApplyImpulse(Vector3 impulse)
        {
            if (bodyDefinition.bodyType == BodyType.dynamic)
            {
                velocity += (impulse);
            }
            return;
        }
            
        public void Move(Vector3 amount)
        {
            if (bodyDefinition.bodyType == BodyType.dynamic)
            {
                position += (amount);
            }
            return;
         
        }

    }
}
