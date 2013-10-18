using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Brace.GameLogic
{
    using SharpDX.Toolkit.Graphics;
    using Brace.Physics;
    class Landscape : Actor
    {
        private Random random = new Random();
        private float xzScale = 200;

        // Verticies
        private float[,] segments;
        public VertexInputLayout inputLayout;
        public Buffer<VertexPositionNormalColor> vertices;
        public Landscape(BraceGame game)
            : base(Vector3.Zero, Vector3.Zero)
        {
            // Generate the terrain and verticies
            segments = GenerateTerrain(10, 10f);

            //build physics object
            pObject = new PhysicsModel();
            TerrainBody bodyDef = new TerrainBody(pObject,segments,xzScale);
            pObject.Initialize(20, 1,1, 1, Vector3.Zero, Vector3.Zero, bodyDef,null);

            //add to physics sim
            BraceGame.get().physicsWorld.AddBody(pObject);

            VertexPositionNormalColor[] vertPosColorNormals = GenerateVerticies(segments);
            //throw new Exception();

            // Create the renderable object
            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                vertPosColorNormals);

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

    

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect)
        {
            // Setup the vertices
            context.SetVertexBuffer(vertices);
            context.SetVertexInputLayout(inputLayout);

            Matrix world = Matrix.RotationX(rot.X) * Matrix.RotationY(rot.Y) * Matrix.RotationZ(rot.Z) * Matrix.Translation(position);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            effect.CurrentTechnique.Passes[0].Apply();

            context.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        public float HeightAt(float x, float z)
        {
            int i = (int)((0.5f + x / xzScale / 2) * segments.GetLength(0));
            int j = (int)((0.5f + z / xzScale / 2) * segments.GetLength(1));
            return segments[i, j];
        }

        private float[,] GenerateTerrain(int divisions, float heightRange)
        {
            int nSegments = (int)Math.Pow(2, divisions) + 1;
            float[,] segments = new float[nSegments, nSegments];
            segments[0, 0] = random.NextFloat(-heightRange, heightRange);
            segments[0, nSegments - 1] = random.NextFloat(-heightRange, heightRange);
            segments[nSegments - 1, 0] = random.NextFloat(-heightRange, heightRange);
            segments[nSegments - 1, nSegments - 1] = random.NextFloat(-heightRange, heightRange);

            int halfStep, stepSize;
            for (stepSize = nSegments - 1; stepSize > 1; stepSize /= 2, heightRange /= 2)
            {
                halfStep = stepSize / 2;

                // Diamond step
                for (int i = 0; i <= nSegments - stepSize; i += stepSize)
                {
                    for (int j = 0; j <= nSegments - stepSize; j += stepSize)
                    {
                        float sumHeights = 0;
                        sumHeights += segments[i, j];
                        sumHeights += segments[i + stepSize, j];
                        sumHeights += segments[i, j + stepSize];
                        sumHeights += segments[i + stepSize, j + stepSize];

                        float avgHeight = sumHeights / 4;

                        segments[i + halfStep, j + halfStep] = avgHeight + random.NextFloat(-1f, 1f) * heightRange;
                    }
                }

                // Square step
                for (int i = 0; i < nSegments; i += halfStep)
                {
                    for (int j = (i + halfStep) % stepSize; j < nSegments; j += stepSize)
                    {
                        float sumHeights = 0;
                        int nIncluded = 0;

                        // Point to the left
                        if (j - halfStep >= 0)
                        {
                            sumHeights += segments[i, j - halfStep];
                            nIncluded++;
                        }

                        // Point to the right
                        if (j + halfStep < nSegments)
                        {
                            sumHeights += segments[i, j + halfStep];
                            nIncluded++;
                        }

                        // Point below
                        if (i + halfStep < nSegments)
                        {
                            sumHeights += segments[i + halfStep, j];
                            nIncluded++;
                        }

                        // Point above
                        if (i - halfStep >= nSegments)
                        {
                            sumHeights += segments[i - halfStep, j];
                            nIncluded++;
                        }

                        float avgHeight = sumHeights / nIncluded;

                        segments[i, j] = avgHeight + random.NextFloat(-1f, 1f) * heightRange;
                    }
                }
            }

            return segments;
        }

        private VertexPositionNormalColor[] GenerateVerticies(float[,] segments)
        {
            int nRows = segments.GetLength(0);
            int nCols = segments.GetLength(1);
            int nVertPosCol = (nRows - 1) * (nCols - 1) * 2 * 3;

            VertexPositionNormalColor[] values = new VertexPositionNormalColor[nVertPosCol];
            Color[,] colors = GenerateColors(segments);
            Vector3[,] normals = GenerateNormals(segments);

            for (int i = 0; i < nRows - 1; i++)
            {
                float xStart = xzScale * ((i * 2.0f / (nRows - 1)) - 1);
                float xDist = xzScale * 2f / (nRows - 1);

                for (int j = 0; j < nCols - 1; j++)
                {
                    int baseIndex = (i * (nCols - 1) + j) * 6;
                    float zStart = xzScale * ((j * 2.0f / (nCols - 1)) - 1);
                    float zDist = xzScale * 2f / (nCols - 1);

                    // First triangle
                    values[baseIndex] = new VertexPositionNormalColor(new Vector3(xStart, segments[i, j], zStart), normals[i, j], colors[i, j]);
                    values[baseIndex + 1] = new VertexPositionNormalColor(new Vector3(xStart + xDist, segments[i + 1, j + 1], zStart + zDist), normals[i + 1, j + 1], colors[i + 1, j + 1]);
                    values[baseIndex + 2] = new VertexPositionNormalColor(new Vector3(xStart + xDist, segments[i + 1, j], zStart), normals[i + 1, j], colors[i + 1, j]);

                    // Second triangle
                    values[baseIndex + 3] = new VertexPositionNormalColor(new Vector3(xStart, segments[i, j], zStart), normals[i, j], colors[i, j]);
                    values[baseIndex + 4] = new VertexPositionNormalColor(new Vector3(xStart, segments[i, j + 1], zStart + zDist), normals[i, j + 1], colors[i, j + 1]);
                    values[baseIndex + 5] = new VertexPositionNormalColor(new Vector3(xStart + xDist, segments[i + 1, j + 1], zStart + zDist), normals[i + 1, j + 1], colors[i + 1, j + 1]);
                }
            }
            
            return values;
        }

        private Color[,] GenerateColors(float[,] segments)
        {
            int nRows = segments.GetLength(0);
            int nCols = segments.GetLength(1);

            float min = Min(segments);
            float max = Max(segments);

            Color[,] colors = new Color[nRows, nCols];

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    float scale = (segments[i, j] - min) / (max - min); // scale in range [0, 1]
                    colors[i, j] = ColorForScale(scale);
                }
            }

            return colors;
        }

        private Color ColorForScale(float scale)
        {
            // num
            float[] scalePoints = new float[7] { 0.37f, 0.4f, 0.45f, 0.7f, 0.75f, 0.8f, 1.0f };
            Color[] colours = new Color[8] {
                Color.Khaki,
                Color.Beige,
                Color.Khaki,
                Color.DarkKhaki,
                Color.LawnGreen,
                Color.ForestGreen,
                Color.Green,
                Color.LimeGreen
            };

            // Find where the scale is in the scalePoints array
            int index = 0;
            while (scale > scalePoints[index])
                index++;

            // Figure out the size of the interval in which the scale lies
            float stepRange;
            if (index == 0)
                stepRange = scalePoints[0];
            else
                stepRange = scalePoints[index] - scalePoints[index - 1];

            return Color.SmoothStep(colours[index + 1], colours[index], (scalePoints[index] - scale) / stepRange);
        }

        private Vector3[,] GenerateNormals(float[,] segments)
        {
            int nRows = segments.GetLength(0);
            int nCols = segments.GetLength(1);

            float xDist = xzScale * 2f / (nRows - 1);
            float zDist = xzScale * 2f / (nCols - 1);

            Vector3[,] xEdgeNormals = new Vector3[nRows - 1, nCols];
            Vector3[,] zEdgeNormals = new Vector3[nRows, nCols - 1];
            Vector3[,] vertexNormals = new Vector3[nRows, nCols];

            // Generate edge normals
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if (i < nRows - 1)
                    {
                        xEdgeNormals[i, j] = Vector3.Cross(Vector3.UnitZ, new Vector3(xDist, segments[i + 1, j] - segments[i, j], 0));
                        xEdgeNormals[i, j].Normalize();
                    }

                    if (j < nCols - 1)
                    {
                        zEdgeNormals[i, j] = Vector3.Cross(new Vector3(0, segments[i, j + 1] - segments[i, j], zDist), Vector3.UnitX);
                        zEdgeNormals[i, j].Normalize();
                    }
                }
            }

            // Generate vertex normals
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    Vector3 xNorm, zNorm;

                    if (i == 0)
                        xNorm = xEdgeNormals[i, j];
                    else if (i == nRows - 1)
                        xNorm = xEdgeNormals[i - 1, j];
                    else
                    {
                        xNorm = xEdgeNormals[i, j] + xEdgeNormals[i - 1, j];
                        xNorm.Normalize();
                    }

                    if (j == 0)
                        zNorm = zEdgeNormals[i, j];
                    else if (j == nCols - 1)
                        zNorm = zEdgeNormals[i, j - 1];
                    else
                    {
                        zNorm = zEdgeNormals[i, j] + zEdgeNormals[i, j - 1];
                        zNorm.Normalize();
                    }

                    vertexNormals[i, j] = zNorm + xNorm;
                    vertexNormals[i, j].Y /= 2;
                }
            }

            return vertexNormals;
        }

        private float Min(float[,] array)
        {
            float min = array[0, 0];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] < min)
                        min = array[i, j];
                }
            }
            return min;
        }

        private float Max(float[,] array)
        {
            float max = array[0, 0];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] > max)
                        max = array[i, j];
                }
            }
            return max;
        }
    }
}
