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
        Vector2 position;
        Random rng = new Random();
        Game1 game;
        int[] indices;
        int iterations;
        int width;
        int maxHeight;
        VertexPositionNormalColored[] verticeList;
        int verticesListLength;
        int indicesListLength;
        float detail;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public Terrain(Game1 game, Vector2 position, int width, int terrainHeight, int iterations, float mountainy, float roughness)
        {
            this.game = game;
            this.position = position;
            this.width = width;
            this.iterations = iterations;
            this.detail = roughness;
            this.maxHeight = terrainHeight;
            SetUpVertices();


            for (int i = 0; i < verticeList.Length; i++)
            {
                verticeList[i].Position.Y *= (float)Math.Pow(verticeList[i].Position.Y, mountainy);
                verticeList[i].Position.Y /= (float)Math.Pow(maxHeight, mountainy);
            }
            //GimmeDatImage(verticeList);

            SetUpIndices();

            CopyToBuffers();
            GenerateNormals(vertexBuffer, indexBuffer);
        }

        private void SetUpVertices()
        {
            verticeList = new VertexPositionNormalColored[4];

            verticeList[0].Position = new Vector3(0f, (((float)rng.NextDouble() - 0.5f) * maxHeight), 0f);
            verticeList[1].Position = new Vector3(width/2 + position.X*width, (((float)rng.NextDouble() - 0.5f) * maxHeight), 0f);
            verticeList[2].Position = new Vector3(0f, (((float)rng.NextDouble() - 0.5f) * maxHeight), -width/2 - position.Y*width);
            verticeList[3].Position = new Vector3(width/2 + position.X*width, (((float)rng.NextDouble() - 0.5f) * maxHeight), -width/2 - position.Y*width);

            //make a new array to save the vertices in
            //the new array should be (number of iterations+1)^2
            //iter stands for iteration
            float lowestY = 0;
            float highestY = 0;
            for (int iter = 0; iter < iterations; iter++)
            {
                VertexPositionNormalColored[] tempList = new VertexPositionNormalColored[(int)(Math.Pow(Math.Pow(2, iter + 1) + 1, 2))];

                int oldSquaresRow = (int)Math.Pow(2, iter);
                int oldSquaresTotal = (int)Math.Pow(oldSquaresRow, 2);
                int oldVerticeRow = (int)(Math.Pow(2, iter) + 1);
                int oldVerticeTotal = (int)Math.Pow(oldVerticeRow, 2);
                int newVerticeRow = (int)(Math.Pow(2, iter + 1) + 1);
                int newVerticeTotal = (int)Math.Pow(newVerticeRow, 2);

                //iterate through every square in the current(old) grid
                for (int y = 0; y < oldSquaresRow; y++)
                {
                    for (int x = 0; x < oldSquaresRow; x++)
                    {
                        //define the index of the several points in the new grid of vertices
                        int topLeftPointIndex = (y * 2) * newVerticeRow + x * 2;
                        int topRightPointIndex = (y * 2) * newVerticeRow + x * 2 + 2;
                        int bottomRightPointIndex = (y * 2 + 2) * newVerticeRow + x * 2 + 2;
                        int bottomLeftPointIndex = (y * 2 + 2) * newVerticeRow + x * 2;

                        int middlePointIndex = (y * 2 + 1) * newVerticeRow + x * 2 + 1;


                        //transfer the square's vertices to the new array
                        tempList[bottomRightPointIndex].Position = verticeList[((y + 1) * oldVerticeRow) + (x) + 1].Position;

                        if (tempList[topLeftPointIndex].Position.Y < lowestY)
                        {
                            lowestY = tempList[topLeftPointIndex].Position.Y;
                        }
                        if (tempList[topLeftPointIndex].Position.Y > highestY)
                        {
                            highestY = tempList[topLeftPointIndex].Position.Y;
                        }
                        if (y == 0)
                        {
                            tempList[topLeftPointIndex].Position = verticeList[(y * oldVerticeRow) + x].Position;
                            tempList[topRightPointIndex].Position = verticeList[(y * oldVerticeRow) + x + 1].Position;
                        }
                        //only calculate the middle point of the left when this square is at the left of the grid
                        if (x == 0)
                        {
                            tempList[bottomLeftPointIndex].Position = verticeList[((y + 1) * oldVerticeRow) + (x)].Position;
                        }

                        //define the new middle point vertice as the average of the 4 corners of the old square
                        tempList[middlePointIndex].Position = AverageSquare(tempList[topLeftPointIndex].Position,
                                                                            tempList[topRightPointIndex].Position,
                                                                            tempList[bottomLeftPointIndex].Position,
                                                                            tempList[bottomRightPointIndex].Position,
                                                                                      iter);


                        //only calculate the middle point of the top when this square is at the top of the grid
                    }
                }
                for (int y = 0; y < oldSquaresRow; y++)
                {
                    for (int x = 0; x < oldSquaresRow; x++)
                    {
                        int middlePointIndex = (y * 2 + 1) * newVerticeRow + x * 2 + 1;
                        int topLeftPointIndex = (y * 2) * newVerticeRow + x * 2;
                        int topRightPointIndex = (y * 2) * newVerticeRow + x * 2 + 2;
                        int bottomRightPointIndex = (y * 2 + 2) * newVerticeRow + x * 2 + 2;
                        int bottomLeftPointIndex = (y * 2 + 2) * newVerticeRow + x * 2;

                        //always calculate the diamond middle point of the right and bottom side of the square
                        int rightMiddlePointIndex = (y * 2 + 1) * newVerticeRow + x * 2 + 2;
                        int bottomMiddlePointIndex = (y * 2 + 2) * newVerticeRow + x * 2 + 1;

                        if (y == 0)
                        {
                            int topMiddelPointIndex = (y * 2) * newVerticeRow + x * 2 + 1;
                            tempList[topMiddelPointIndex].Position = AverageDiamond(tempList[topLeftPointIndex].Position,
                                                                                     tempList[topRightPointIndex].Position,
                                                                                     tempList[middlePointIndex].Position,
                                                                                      iter);
                        }

                        if (x == 0)
                        {
                            int leftMiddelPointIndex = (y * 2 + 1) * newVerticeRow + x * 2;
                            tempList[leftMiddelPointIndex].Position = AverageDiamond(tempList[topLeftPointIndex].Position,
                                                                                     tempList[bottomLeftPointIndex].Position,
                                                                                     tempList[middlePointIndex].Position,
                                                                                      iter);
                        }

                        if (x == oldSquaresRow - 1)
                        {
                            tempList[rightMiddlePointIndex].Position = AverageDiamond(tempList[topRightPointIndex].Position,
                                                                                      tempList[bottomRightPointIndex].Position,
                                                                                      tempList[middlePointIndex].Position,
                                                                                      iter);
                        }
                        else
                        {
                            tempList[rightMiddlePointIndex].Position = AverageSquare(tempList[topRightPointIndex].Position,
                                                                                      tempList[bottomRightPointIndex].Position,
                                                                                      tempList[middlePointIndex].Position,
                                                                                      tempList[rightMiddlePointIndex + 1].Position,
                                                                                      iter);

                        }
                        if (y == oldSquaresRow - 1)
                        {
                            tempList[bottomMiddlePointIndex].Position = AverageDiamond(tempList[bottomLeftPointIndex].Position,
                                                                                       tempList[bottomRightPointIndex].Position,
                                                                                       tempList[middlePointIndex].Position,
                                                                                      iter);
                        }
                        else
                        {
                            tempList[bottomMiddlePointIndex].Position = AverageSquare(tempList[bottomLeftPointIndex].Position,
                                                                                      tempList[bottomRightPointIndex].Position,
                                                                                      tempList[middlePointIndex].Position,
                                                                                      tempList[bottomMiddlePointIndex + newVerticeRow].Position,
                                                                                      iter);
                        }
                    }
                }
                //now store the temporary list to the real list for the next iteration or actual use
                verticeList = tempList;
            }
            for (int i = 0; i < verticeList.Length; i++)
            {
                verticeList[i].Position.Y += maxHeight / 2;
            }

        }
        private void SetUpIndices()
        {
            int amountOfIndices = (int)Math.Pow(4, iterations) * 6;
            int squareRow = (int)Math.Pow(2, iterations );
            int verticeRow = (int)(Math.Pow(2, iterations ) + 1);

            indices = new int[amountOfIndices];

            int index = 0;
            for (int y = 0; y < squareRow; y++)
            {
                for (int x = 0; x < squareRow; x++)
                {
                    indices[index++] = ((y+1) * verticeRow + (x+1));
                    indices[index++] = (y * verticeRow + (x + 1));
                    indices[index++] = (y * verticeRow + x);

                    indices[index++] = ((y + 1) * verticeRow + x+1);
                    indices[index++] = (y * verticeRow + x);
                    indices[index++] = ((y + 1) * verticeRow + x);
                }
            }
        }
        private void GenerateNormals(VertexBuffer vb, IndexBuffer ib)
        {
            VertexPositionNormalColored[] vertices = new VertexPositionNormalColored[verticesListLength];
            vb.GetData(vertices);
            int[] indices = new int[indicesListLength];
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
                if (vertices[i].Position.Y < maxHeight / 2)
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
                    vertices[i].Color = Color.White;
                }
            }
            vb.SetData(vertices);
        }

        public void GimmeDatImage(VertexPositionNormalColored[] vertices)
        {
            Texture2D newTexture = new Texture2D(game.GraphicsDevice, (int)(Math.Pow(2, iterations) + 1), (int)(Math.Pow(2, iterations) + 1));
            Color[] colorData = new Color[vertices.Length];

            for(int index = 0; index < vertices.Length; index++)
            {
                float thingy = (vertices[index].Position.Y + (maxHeight / 2))/maxHeight;
                if(thingy < 0 || thingy > 1)
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
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(detail + 1, iteration);
            while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(detail + 1, iteration);
            }
            float newZ = (vec1.Z + vec2.Z) / 2;
            Vector3 newVector = new Vector3(newX, newY, newZ);
            return newVector;
        }
        public Vector3 AverageSquare(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4, int iteration)
        {
            float newX = (vec1.X + vec2.X + vec3.X + vec4.X) / 4;
            float newY = ((vec1.Y + vec2.Y + vec3.Y + vec4.Y) / 4);
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight / 2) / (float)Math.Pow(detail + 1, iteration);
            while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight / 2) / (float)Math.Pow(detail + 1, iteration);
            }
            float newZ = (vec1.Z + vec2.Z + vec3.Z + vec4.Z) / 4;
            Vector3 newVector = new Vector3(newX, newY, newZ);
            return newVector;
        }
        private void CopyToBuffers()
        {
            verticesListLength = verticeList.Length;
            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalColored), verticesListLength, BufferUsage.None);
            vertexBuffer.SetData(verticeList);
            verticeList = null;

            indicesListLength = indices.Length;
            indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(int), indices.Length, BufferUsage.None);
            indexBuffer.SetData(indices);
            indices = null;
        }

        public void Draw(DrawHelper drawHelper)
        {
            Matrix worldMatrix = Matrix.Identity;
            drawHelper.Draw(vertexBuffer, indexBuffer, verticesListLength, indicesListLength, worldMatrix);
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
