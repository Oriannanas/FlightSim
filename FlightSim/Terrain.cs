using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class Terrain
    {
        public int index;
        Game1 game;
        TerrainGrid grid;

        Vector2 position;
        Random rng = new Random();
        VertexPositionNormalColored[] verticeList;
        public float[] leftEdge
        {
            get; private set;
        }
        public float[] rightEdge
        {
            get; private set;
        }
        public float[] bottomEdge
        {
            get; private set;
        }
        public float[] topEdge
        {
            get; private set;
        }
        float[] cornerValues;
        int[] indiceList;
        int iterations;
        int width;
        int maxHeight;
        int verticeListLength;
        public DiamondSquare ds;
        int indiceListLength;
        float roughness;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        public Terrain(int index, Game1 game, TerrainGrid grid, Vector2 position, int width, int terrainHeight, int iterations, float mountainy, float roughness, float[] cornerValues)
        {
            this.cornerValues = cornerValues;
            this.index = index;
            this.game = game;
            this.grid = grid;
            this.position = position;
            this.width = width;
            this.iterations = iterations;
            this.roughness = roughness;
            this.maxHeight = terrainHeight;
            SetUpVertices();
            
            SetUpIndices();

            CopyToBuffers();
            GenerateNormals(vertexBuffer, indexBuffer);
        }

        private void SetUpVertices()
        {
            int squaresRow = (int)Math.Pow(2, iterations);
            int squaresTotal = (int)Math.Pow(squaresRow, 2);
            int verticeRow = (int)(Math.Pow(2, iterations) + 1);
            int verticeTotal = (int)Math.Pow(verticeRow, 2);

            verticeList = new VertexPositionNormalColored[verticeTotal];

            List<DiamondSquare> neighboors = new List<DiamondSquare>();
            List<Side> neighboorSides = new List<Side>();

            if (grid.GetTerrain(index, -1, 0) != null)
            {
                neighboors.Add(grid.GetTerrain(index, -1, 0).ds);
                neighboorSides.Add(Side.Left);
            }
            if (grid.GetTerrain(index, 1, 0) != null)
            {
                neighboors.Add(grid.GetTerrain(index, 1, 0).ds);
                neighboorSides.Add(Side.Right);
            }
            if (grid.GetTerrain(index, 0, 1) != null)
            {
                neighboors.Add(grid.GetTerrain(index, 0, 1).ds);
                neighboorSides.Add(Side.Bottom);
            }
            if (grid.GetTerrain(index, 0, -1) != null)
            {
                neighboors.Add(grid.GetTerrain(index, 0, -1).ds);
                neighboorSides.Add(Side.Top);
            }


            ds = new DiamondSquare(iterations, roughness, false, cornerValues, neighboors.ToArray(), neighboorSides.ToArray());

            for (int z = 0; z < verticeRow; z++)
            {
                for (int x = 0; x < verticeRow; x++)
                {
                    int index = z * verticeRow + x;
                    verticeList[index].Position = new Vector3(x * ((float)width / (verticeRow - 1)), ds.valueList[index]*maxHeight, -z * ((float)width / (verticeRow - 1)));
                }
            }
        }

        private void SetUpIndices()
        {
            int amountOfIndices = (int)Math.Pow(4, iterations) * 6;
            int squareRow = (int)Math.Pow(2, iterations);
            int verticeRow = (int)(Math.Pow(2, iterations) + 1);

            indiceList = new int[amountOfIndices];
            
            int index = 0;
            for (int y = 0; y < squareRow; y++)
            {
                for (int x = 0; x < squareRow; x++)
                {
                    indiceList[index++] = ((y + 1) * verticeRow + (x + 1));
                    indiceList[index++] = (y * verticeRow + (x + 1));
                    indiceList[index++] = (y * verticeRow + x);

                    indiceList[index++] = ((y + 1) * verticeRow + x + 1);
                    indiceList[index++] = (y * verticeRow + x);
                    indiceList[index++] = ((y + 1) * verticeRow + x);
                }
            }
        }
        private void GenerateNormals(VertexBuffer vb, IndexBuffer ib)
        {
            VertexPositionNormalColored[] vertices = new VertexPositionNormalColored[verticeListLength];
            vb.GetData(vertices);
            int[] indices = new int[indiceListLength];
            ib.GetData(indices);

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                Vector3 firstvec = vertices[indices[i * 3 + 1]].Position - vertices[indices[i * 3]].Position;
                Vector3 secondvec = vertices[indices[i * 3]].Position - vertices[indices[i * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                vertices[indices[i * 3]].Normal += normal;
                vertices[indices[i * 3 + 1]].Normal += normal;
                vertices[indices[i * 3 + 2]].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].Position.Y < maxHeight *0.75f)
                {
                    if (vertices[i].Normal.Y > 0.8f)
                    {
                        vertices[i].Color = Color.Green;
                    }
                    else
                    {
                        vertices[i].Color = Color.Gray;
                    }
                }
                else
                {
                    if (vertices[i].Normal.Y > 0.8f)
                    {
                        vertices[i].Color = Color.White;
                    }
                    else
                    {
                        vertices[i].Color = Color.DarkGray;
                    }
                }
            }
            vb.SetData(vertices);
        }

        public void GimmeDatImage(VertexPositionNormalColored[] vertices)
        {
            Texture2D newTexture = new Texture2D(game.GraphicsDevice, (int)(Math.Sqrt(vertices.Length)), (int)(Math.Sqrt(vertices.Length)));
            Color[] colorData = new Color[vertices.Length];

            for (int index = 0; index < vertices.Length; index++)
            {
                float thingy = (vertices[index].Position.Y + (maxHeight / 2)) / maxHeight;
                if (thingy < 0 || thingy > 1)
                {
                    //Console.WriteLine(thingy);
                }
                colorData[index] = new Color(thingy, thingy, thingy);
            }
            newTexture.SetData(colorData);
            Stream stream = File.Create(DateTime.Now.ToString("MM-dd-yy H;mm;ss") + ".png");

            //Save as PNG
            newTexture.SaveAsPng(stream, newTexture.Width, newTexture.Height);
            stream.Dispose();
            newTexture.Dispose();
        }

        public Vector3 AverageDiamond(Vector3 vec1, Vector3 vec2, Vector3 middlePoint, int iteration)
        {
            float newX = (vec1.X + vec2.X) / 2;
            float newY = ((vec1.Y + vec2.Y + middlePoint.Y) / 3);
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(roughness + 1, iteration);
            while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(roughness + 1, iteration);
            }
            float newZ = (vec1.Z + vec2.Z) / 2;
            Vector3 newVector = new Vector3(newX, newY, newZ);
            return newVector;
        }
        public Vector3 AverageSquare(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4, int iteration)
        {
            float newX = (vec1.X + vec2.X + vec3.X + vec4.X) / 4;
            float newY = ((vec1.Y + vec2.Y + vec3.Y + vec4.Y) / 4);
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight / 2) / (float)Math.Pow(roughness + 1, iteration);
            while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight / 2) / (float)Math.Pow(roughness + 1, iteration);
            }
            float newZ = (vec1.Z + vec2.Z + vec3.Z + vec4.Z) / 4;
            Vector3 newVector = new Vector3(newX, newY, newZ);
            return newVector;
        }
        public float AverageFloat(float y1, float y2, float y3, int iteration)
        {
            float newY = (y1 + y2 + y3) / 3;
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(roughness + 1, iteration);
            /*while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(roughness + 1, iteration);
            }*/
            return newY;
        }

        public float AverageFloat(float y1, float y2, float y3, float y4, int iteration)
        {
            float newY = (y1 + y2 + y3 + y4) / 4;
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(roughness + 1, iteration);
            /*while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(roughness + 1, iteration);
            }*/
            return newY;
        }

        private void CopyToBuffers()
        {
            verticeListLength = verticeList.Length;
            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalColored), verticeListLength, BufferUsage.None);
            vertexBuffer.SetData(verticeList);
            verticeList = null;

            indiceListLength = indiceList.Length;
            indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(int), indiceList.Length, BufferUsage.None);
            indexBuffer.SetData(indiceList);
            indiceList = null;
        }

        public void Draw(DrawHelper drawHelper)
        {
            Matrix worldMatrix = Matrix.Identity * Matrix.CreateTranslation(new Vector3((width) * position.X, 0, -(width) * position.Y));
            drawHelper.Draw(vertexBuffer, indexBuffer, verticeListLength, indiceListLength, worldMatrix);
            //drawHelper.Draw(verticeList, indices, worldMatrix);
        }
    }
    public struct VertexPositionNormalColored : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                  new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0));

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionNormalColored.VertexDeclaration; }
        }
    }
}
