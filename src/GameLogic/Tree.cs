using Brace.Physics;
using Brace.Utils;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class Tree : Unit
    {
        public Tree(Vector3 position)
            : base(position, Vector3.Zero, Assets.tree, null)
        {

        }
        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, true, 1, Vector3.UnitY);
            pObject.Initialize(1, 0.1f, 0.2f, 0.2f, position, Vector3.Zero, bodyDef);
            pObject.bodyDefinition.bodyType = BodyType.stationary;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
