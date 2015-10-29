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
        Random rng = new Random();
        Game1 game;
        int[] indices;
        VertexPositionColor[] verticeList;
        //VertexBuffer vertexBuffer;

        //int width;
        //int height;

        public Terrain(Game1 game)
        {
            this.game = game;
            SetUpVertices(3);
            SetUpIndices();
        }

        private void SetUpVertices(int iterations)
        {
            verticeList = new VertexPositionColor[4];


            verticeList[0].Position = new Vector3(0f, 0f, 0f);
            verticeList[0].Color = Color.White;
            verticeList[1].Position = new Vector3(5f, 0f, 0f);
            verticeList[1].Color = Color.White;
            verticeList[2].Position = new Vector3(0f, 0f, -5f);
            verticeList[2].Color = Color.White;
            verticeList[3].Position = new Vector3(5f, 0f, -5f);
            verticeList[3].Color = Color.White;

            //make a new array to save the vertices in
            //the new array should be (number of iterations+1)^2
            //iter stands for iteration
            for (int iter = 0; iter < iterations; iter++)
            {
                VertexPositionColor[] tempList = new VertexPositionColor[(int)(Math.Pow(Math.Pow(2, iter+1) + 1, 2))];

                int oldSquaresRow = (int)Math.Pow(2, iter);
                int oldSquaresTotal = (int)Math.Pow(oldSquaresRow, 2);
                int newSquaresRow = (int)Math.Pow(2, iter+1);
                int newSquaresTotal = (int)Math.Pow(newSquaresRow, 2);
                int oldVerticeRow = (int)(Math.Pow(2, iter) + 1);
                int oldVerticeTotal = (int)Math.Pow(oldVerticeRow, 2);
                int newVerticeRow = (int)(Math.Pow(2, iter+1) + 1);
                int newVerticeTotal = (int)Math.Pow(newVerticeRow, 2);

                //Console.WriteLine("Row: old vertice "+oldVerticeRow+" new vertice "+newVerticeRow + " old squares " + oldSquaresRow + " new squares " + newSquaresRow + "Total: old vertice " + oldVerticeTotal + " new vertice " + newVerticeTotal + " old squares " + oldSquaresTotal + " new squares " + newSquaresTotal);
                
                //first we need to calculate the middle point of every square (diamond step)
                //iterate through every square in the current(old) grid
                for (int y = 0; y < oldSquaresRow; y++)
                {
                    for(int x= 0; x < oldSquaresRow; x++)
                    {
                        //find the index of the middle point of this square in the new grid of vertices
                        int middlePointIndex = (y * 2 + 1) * newVerticeRow + (x + 1) * 2;
                        Console.WriteLine(middlePointIndex);
                        Console.WriteLine(tempList.Length);
                        //define the new vertice as the average of the 4 corners of the old square
                        tempList[middlePointIndex].Position = AverageSquare(      verticeList[(y * oldVerticeRow) + (x)].Position,
                                                                                  verticeList[(y * oldVerticeRow) + (x) + 1].Position,
                                                                                  verticeList[((y + 1) * oldVerticeRow) + (x)].Position,
                                                                                  verticeList[((y + 1) * oldVerticeRow) + (x) + 1].Position);
                    }
                }
                //now calculate the middlepoints of the diamonds

                verticeList = tempList;
            }

        }
        private void SetUpIndices()
        {
            indices = new int[6];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 3;
        }


        public Vector3 AverageSquare(Vector3 vec1, Vector3 vec2, Vector3 middlePoint)
        {
            Vector3 newVector = new Vector3((vec1.X + vec2.X)/2, ((vec1.Y+vec2.Y + middlePoint.Z)/3)* ((float)rng.NextDouble() - 0.5f) / 2, (vec1.Z + vec2.Z) / 2);
            return newVector;
        }
        public Vector3 AverageSquare(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4)
        {
            Vector3 newVector = ((vec1 + vec2 + vec3 + vec4) / 4) * new Vector3(1, ((float)rng.NextDouble() - 0.5f) / 2, 1);
            return newVector;
        }

        public void Draw(DrawHelper drawHelper)
        {
            Matrix worldMatrix = Matrix.Identity;

            drawHelper.Draw(verticeList, indices);
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
