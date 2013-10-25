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
    class HealthOrb : Unit
    {
        private int doomedTimer;
        public HealthOrb(Vector3 position) : base(position, Vector3.Zero, Assets.healthPickup, null)
        {
            BraceGame.get().AddLight(new TrackingLight(this, new Vector4(0.4f, 1.0f, 0.8f, 1), new Vector3(0, 0, 0), 0.0f, 0f, 2f, 2f, 2f));
        }
        public override void Update(SharpDX.Toolkit.GameTime gametime)
        {
            position = pObject.position;
            foreach(Contact contact in pObject.contacts) 
            {
                if (contact.y.parent.extraData.GetType() == typeof(Player))
                {
                    Player target = (Player)contact.y.parent.extraData;
                    target.addHealth(10);
                    BraceGame.get().StopTrackingProjectile(this);
                    DestroyPhysicsObject();
                    doomed = true;
                }
            }
        }

        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 0, 0.2f, 0.2f, position, Vector3.Zero, bodyDef);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
    }
}
