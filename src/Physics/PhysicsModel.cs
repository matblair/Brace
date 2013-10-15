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
        public Vector3 velocity;
        public Vector3 forces;
        public Vector3 position;
        public float restitution;
        public List<Contact> contacts;

        public void Initialize(float mass, float restitution, Vector3 iPosition, Vector3 iVelocity, PhysicsBody bodyDefinition)
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
            
            contacts = new List<Contact>();
        }

        public void ApplyForce(Vector3 direction, float size)
        {

            if (bodyDefinition.bodyType == BodyType.dynamic)
            {
                forces += (direction * size);
            }
            return;
            
        }
        public void ApplyImpulse(Vector3 direction, float size)
        {
            if (bodyDefinition.bodyType == BodyType.dynamic)
            {
                velocity += (direction * size);
            }
            return;
        }
            
        public void Move(Vector3 direction, float size)
        {
            if (bodyDefinition.bodyType == BodyType.dynamic)
            {
                position += (direction * size);
            }
            return;
         
        }

    }
}
