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
        int[] indiceList;
        int iterations;
        int width;
        int maxHeight;
        int verticeListLength;
        int indiceListLength;
        float roughness;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        public Terrain(int index, Game1 game, TerrainGrid grid, Vector2 position, int width, int terrainHeight, int iterations, float mountainy, float roughness)
        {
            this.index = index;
            this.game = game;
            this.grid = grid;
            this.position = position;
            this.width = width;
            this.iterations = iterations;
            this.roughness = roughness;
            this.maxHeight = terrainHeight;
            SetUpVerticesBetter();

            for (int i = 0; i < verticeList.Length; i++)
            {
                verticeList[i].Position.Y *= (float)Math.Pow(verticeList[i].Position.Y, mountainy);
                verticeList[i].Position.Y /= (float)Math.Pow(maxHeight/2, mountainy);
            }
            //GimmeDatImage(verticeList);

            SetUpIndices();

            CopyToBuffers();
            GenerateNormals(vertexBuffer, indexBuffer);
        }

        private void SetUpVerticesBetter()
        {
            int squaresRow = (int)Math.Pow(2, iterations);
            int squaresTotal = (int)Math.Pow(squaresRow, 2);
            int verticeRow = (int)(Math.Pow(2, iterations) + 1);
            int verticeTotal = (int)Math.Pow(verticeRow, 2);
            

            verticeList = new VertexPositionNormalColored[verticeTotal];

            bool leftLoaded = false;
            bool rightLoaded = false;
            bool topLoaded = false;
            bool bottomLoaded = false; 

            if (grid.CheckTerrain(index, -1, 0) != short.MinValue)
            {
                Console.WriteLine("Left");
                leftEdge = grid.GetTerrainEdge(grid.CheckTerrain(index, -1, 0), 1, 0);
                leftLoaded = true;
            }
            else
            {
                leftEdge = new float[verticeRow];
            }
            if (grid.CheckTerrain(index, 1, 0) != short.MinValue)
            {
                Console.WriteLine("right");
                rightEdge = grid.GetTerrainEdge(grid.CheckTerrain(index, 1, 0), -1, 0);
                rightLoaded = true;
            }
            else
            {
                rightEdge = new float[verticeRow];
            }
            if (grid.CheckTerrain(index, 0, -1) != short.MinValue)
            {
                Console.WriteLine("top");
                topEdge = grid.GetTerrainEdge(grid.CheckTerrain(index, 0, -1), 0, 1);
                topLoaded = true;
            }
            else
            {
                topEdge = new float[verticeRow];
            }
            if (grid.CheckTerrain(index, 0, 1) != short.MinValue)
            {
                Console.WriteLine("bottom");
                bottomEdge = grid.GetTerrainEdge(grid.CheckTerrain(index, 0, 1), 0, -1);
                bottomLoaded = true;
            }
            else
            {
                bottomEdge = new float[verticeRow];
            }


            for (int z = 0; z < verticeRow; z++)
            {
                for (int x = 0; x < verticeRow; x++)
                {
                    int index = z * verticeRow + x;
                    verticeList[index].Position = new Vector3(x * ((float)width / (verticeRow - 1)), 0, -z * ((float)width / (verticeRow - 1)));
                    if (x == 0 && leftLoaded)
                    {
                        verticeList[index].Position.Y = leftEdge[z];
                    }
                    if (x == verticeRow - 1 && rightLoaded)
                    {

                        verticeList[index].Position.Y = rightEdge[z];
                    }
                    if (z == 0 && bottomLoaded)
                    {
                        verticeList[index].Position.Y = bottomEdge[x];
                    }
                    if (z == verticeRow - 1 && topLoaded)
                    {

                        verticeList[index].Position.Y = topEdge[x];
                    }
                }
            }
            /*if (!topLoaded && !leftLoaded)
            {
                verticeList[0].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;
                topEdge[0] = verticeList[0].Position.Y;
                leftEdge[verticeRow - 1] = verticeList[0].Position.Y;
            }
            else if(topLoaded)
            {
                verticeList[0].Position.Y = topEdge[0];
            }
            else
            {
                verticeList[0].Position.Y =leftEdge[verticeRow - 1];
            }

            if (!topLoaded &&  !rightLoaded) {
                verticeList[verticeRow - 1].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;
                topEdge[verticeRow - 1] = verticeList[verticeRow - 1].Position.Y;
                rightEdge[0] = verticeList[verticeRow - 1].Position.Y;
            }
            else if (topLoaded)
            {
                verticeList[verticeRow - 1].Position.Y = topEdge[verticeRow - 1];
            }
            else
            {
                verticeList[verticeRow - 1].Position.Y = rightEdge[0];
            }

            if (!bottomLoaded && !leftLoaded) {
                verticeList[verticeRow * (verticeRow - 1)].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;
                bottomEdge[0] = verticeList[verticeRow * (verticeRow - 1)].Position.Y;
                leftEdge[0] = verticeList[verticeRow * (verticeRow - 1)].Position.Y;
            }
            else if (bottomLoaded)
            {
                verticeList[verticeRow * (verticeRow - 1)].Position.Y = bottomEdge[0];
            }
            else
            {
                verticeList[verticeRow * (verticeRow - 1)].Position.Y = leftEdge[0];
            }

            if (!bottomLoaded && !rightLoaded)
            {
                verticeList[verticeRow * verticeRow - 1].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;
                bottomEdge[verticeRow - 1] = verticeList[verticeRow * verticeRow - 1].Position.Y;
                rightEdge[verticeRow - 1] = verticeList[verticeRow * verticeRow - 1].Position.Y;
            }
            else if (bottomLoaded)
            {
                verticeList[verticeRow * verticeRow - 1].Position.Y = bottomEdge[verticeRow - 1];
            }
            else
            {
                verticeList[verticeRow * verticeRow - 1].Position.Y = rightEdge[verticeRow - 1];
            }*/

            for (int iter = 1; iter <= iterations; iter++)
            {
                int iterSquaresRow = (int)Math.Pow(2, iter);
                int verticesPerSquare = ((verticeRow - 1) / iterSquaresRow);
                for (int x = 0; x < iterSquaresRow; x++)
                {
                    for (int z = 0; z < iterSquaresRow; z++)
                    {
                        int leftTopPoint = z * verticeRow * verticesPerSquare + x * verticesPerSquare;
                        int rightTopPoint = z * verticesPerSquare * verticeRow + x * verticesPerSquare + verticesPerSquare;
                        int middlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * verticeRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int leftBottomPoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare;
                        int rightBottomPoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare;


                        verticeList[middlePoint].Position.Y = AverageFloat(verticeList[leftTopPoint].Position.Y,
                                                                            verticeList[rightTopPoint].Position.Y,
                                                                            verticeList[leftBottomPoint].Position.Y,
                                                                            verticeList[rightBottomPoint].Position.Y,
                                                                                      iter);
                    }
                }
                for (int x = 0; x < iterSquaresRow; x++)
                {
                    for (int z = 0; z < iterSquaresRow; z++)
                    {
                        int leftTopPoint = z * verticeRow * verticesPerSquare + x * verticesPerSquare;
                        int topMiddlePoint = z * verticesPerSquare * verticeRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightTopPoint = z * verticesPerSquare * verticeRow + x * verticesPerSquare + verticesPerSquare;
                        int leftMiddlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * verticeRow + x * verticesPerSquare;
                        int middlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * verticeRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightMiddlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * verticeRow + x * verticesPerSquare + verticesPerSquare;
                        int leftBottomPoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare;
                        int bottomMiddlePoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightBottomPoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare;

                        if (x == 0)
                        {
                            if (!leftLoaded)
                            {
                                verticeList[leftMiddlePoint].Position.Y = AverageFloat(verticeList[leftTopPoint].Position.Y,
                                                                                         verticeList[leftBottomPoint].Position.Y,
                                                                                         verticeList[middlePoint].Position.Y,
                                                                                          iter);
                                leftEdge[z * verticesPerSquare + verticesPerSquare / 2] = verticeList[leftMiddlePoint].Position.Y;
                            }
                            else
                            {
                                verticeList[leftMiddlePoint].Position.Y = leftEdge[z * verticesPerSquare + verticesPerSquare / 2];
                            }
                        }
                        if (z == 0)
                        {
                            if (!topLoaded)
                            {
                                verticeList[topMiddlePoint].Position.Y = AverageFloat(verticeList[leftTopPoint].Position.Y,
                                                                                         verticeList[rightTopPoint].Position.Y,
                                                                                         verticeList[middlePoint].Position.Y,
                                                                                          iter);
                                topEdge[x * verticesPerSquare + verticesPerSquare / 2] = verticeList[topMiddlePoint].Position.Y;
                            }
                            else
                            {
                                verticeList[topMiddlePoint].Position.Y = topEdge[x * verticesPerSquare + verticesPerSquare / 2];
                            }
                        }


                        if (x == iterSquaresRow - 1)
                        {
                            if (!rightLoaded)
                            {
                                verticeList[rightMiddlePoint].Position.Y = AverageFloat(verticeList[rightTopPoint].Position.Y,
                                                                                          verticeList[rightBottomPoint].Position.Y,
                                                                                          verticeList[middlePoint].Position.Y,
                                                                                          iter);
                                rightEdge[z * verticesPerSquare + verticesPerSquare / 2] = verticeList[rightMiddlePoint].Position.Y;
                            }
                            else
                            {
                                verticeList[rightMiddlePoint].Position.Y = rightEdge[z * verticesPerSquare + verticesPerSquare / 2];
                            }
                        }
                        else
                        {
                            verticeList[rightMiddlePoint].Position.Y = AverageFloat(verticeList[rightTopPoint].Position.Y,
                                                                                      verticeList[rightBottomPoint].Position.Y,
                                                                                      verticeList[middlePoint].Position.Y,
                                                                                      verticeList[rightMiddlePoint + verticesPerSquare / 2].Position.Y,
                                                                                      iter);

                        }
                        if (z == (iterSquaresRow - 1))
                        {
                            if (!bottomLoaded)
                            {
                                verticeList[bottomMiddlePoint].Position.Y = AverageFloat(verticeList[leftBottomPoint].Position.Y,
                                                                                           verticeList[rightBottomPoint].Position.Y,
                                                                                           verticeList[middlePoint].Position.Y,
                                                                                          iter);
                                bottomEdge[x * verticesPerSquare + verticesPerSquare / 2] = verticeList[bottomMiddlePoint].Position.Y;
                            }
                            else
                            {
                                verticeList[bottomMiddlePoint].Position.Y = bottomEdge[x * verticesPerSquare + verticesPerSquare / 2];

                            }
                        }
                        else
                        {
                            verticeList[bottomMiddlePoint].Position.Y = AverageFloat(verticeList[leftBottomPoint].Position.Y,
                                                                                      verticeList[rightBottomPoint].Position.Y,
                                                                                      verticeList[middlePoint].Position.Y,
                                                                                      verticeList[bottomMiddlePoint + (verticesPerSquare / 2) * verticeRow].Position.Y,
                                                                                      iter);
                        }
                    }
                }
            }
            float highestY = 0;
            float lowestY = 0;
            for (int i = 0; i < verticeList.Length; i++)
            {
                if (verticeList[i].Position.Y < lowestY)
                {
                    lowestY = verticeList[i].Position.Y;
                }

                if (verticeList[i].Position.Y > highestY)
                {
                    highestY = verticeList[i].Position.Y;
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
