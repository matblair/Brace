using Brace.Physics;
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


        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect)
        {
            Matrix world = Matrix.RotationYawPitchRoll(rot.X, rot.Y, rot.Z) * Matrix.Translation(position);
            Matrix worldInvTranspose = Matrix.Transpose(Matrix.Invert(world));
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["worldInvTrp"].SetValue(worldInvTranspose);

            // Draw the models
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    //effect.Parameters["Texture"].SetResource(part.Effect.Parameters["Texture"].GetResource<Texture2D>());
                    effect.CurrentTechnique.Passes[0].Apply();
                    part.Draw(context);
                }
            }
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
