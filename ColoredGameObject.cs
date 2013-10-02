using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    abstract public class ColoredGameObject : GameObject
    {
        public Buffer<VertexPositionColor> vertices;

        public override void Draw(GameTime gametime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        protected void GenerateBasicEffect()
        {
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                Projection = game.camera.Projection(),
                VertexColorEnabled = true
            };
        }
    }
}
