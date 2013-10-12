using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brace.PhysicsEngine
{
    public class PhysicsModel
    {
        public PhysicsBody bodyDefinition;
        public float invmass;
        public float mass;
        public Vector3 velocity;
        public Vector3 forces;
        public Vector3 position;
        public Vector3 location;
        public float restitution;
        public List<Contact> contacts;


        public void applyForce(Vector3 direction, float size)
        {
            forces += (direction * size);
        }
        public void applyImpulse(Vector3 direction, float size)
        {
            velocity += (direction * size);
        }
        public void move(Vector3 direction, float size)
        {
            position += (direction * size);
        }
        
        
    }
}
