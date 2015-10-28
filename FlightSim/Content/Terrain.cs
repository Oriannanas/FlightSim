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
            SetUpVertices(1);
            SetUpIndices();
        }

        private void SetUpVertices(int iterations)
        {
            VertexPositionColor[] tempList = new VertexPositionColor[4];


            tempList[0].Position = new Vector3(0f, 0f, 0f);
            tempList[0].Color = Color.White;
            tempList[1].Position = new Vector3(5f, 0f, 0f);
            tempList[1].Color = Color.White;
            tempList[2].Position = new Vector3(0f, 0f, -5f);
            tempList[2].Color = Color.White;
            tempList[3].Position = new Vector3(5f, 0f, -5f);
            tempList[3].Color = Color.White;

            //make a new array to save the vertices in
            //the new array should be (number of iterations+1)^2
            verticeList = new VertexPositionColor[(int)(Math.Pow(2, iterations)+1)^2];
            //iter stands for iteration
            for (int iter = 0; iter < iterations; iter++)
            {
                int squareAmount = (int)Math.Pow(4, iter) ;
                int oldGridWidth = (int)(Math.Pow(2, iter-1) + 1);
                int oldGridHeight = oldGridWidth;
                int newGridWidth = (int)(Math.Pow(2, iter) + 1);
                int newGridHeight = newGridWidth;
                //first we need to calculate the middle point of every square
                //find out how many squares which is 4^index
                for(int square = 0; square < squareAmount; square++)
                {
                    int x = square % (int)Math.Sqrt(squareAmount);
                    int y = (int)Math.Floor((float)square / (int)Math.Sqrt(squareAmount));

                    verticeList[(y * oldGridWidth) +(x * 2)].Position = tempList[(y * newGridWidth) + (x * 2)].Position;
                    verticeList[0].Position = tempList[(y * newGridWidth) + (x * 2) + 1].Position;
                    verticeList[0].Position = tempList[((y + 1) * newGridWidth) + (x * 2)].Position;
                    verticeList[0].Position = tempList[((y + 1) * newGridWidth) + (x * 2) + 1].Position;

                    //use the RandomAverage method to find the middle point of each square
                    RandomAverage(  tempList[(y*newGridWidth) +(x * 2)].Position, 
                                    tempList[(y * newGridWidth) + (x * 2) + 1].Position, 
                                    tempList[((y+1) * newGridWidth) + (x * 2)].Position, 
                                    tempList[((y+1) * newGridWidth) +(x * 2)+1].Position);



                }


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

        public void Draw(DrawHelper drawHelper)
        {
            Matrix worldMatrix = Matrix.Identity;

            drawHelper.Draw(verticeList, indices);
        }

        public Vector3 RandomAverage(Vector3 vec1, Vector3 vec2)
        {
            Vector3 newVector = ((vec1+vec2)/2) *new Vector3(0,((float)rng.NextDouble()-0.5f)/2,0);
            return newVector;
        }

        public Vector3 RandomAverage(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4)
        {
            Vector3 newVector = ((vec1 + vec2 + vec3 + vec4) / 4) * new Vector3(0, ((float)rng.NextDouble() - 0.5f) / 2, 0);
            return newVector;
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
