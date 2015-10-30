using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class Terrain
    {
        Vector2 position;
        Random rng = new Random();
        Game1 game;
        short[] indices;
        int iterations;
        int width;
        int maxHeight;
        VertexPositionColor[] verticeList;
        float detail;
        //VertexBuffer vertexBuffer;

        //int width;
        //int height;

        public Terrain(Game1 game, Vector2 position, int width, int iterations, float detail)
        {
            this.game = game;
            this.position = position;
            this.width = width;
            this.iterations = iterations;
            this.detail = detail;
            this.maxHeight = width / 2;
            SetUpVertices();
            SetUpIndices();
        }

        private void SetUpVertices()
        {
            verticeList = new VertexPositionColor[4];
            Console.WriteLine(((float)rng.NextDouble() - 0.5f) * 16);

            verticeList[0].Position = new Vector3(0f, (((float)rng.NextDouble() - 0.5f) * maxHeight), 0f);
            verticeList[0].Color = Color.White;
            verticeList[1].Position = new Vector3(width/2 + position.X*width, (((float)rng.NextDouble() - 0.5f) * maxHeight), 0f);
            verticeList[1].Color = Color.White;
            verticeList[2].Position = new Vector3(0f, (((float)rng.NextDouble() - 0.5f) * maxHeight), -width/2 - position.Y*width);
            verticeList[2].Color = Color.White;
            verticeList[3].Position = new Vector3(width/2 + position.X*width, (((float)rng.NextDouble() - 0.5f) * maxHeight), -width/2 - position.Y*width);
            verticeList[3].Color = Color.White;

            //make a new array to save the vertices in
            //the new array should be (number of iterations+1)^2
            //iter stands for iteration
            for (int iter = 0; iter < iterations; iter++)
            {
                VertexPositionColor[] tempList = new VertexPositionColor[(int)(Math.Pow(Math.Pow(2, iter + 1) + 1, 2))];

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
                for(int i = 0; i < verticeList.Length; i++)
                {
                    verticeList[i].Color = Color.White;
                }
            }

        }
        private void SetUpIndices()
        {
            int amountOfIndices = (int)Math.Pow(4, iterations) * 6;
            int squareRow = (int)Math.Pow(2, iterations );
            int verticeRow = (int)(Math.Pow(2, iterations ) + 1);

            indices = new short[amountOfIndices];

            int index = 0;
            for (int y = 0; y < squareRow; y++)
            {
                for (int x = 0; x < squareRow; x++)
                {
                    indices[index++] = (short)(y * verticeRow + x);
                    indices[index++] = (short)(y * verticeRow + x + 1);
                    indices[index++] = (short)((y+1) * verticeRow + x);

                    indices[index++] = (short)(y * verticeRow + x+1);
                    indices[index++] = (short)((y + 1) * verticeRow + x);
                    indices[index++] = (short)((y + 1) * verticeRow + x+1);
                }
            }
        }


        public Vector3 AverageDiamond(Vector3 vec1, Vector3 vec2, Vector3 middlePoint, int iteration)
        {
            float newX = (vec1.X + vec2.X) / 2;
            float newY = ((vec1.Y + vec2.Y + middlePoint.Y) / 3) + ((((float)rng.NextDouble() - 0.5f) * width / 4) / (float)Math.Pow((iteration + 1),detail));
            float newZ = (vec1.Z + vec2.Z) / 2;
            Vector3 newVector = new Vector3(newX, newY, newZ);
            return newVector;
        }
        public Vector3 AverageSquare(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4, int iteration)
        {
            float newX = (vec1.X + vec2.X + vec3.X + vec4.X) / 4;
            float newY = ((vec1.Y + vec2.Y + vec3.Y + vec4.Y) / 4);
            newY += (((float)rng.NextDouble() - 0.5f) * maxHeight  *(newY / maxHeight)) / (float)Math.Pow((iteration + 1), detail);
            float newZ = (vec1.Z + vec2.Z + vec3.Z + vec4.Z) / 4;
            Vector3 newVector = new Vector3(newX, newY, newZ);
            return newVector;
        }

        public void Draw(DrawHelper drawHelper)
        {
            Matrix worldMatrix = Matrix.Identity;

            drawHelper.Draw(verticeList, indices, worldMatrix);
        }
    }
    public struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }
}
