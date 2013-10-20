﻿using Brace.Physics;
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
    class Cube : Unit
    {
        public Cube(Vector3 position) : base(position, Vector3.Zero, Assets.cube, Assets.cubeTexture)
        {
            
        }
        protected override void InitializePhysicsObject() {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(1, 1, 0.2f, 0.2f, position, Vector3.Zero, bodyDef,this);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }
        public Cube(Vector3 position, bool passive)
            : base(position, Vector3.Zero, Assets.cube, Assets.cubeTexture)
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            BraceGame.get().physicsWorld.AddBody(pObject);

            if (passive)
            {
                pObject.bodyDefinition.bodyType = BodyType.stationary;
            }
            else
            {
                pObject.bodyDefinition.bodyType = BodyType.dynamic;
            }

        }

        public override void Update(GameTime gameTime)
        {

            this.position = pObject.position;

            
        }
    }
}
