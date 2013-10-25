﻿using Brace.Physics;
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
    abstract public class Unit : Actor, ITrackable
    {
        private Model model;
        private Texture2D texture;
        public PhysicsModel pObject;

        public Unit(Vector3 position, Vector3 rotation, Model model, Texture2D text)
            : base(position, rotation)
        {
            this.model = model;
            InitializePhysicsObject();
            this.texture = text;
            pObject.extraData = this;
        }

        public abstract override void Update(GameTime gametime);
        protected abstract void InitializePhysicsObject();

        public virtual void Move(Vector2 destination)
        {
            Vector2 dir = Vector2.Subtract(destination, new Vector2(position.X, position.Z));
            if (dir.Length() > 1)
            {
                float angle = (float)Math.Atan2(dir.X, dir.Y);
                SetRotPitch(angle);
            }
        }

        public void DestroyPhysicsObject()
        {
            BraceGame.get().physicsWorld.RemoveBody(pObject);
        }


        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect, List<Light> lights)
        {
            //Set up the render operation
            Matrix world = Matrix.RotationYawPitchRoll(rot.X, rot.Y, rot.Z) * Matrix.Translation(position);
            Matrix worldInvTranspose = Matrix.Transpose(Matrix.Invert(world));
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["worldInvTrp"].SetValue(worldInvTranspose);
            //context.SetDepthStencilState(context.DepthStencilStates.None);

//           Draw the models
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    ////Draw the rest as additive
                    //for (int i = 0; i < lights.Count(); i++)
                    //{
                    //    if(i==0){
                    //        context.SetBlendState(context.BlendStates.Opaque);
                    //    } else {
                    //        context.SetBlendState(context.BlendStates.Additive);
                    //    }

                      //  effect.Parameters["light"].SetValue(lights[i].shadingLight);
                        effect.CurrentTechnique.Passes[0].Apply();

                        part.Draw(context);
                    //}
                }
            }
          
            //Rest our system
            //context.SetDepthStencilState(context.DepthStencilStates.Default);
            //context.SetBlendState(context.BlendStates.Opaque);
        }

        public Vector3 ViewDirection()
        {
            // Might need to negate rot.X
            return Vector3.TransformCoordinate(Vector3.UnitZ, Matrix.RotationAxis(Vector3.UnitY, rot.X));
        }

        public Vector3 BodyLocation()
        {
            return position;
        }

        public Vector3 EyeLocation()
        {
            return position;
        }


    }
}
