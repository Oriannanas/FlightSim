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
        VertexPositionNormalColored[] verticeList;
        VertexPositionColor[] waterVerticeList;
        int[] indiceList;
        int[] waterIndiceList;
        int iterations;
        int width;
        int maxHeight;
        int verticeListLength;
        int waterVerticeListLength;
        int indiceListLength;
        int waterIndiceListLength;
        float detail;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        VertexBuffer waterVertexBuffer;
        IndexBuffer waterIndexBuffer;

        public Terrain(Game1 game, Vector2 position, int width, int terrainHeight, int iterations, float mountainy, float roughness)
        {
            this.game = game;
            this.position = position;
            this.width = width;
            this.iterations = iterations;
            this.detail = roughness;
            this.maxHeight = terrainHeight;
            SetUpVerticesBetter();
            GenerateWater();


           /* for (int i = 0; i < verticeList.Length; i++)
            {
                verticeList[i].Position.Y *= (float)Math.Pow(verticeList[i].Position.Y, mountainy);
                verticeList[i].Position.Y /= (float)Math.Pow((maxHeight/2), mountainy);
            }*/
            GimmeDatImage(verticeList);

            SetUpIndices();

            CopyToBuffers();
            GenerateNormals(vertexBuffer, indexBuffer);
            GenerateWater();
        }

        private void GenerateWater()
        {
            waterVerticeList = new VertexPositionColor[4];

            waterVerticeList[0].Position = new Vector3(0f, -10f, 0f);
            waterVerticeList[0].Color = Color.Blue;
            waterVerticeList[1].Position = new Vector3(width, -10f, 0f);
            waterVerticeList[1].Color = Color.Blue;
            waterVerticeList[2].Position = new Vector3(0f, -10f, -width);
            waterVerticeList[2].Color = Color.Blue;
            waterVerticeList[3].Position = new Vector3(width, -10f, -width);
            waterVerticeList[3].Color = Color.Blue;

            waterIndiceList = new int[6];

            waterIndiceList[0] = 0;
            waterIndiceList[1] = 1;
            waterIndiceList[2] = 2;
            
            waterIndiceList[3] = 1;
            waterIndiceList[4] = 2;
            waterIndiceList[5] = 3;

            waterVerticeListLength = waterVerticeList.Length;
            waterVertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalColored), waterVerticeListLength, BufferUsage.None);
            waterVertexBuffer.SetData(waterVerticeList);
            waterVerticeList = null;

            waterIndiceListLength = waterIndiceList.Length;
            waterIndexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(int), waterIndiceList.Length, BufferUsage.None);
            waterIndexBuffer.SetData(waterIndiceList);
            waterIndiceList = null;
            
        }
        /*private void SetUpVertices()
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

        }*/

        private void SetUpVerticesBetter()
        {
            int squaresRow = (int)Math.Pow(2, iterations);
            int squaresTotal = (int)Math.Pow(squaresRow, 2);
            int verticeRow = (int)(Math.Pow(2, iterations + 1) + 1);
            int verticeTotal = (int)Math.Pow(verticeRow, 2);
            verticeList = new VertexPositionNormalColored[verticeTotal];
            Console.WriteLine((float)width / verticeRow);
            

            for (int z = 0; z < verticeRow; z++)
            {
                for (int x = 0; x < verticeRow; x++)
                {
                    int index = z * verticeRow + x;
                    verticeList[index].Position = new Vector3(x*((float)width / (verticeRow-1)), 0, -z*((float)width / (verticeRow - 1)));
                    verticeList[index].Color = Color.Gray;
                }
            }

            verticeList[0].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;
            verticeList[verticeRow - 1].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;
            verticeList[verticeRow * (verticeRow - 1)].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;
            verticeList[verticeRow * verticeRow - 1].Position.Y = ((float)rng.NextDouble() - 0.5f) * maxHeight;

            for (int iter = 0; iter < iterations; iter++)
            {
                int iterSquaresRow = (int)Math.Pow(2, iter);
                int verticesPerSquare = ((verticeRow-1) / iterSquaresRow);
                for (int x = 0; x < iterSquaresRow; x++)
                {
                    for (int z = 0; z < iterSquaresRow; z++)
                    {
                        int leftTopPoint = z * verticeRow * verticesPerSquare + x * verticesPerSquare;
                        int topMiddlePoint = z * verticesPerSquare * verticeRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightTopPoint = z * verticesPerSquare * verticeRow + x * verticesPerSquare + verticesPerSquare;
                        int leftMiddlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * verticeRow + x * verticesPerSquare;
                        int middlePoint = (z* verticesPerSquare + verticesPerSquare / 2)*verticeRow +  x*verticesPerSquare + verticesPerSquare/2;
                        int rightMiddlePoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare;
                        int leftBottomPoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare;
                        int bottomMiddlePoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare / 2;
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
                        int rightMiddlePoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare;
                        int leftBottomPoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare;
                        int bottomMiddlePoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightBottomPoint = (z * verticesPerSquare + verticesPerSquare) * verticeRow + x * verticesPerSquare + verticesPerSquare;

                        if (z == 0)
                        {
                            verticeList[topMiddlePoint].Position.Y = AverageFloat(verticeList[leftTopPoint].Position.Y,
                                                                                     verticeList[rightTopPoint].Position.Y,
                                                                                     verticeList[middlePoint].Position.Y,
                                                                                      iter);
                        }

                        if (z == 0)
                        {
                            verticeList[leftMiddlePoint].Position.Y = AverageFloat(verticeList[leftTopPoint].Position.Y,
                                                                                     verticeList[leftBottomPoint].Position.Y,
                                                                                     verticeList[middlePoint].Position.Y,
                                                                                      iter);
                        }

                        if (x == iterSquaresRow - 1)
                        {
                            verticeList[rightMiddlePoint].Position.Y = AverageFloat(verticeList[rightTopPoint].Position.Y,
                                                                                      verticeList[rightBottomPoint].Position.Y,
                                                                                      verticeList[middlePoint].Position.Y,
                                                                                      iter);
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
                            verticeList[bottomMiddlePoint].Position.Y = AverageFloat(verticeList[leftBottomPoint].Position.Y,
                                                                                       verticeList[rightBottomPoint].Position.Y,
                                                                                       verticeList[middlePoint].Position.Y,
                                                                                      iter);
                        }
                        else
                        {
                            verticeList[bottomMiddlePoint].Position.Y = AverageFloat(verticeList[leftBottomPoint].Position.Y,
                                                                                      verticeList[rightBottomPoint].Position.Y,
                                                                                      verticeList[middlePoint].Position.Y,
                                                                                      verticeList[bottomMiddlePoint + (verticesPerSquare / 2)*verticeRow].Position.Y,
                                                                                      iter);
                        }
                    }
                }
            }

            for (int i = 0; i < verticeList.Length; i++)
            {
                if(verticeList[i].Position.Y == 0)
                {
                    //Console.WriteLine(i);
                }
            }
            Console.WriteLine(verticeList.Length);

        }

        private void SetUpIndices()
        {
            int amountOfIndices = (int)Math.Pow(4, iterations) * 6;
            int squareRow = (int)Math.Pow(2, iterations );
            int verticeRow = (int)(Math.Pow(2, iterations +1) + 1);
            Console.WriteLine(squareRow + "   "+ verticeRow);

            indiceList = new int[amountOfIndices];

            int index = 0;
            for (int y = 0; y < squareRow; y++)
            {
                for (int x = 0; x < squareRow; x++)
                {
                    indiceList[index++] = ((y+1) * verticeRow + (x+1));
                    indiceList[index++] = (y * verticeRow + (x + 1));
                    indiceList[index++] = (y * verticeRow + x);

                    indiceList[index++] = ((y + 1) * verticeRow + x+1);
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
        public float AverageFloat(float y1, float y2, float y3, int iteration)
        {
            float newY = (y1 + y2 + y3) / 3;
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight);
            while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(detail + 1, iteration);
            }
            return newY;
        }

        public float AverageFloat(float y1, float y2, float y3, float y4, int iteration)
        {
            float newY = (y1 + y2 + y3+y4) / 4;
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight);
            while (newY < -maxHeight / 2 || newY > maxHeight / 2)
            {
                newY += (((float)rng.NextDouble() - 0.5f) * maxHeight) / (float)Math.Pow(detail + 1, iteration);
            }
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
            Matrix worldMatrix = Matrix.Identity;
            drawHelper.Draw(vertexBuffer, indexBuffer, verticeListLength, indiceListLength, worldMatrix);
            drawHelper.Draw(waterVertexBuffer, waterIndexBuffer, waterVerticeListLength, waterIndiceListLength, worldMatrix);
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
