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
        List<PhysicsModel> bodies;
        public void step(float dt){
            MoveObjects(dt);
            CheckForCollisions();
            ResolveCollisions(dt);
        }

        private void ResolveCollisions(float dt)
        {
            throw new NotImplementedException();
        }


        private void MoveObjects(float dt)
        {

            foreach (PhysicsModel body in bodies)
            {
                UpdateForces(body,dt);
                MoveBody(body,dt);
              
            }

        }

        private void MoveBody(PhysicsModel body, float dt)
        {
            
            Vector3 dist = VectorUtils.mul(body.velocity, dt);
            body.location = VectorUtils.add(body.location, dist);
        }

        private void UpdateForces(PhysicsModel body, float dt)
        {
            Vector3 dv = VectorUtils.mul(body.forces, body.invmass);
            dv = VectorUtils.mul(dv, dt);
            body.velocity = VectorUtils.add(body.velocity, dv);
        }
        List<Contact> contacts;
        private void CheckForCollisions()
        {
            contacts.RemoveAll(contact=>true);
            for (int i=0; i<bodies.Count;++i) {
                PhysicsModel target = bodies[i];
                for (int j = i; j < bodies.Count; ++j)
                {
                    PhysicsModel body = bodies[j];
                    if (CheckCollision(target, body))
                    {
                        contacts.Add(new Contact(target, body));
                    }

                }
            
            }

        }


        private bool CheckCollision(PhysicsModel target, PhysicsModel body)
        {
            if (target.GetType() == typeof(Terrain))
            {

            }
            else if (body.GetType() == typeof(Terrain))
            {

            }
            else
            {
                SpheresBody x = (SpheresBody)target.bodyDefinition;
                SpheresBody y = (SpheresBody)body.bodyDefinition;
                Vector3 aTrans = x.position;
                Vector3 bTrans = y.position;
                for (int i = 0; i < x.spheres.Count; ++i)
                {
                    for (int j = 0; j < y.spheres.Count; ++j)
                    {
                        Sphere a = x.spheres[i];
                        Sphere b = y.spheres[j];
                        Vector3 aLoc = VectorUtils.add(a.position, aTrans);
                        Vector3 bLoc = VectorUtils.add(b.position, bTrans);
                        Vector3 distance = VectorUtils.sub(aLoc,bLoc);
                        float mag2 = VectorUtils.mag2(distance);
                        float rad2 = (a.radius+b.radius) * (a.radius+b.radius);
                        if (mag2 < rad2)
                        {
                            return true;
                        }
                    }
                }

                return false;
                

            }
            throw new NotImplementedException();
            

        }
        private void addBody(PhysicsModel obj) 
        {
            bodies.Add(obj);

        }
        private void removeBody(PhysicsModel obj)
        {
            bodies.Remove(obj);
        }
    }
}
