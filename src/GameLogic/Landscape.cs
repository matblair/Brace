using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Project1.src.GameLogic
{
    using SharpDX.Toolkit.Graphics;
    using Project1.src;
    class Landscape : GameObject
    {
        Random rnd;
        int size = 33;
        float diff = 0.3f;
        BasicEffect basicEffect;
        public Landscape(Vector3 position, Vector3 rotation, RenderModel rObject, PhysicsModel pObject)
        {
            rnd = new Random();
            rObject.bodyDefinition =CreateTerrain();
            Buffer.Vertex.New(
                Brace.get().graphicsDeviceManager.GraphicsDevice,
                rObject.bodyDefinition);
            basicEffect = new BasicEffect(Brace.get().graphicsDeviceManager.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, size * (1 + diff), 0), new Vector3(0, 0, 0), Vector3.UnitZ),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)Brace.get().graphicsDeviceManager.GraphicsDevice.BackBuffer.Width / Brace.get().graphicsDeviceManager.GraphicsDevice.BackBuffer.Height, 0.1f, size * (1 + diff)),
                World = Matrix.Identity
            };
        }

        private VertexPositionColor[] CreateTerrain()
        {
            float v1, v2, v3, v4;
            v1 = rnd.NextFloat(0, size);
            v2 = rnd.NextFloat(0, size);
            v3 = rnd.NextFloat(0, size);
            v4 = rnd.NextFloat(0, size);

            float[,] heights = new float[size, size];
            heights[0, 0] = v1;
            heights[0, size - 1] = v2;
            heights[size - 1, 0] = v3;
            heights[size - 1, size - 1] = v4;
            GeneratePlasmaMap(heights, new Point(0, size - 1), new Point(size - 1, 0));
            var array = HeightsToVerticies(heights, size, size);
            var terrain = ColouriseTerrain(array);
            return terrain;

        }
        private VertexPositionColor[] ColouriseTerrain(Vector3[] array)
        {

            VertexPositionColor[] terrain = new VertexPositionColor[array.Length];

            float maxHeight = size * (1 + diff);
            for (int i = 0; i < array.Length; ++i)
            {
                Color c = Color.Black;
                if (array[i].Y < maxHeight * 0.1)
                {
                    c = Color.DarkBlue;
                }
                else if (array[i].Y < maxHeight * 0.2)
                {
                    c = Color.Blue;
                }
                else if (array[i].Y < maxHeight * 0.3)
                {
                    c = Color.YellowGreen;
                }
                else if (array[i].Y < maxHeight * 0.4)
                {
                    c = Color.Yellow;
                }
                else if (array[i].Y < maxHeight * 0.5)
                {
                    c = Color.GreenYellow;
                }
                else if (array[i].Y < maxHeight * 0.6)
                {
                    c = Color.ForestGreen;
                }
                else if (array[i].Y < maxHeight * 0.8)
                {
                    c = Color.DarkGray;
                }
                else
                {
                    c = Color.White;
                }
                terrain[i] = new VertexPositionColor(array[i], c);
            }
            return terrain;
        }
        private Vector3[] HeightsToVerticies(float[,] heights, int width, int height)
        {
            var arraySize = ((width) * (height - 1) * 6);
            var array = new Vector3[arraySize];
            for (int i = 0; i < width - 1; ++i)
            {
                for (int j = 0; j < height - 1; ++j)
                {
                    array[6 * (i + width * j)] = new Vector3((float)i - size / 2f, heights[i, j], (float)j - size / 2f);
                    array[2 + 6 * (i + width * j)] = new Vector3((float)i - size / 2f + 1, heights[i + 1, j], (float)j - size / 2f);
                    array[1 + 6 * (i + width * j)] = new Vector3((float)i - size / 2f, heights[i, j + 1], (float)j + 1 - size / 2f);

                    array[3 + 6 * (i + width * j)] = new Vector3((float)i - size / 2f + 1, heights[i + 1, j], (float)j - size / 2f);
                    array[5 + 6 * (i + width * j)] = new Vector3((float)i + 1 - size / 2f, heights[i + 1, j + 1], (float)j + 1 - size / 2f);
                    array[4 + 6 * (i + width * j)] = new Vector3((float)i - size / 2f, heights[i, j + 1], (float)j + 1 - size / 2f);

                }

            }
            return array;
        }
        private void GeneratePlasmaMap(float[,] heights, Point topLeft, Point bottomRight)
        {
            float length = (bottomRight.X - topLeft.X + topLeft.Y - bottomRight.Y) / 2;
            length *= diff;
            if (bottomRight.X - topLeft.X <= 1 || topLeft.Y - bottomRight.Y <= 1)
            {
                return;
            }
            float h1 = heights[topLeft.X, topLeft.Y];  //height of the top left of the square
            float h2 = heights[bottomRight.X, topLeft.Y]; //height of the top right of the square
            float h3 = heights[bottomRight.X, bottomRight.Y]; // height of the bottom right of the square
            float h4 = heights[topLeft.X, bottomRight.Y]; //height of the bottom left of the square
            int midX = (bottomRight.X + topLeft.X) / 2;
            int midY = (bottomRight.Y + topLeft.Y) / 2;
            heights[midX, midY] = (h1 + h2 + h3 + h4) / 4 + rnd.NextFloat(-length, length);
            heights[midX, topLeft.Y] = (h1 + h2) / 2 + rnd.NextFloat(-length, length);
            heights[midX, bottomRight.Y] = (h3 + h4) / 2 + rnd.NextFloat(-length, length);
            heights[topLeft.X, midY] = (h4 + h1) / 2 + rnd.NextFloat(-length, length);
            heights[bottomRight.X, midY] = (h2 + h3) / 2 + rnd.NextFloat(-length, length);
            GeneratePlasmaMap(heights, topLeft, new Point(midX, midY));
            GeneratePlasmaMap(heights, new Point(midX, topLeft.Y), new Point(bottomRight.X, midY));
            GeneratePlasmaMap(heights, new Point(topLeft.X, midY), new Point(midX, bottomRight.Y));
            GeneratePlasmaMap(heights, new Point(midX, midY), bottomRight);
        }

        private float prevTime;
        MouseState previousMouseState;
        private float movementSpeed = 3f;
        private float mouseSens = 5f;
        public override void Update(GameTime gameTime)
        {
        
        }


        public override void Draw(GraphicsDeviceManager graphics)
        {

        }
    }
}
