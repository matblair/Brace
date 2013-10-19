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
        public object extraData;

        public void Initialize(float mass, float restitution, float linearDamp, float friction, Vector3 iPosition, Vector3 iVelocity, PhysicsBody bodyDefinition, Object extraData)
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
            this.extraData = extraData;
        }

        public void ApplyForce(Vector3 force)
        {

            if (bodyDefinition.bodyType == BodyType.stationary || bodyDefinition.bodyType == BodyType.terrain)
            {
                return;
            }
            forces += (force);            
        }
        public void ApplyImpulse(Vector3 impulse)
        {
            if (bodyDefinition.bodyType == BodyType.stationary || bodyDefinition.bodyType == BodyType.terrain)
            {
                return;
            }
            velocity += (impulse);
        }
            
        public void Move(Vector3 amount)
        {
            if (bodyDefinition.bodyType == BodyType.stationary||bodyDefinition.bodyType==BodyType.terrain)
            {
                return;
            }
            position += (amount);
        }

    }
}
