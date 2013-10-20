using Brace.Physics;
using Brace.Utils;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class Player : Unit
    {
        PlayerController controller;
        Player(Vector3 position, Vector3 rotation)
            : base(position, rotation, Assets.cube, Assets.cubeTexture)
        {
            controller = new PlayerController(this);
        }
        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 1, 0.2f, 0.2f, position, Vector3.Zero, bodyDef,this);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
        public override void Update(GameTime gameTime)
        {
            controller.Update();

        }

        private void SwingSword()
        {
        }

        private void ShootArrow(Vector2 Direction)
        {
        }
        private void ChargeArrow()
        {
        }
        private void Move(Vector2 direction)
        {
        }
    }
}
