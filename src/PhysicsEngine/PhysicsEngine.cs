using Brace.Utils;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.PhysicsEngine
{
    
    class PhysicsEngine
    {
        enum CollisionType
        {
            Terrain,
            Sphere
        }

        List<PhysicsModel> bodies;
        public void step(float dt){
            
            CheckForCollisions();
            ResolveCollisions(dt);
            MoveObjects(dt);

        }

        private void ResolveCollisions(float dt)
        {
            ImpulseResolution();
            ApplyFriction();
            PositionalCorrection();
        }

        private void ApplyFriction()
        {
            
        }

        private void PositionalCorrection()
        {
            foreach (Contact contact in contacts)
            {
                const float percentageOfIntersection = 0.02f;
                float movedist = contact.distance*percentageOfIntersection;
                PhysicsModel body1 = contact.x.parent;
                PhysicsModel body2 = contact.y.parent;
                body1.move(contact.normal, movedist);
                body2.move(contact.normal, -movedist);
            }
        }

        private void ImpulseResolution()
        {
            foreach (Contact contact in contacts)
            {
                PhysicsModel body1 = contact.x.parent;
                PhysicsModel body2 = contact.y.parent;
                Vector3 relativeVelBefore = body1.velocity - body2.velocity;
                float contactVelocity = Vector3.Dot(relativeVelBefore, contact.normal);

                //nothing to solve
                if (contactVelocity > 0)
                {
                    return;
                }
                //Get Resutitution
                float restitution = Math.Min(body1.restitution, body2.restitution);
                //Get Impulse
                float impulseScalar = -(1 + restitution) * contactVelocity;
                impulseScalar /= body1.invmass + body2.invmass;

                //Apply Impulses
                body1.applyImpulse(contact.normal, impulseScalar);
                body2.applyImpulse(contact.normal, impulseScalar);
            }
        }

        private void MoveObjects(float dt)
        {

            foreach (PhysicsModel body in bodies)
            {
                UpdateForces(body, dt);
                for (int i = 0; i < numIterations; ++i)
                {
                    UpdateVelocity(body, dt);
                    MoveBody(body, dt);
                }
              
            }

        }

        private void UpdateForces(PhysicsModel body, float dt)
        {
            //Apply global forces suchas gravity linear damp etc...
        }

        private void MoveBody(PhysicsModel body, float dt)
        {
            
            Vector3 dist = body.velocity * dt;
            body.location = body.location + dist;
        }

        private void UpdateVelocity(PhysicsModel body, float dt)
        {
            Vector3 dv = body.forces * body.invmass;
            body.forces = Vector3.Zero;
            body.velocity = body.velocity + dv * dt;
        }

        List<Contact> contacts;
        private const int numIterations = 5;
        private void CheckForCollisions()
        {
            contacts.Clear();
            foreach(PhysicsModel body in bodies)
            {
                body.contacts.Clear();
            }
            for (int i=0; i<bodies.Count;++i) {
                PhysicsModel target = bodies[i];
                for (int j = i; j < bodies.Count; ++j)
                {
                    PhysicsModel body = bodies[j];
                    Contact newContact = CheckCollision(target, body);
                    if (newContact!=null)
                   {
                        contacts.Add(newContact);
                        target.contacts.Add(newContact);
                        body.contacts.Add(newContact);
                    }
                }
            }
        }


        private Contact CheckCollision(PhysicsModel target, PhysicsModel body)
        {
            
            CollisionType collisionType;
            Contact result = null;
            bool isTargetTerrain = false;

            //ASSUMPTION TERRAIN WON'T COLLIDE WITH OTHER TERRAIN!!!
            if (target.GetType() == typeof(TerrainBody))
            {
                isTargetTerrain=true;
                collisionType = CollisionType.Terrain;
            } else if (body.GetType() == typeof(TerrainBody)) {
                isTargetTerrain=false;
                collisionType = CollisionType.Terrain;
            } else {
                collisionType = CollisionType.Sphere;
            }
            switch(collisionType) 
            {
                case CollisionType.Terrain:
                    TerrainBody a;
                    SpheresBody b;
                    if(isTargetTerrain)
                    {
                        a = (TerrainBody)target.bodyDefinition;
                        b = (SpheresBody)body.bodyDefinition;
                    } else {
                        b = (SpheresBody)target.bodyDefinition;
                        a = (TerrainBody)body.bodyDefinition;

                    }
                    result = CheckTerrainCollision(a, b);
                    break;
                case CollisionType.Sphere:
                    SpheresBody x = (SpheresBody)target.bodyDefinition;
                    SpheresBody y = (SpheresBody)body.bodyDefinition;
                    result = CheckSphereCollision(x, y);               
                    break;
            }
            return result;            
        }

        private Contact CheckTerrainCollision(TerrainBody a, SpheresBody b)
        {
            Sphere s = GetLowestSphere(b);
            Vector3 up = a.up;
            Vector3 lowestPoint = s.position + b.position - (a.up * s.radius);
            Vector3 targetPoint = GetClosestPoint(a,lowestPoint);
            Contact result = null;
            
            if (BelowPoint(lowestPoint,targetPoint))
            {
                result = new Contact(a, targetPoint, b,lowestPoint);
            }

            return result;
        }

        private Vector3 GetClosestPoint(TerrainBody a, Vector3 lowestPoint)
        {
            float[,] segments = a.points;
            float xzScale = a.xzScale;
            int i = (int)((0.5f + lowestPoint.X / xzScale / 2) * segments.GetLength(0));
            int j = (int)((0.5f + lowestPoint.Y / xzScale / 2) * segments.GetLength(1));
            return new Vector3(i,segments[i, j],j);
        }

        private bool BelowPoint(Vector3 a, Vector3 b)
        {
            if (a.Y - b.Y > 0)
            {
                return true;
            }
            return false;
        }

        private Sphere GetLowestSphere(SpheresBody body)
        {
            Sphere lowest = body.spheres[0];
            List<Sphere> spheres = body.spheres;
            for (int i = 0; i < spheres.Count; ++i)
            {
                if (lowest.position.Y > spheres[i].position.Y)
                {
                    lowest = spheres[i];
                }
            }
            return lowest;
        }

        private Contact CheckSphereCollision(SpheresBody x, SpheresBody y)
        {
            Vector3 aTrans = x.position;
            Vector3 bTrans = y.position;
            for (int i = 0; i < x.spheres.Count; ++i)
            {
                for (int j = 0; j < y.spheres.Count; ++j)
                {
                    Sphere a = x.spheres[i];
                    Sphere b = y.spheres[j];
                    Vector3 aLoc = a.position + aTrans;
                    Vector3 bLoc = b.position + bTrans;
                    Vector3 direction = aLoc - bLoc;
                    double mag2 = VectorUtils.mag2(direction);
                    double rad2 = (a.radius + b.radius) * (a.radius + b.radius);
                    if (mag2 < rad2)
                    {
                        float temp = (float)(Math.Sqrt(rad2) - Math.Sqrt(mag2));
                        direction.Normalize();
                        return new Contact(x,y,direction,temp);
                    }
                }
            }
            return null;
        }

        private void AddBody(PhysicsModel obj) 
        {
            bodies.Add(obj);
        }

        private void RemoveBody(PhysicsModel obj)
        {
            bodies.Remove(obj);
        }
    }
}
