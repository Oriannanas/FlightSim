﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class DiamondSquare
    {
        private Random rng = new Random();
        
        private DiamondSquare leftEdge = null;
        private DiamondSquare rightEdge = null;
        private DiamondSquare topEdge = null;
        private DiamondSquare bottomEdge = null;

        public float[] valueList;
        private float[] cornerValues;

        private int squaresPerRow;
        private int squaresPerGrid;
        private int pointsPerRow;
        private int pointsPerGrid;

        int iterations;
        float roughness;
        bool clamp;

        public DiamondSquare(int iterations, float roughness, bool clamp, float[] cornerValues)
        {
            this.cornerValues = cornerValues;
            this.iterations = iterations;
            this.roughness = roughness;
            this.clamp = clamp;

            GenerateValues();
        }
        public DiamondSquare(int iterations, float roughness, bool clamp, float[] cornerValues, DiamondSquare[] neighboors, Side[] neighboorSides)
        {
            this.cornerValues = cornerValues;
            this.iterations = iterations;
            this.roughness = roughness;
            this.clamp = clamp;

            for (int i = 0; i < neighboors.Length; i++)
            {
                if (neighboorSides[i] == Side.Bottom)
                {
                    bottomEdge = neighboors[i];
                }
                if (neighboorSides[i] == Side.Top)
                {
                    topEdge = neighboors[i];
                }
                if (neighboorSides[i] == Side.Left)
                {
                    leftEdge = neighboors[i];
                }
                if (neighboorSides[i] == Side.Right)
                {
                    rightEdge = neighboors[i];
                }
            }
            GenerateValues();
        }

        private void GenerateValues()
        {
            squaresPerRow = (int)Math.Pow(2, iterations);
            squaresPerGrid = (int)Math.Pow(squaresPerRow, 2);
            pointsPerRow = (int)(Math.Pow(2, iterations) + 1);
            pointsPerGrid = (int)Math.Pow(pointsPerRow, 2);

            valueList = new float[pointsPerGrid];

            valueList[0] = cornerValues[0];
            valueList[pointsPerRow - 1] = cornerValues[1];
            valueList[pointsPerGrid - pointsPerRow] = cornerValues[2];
            valueList[pointsPerGrid - 1] = cornerValues[3];

            Console.Write("initial: ");
            Console.WriteLine(valueList[0] + ", " + valueList[pointsPerRow - 1] + ", " + valueList[pointsPerGrid - pointsPerRow] + ", " + valueList[pointsPerGrid - 1]);

            for (int iter = 1; iter < iterations; iter++)
            {
                float amplitude = (float)(1f / Math.Pow(roughness + 1, iter - 1));
                int iterSquaresRow = (int)Math.Pow(2, iter);
                int verticesPerSquare = ((pointsPerRow - 1) / iterSquaresRow);
                for (int x = 0; x < iterSquaresRow; x++)
                {
                    for (int z = 0; z < iterSquaresRow; z++)
                    {
                        int leftTopPoint = z * pointsPerRow * verticesPerSquare + x * verticesPerSquare;
                        int rightTopPoint = z * verticesPerSquare * pointsPerRow + x * verticesPerSquare + verticesPerSquare;
                        int middlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * pointsPerRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int leftBottomPoint = (z * verticesPerSquare + verticesPerSquare) * pointsPerRow + x * verticesPerSquare;
                        int rightBottomPoint = (z * verticesPerSquare + verticesPerSquare) * pointsPerRow + x * verticesPerSquare + verticesPerSquare;


                        valueList[middlePoint] = AverageRandomFloat(valueList[leftTopPoint], valueList[rightTopPoint], valueList[leftBottomPoint], valueList[rightBottomPoint], amplitude);
                        if(middlePoint == 0)
                        {
                            Console.WriteLine("middlepoint");
                        }
                    }
                }
                for (int x = 0; x < iterSquaresRow; x++)
                {
                    for (int z = 0; z < iterSquaresRow; z++)
                    {
                        int leftTopPoint = z * pointsPerRow * verticesPerSquare + x * verticesPerSquare;
                        int topMiddlePoint = z * verticesPerSquare * pointsPerRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightTopPoint = z * verticesPerSquare * pointsPerRow + x * verticesPerSquare + verticesPerSquare;
                        int leftMiddlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * pointsPerRow + x * verticesPerSquare;
                        int middlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * pointsPerRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightMiddlePoint = (z * verticesPerSquare + verticesPerSquare / 2) * pointsPerRow + x * verticesPerSquare + verticesPerSquare;
                        int leftBottomPoint = (z * verticesPerSquare + verticesPerSquare) * pointsPerRow + x * verticesPerSquare;
                        int bottomMiddlePoint = (z * verticesPerSquare + verticesPerSquare) * pointsPerRow + x * verticesPerSquare + verticesPerSquare / 2;
                        int rightBottomPoint = (z * verticesPerSquare + verticesPerSquare) * pointsPerRow + x * verticesPerSquare + verticesPerSquare;
                        if (x == 0)
                        {
                            if (leftEdge == null)
                            {
                                valueList[leftMiddlePoint] = AverageRandomFloat(valueList[leftTopPoint], valueList[leftBottomPoint], valueList[middlePoint], amplitude);
                            }
                            else
                            {
                                int neighboorValue = (z * verticesPerSquare + verticesPerSquare / 2) * pointsPerRow + (iterSquaresRow - 1) * verticesPerSquare + verticesPerSquare / 2;
                                valueList[leftMiddlePoint] = AverageRandomFloat(valueList[leftTopPoint], valueList[leftBottomPoint], valueList[middlePoint], leftEdge.valueList[neighboorValue], amplitude);
                            }
                            if (leftMiddlePoint == 0)
                            {
                                Console.WriteLine("leftMiddlePoint X:" + x + "z " + z + "iter " + iter);
                            }
                        }
                        if (z == 0)
                        {
                            if (topEdge == null)
                            {
                                valueList[topMiddlePoint] = AverageRandomFloat(valueList[leftTopPoint], valueList[rightTopPoint], valueList[middlePoint], amplitude);
                            }
                            else
                            {
                                valueList[topMiddlePoint] = AverageRandomFloat(valueList[leftTopPoint], valueList[rightTopPoint], valueList[middlePoint],
                                   topEdge.valueList[((iterSquaresRow - 1) * verticesPerSquare + verticesPerSquare / 2) * pointsPerRow + x * verticesPerSquare + verticesPerSquare / 2], amplitude);
                            }
                            if (topMiddlePoint == 0)
                            {
                                Console.WriteLine("topMiddlePoint");
                            }
                        }
                        if (x == iterSquaresRow - 1)
                        {
                            if (rightEdge == null)
                            {
                                valueList[rightMiddlePoint] = AverageRandomFloat(valueList[rightTopPoint], valueList[rightBottomPoint], valueList[middlePoint], amplitude);
                            }
                            else
                            {
                                valueList[rightMiddlePoint] = AverageRandomFloat(valueList[rightTopPoint], valueList[rightBottomPoint], valueList[middlePoint],
                                    rightEdge.valueList[(z * verticesPerSquare + verticesPerSquare / 2) * pointsPerRow + verticesPerSquare / 2], amplitude);
                            }
                        }
                        else
                        {
                            valueList[rightMiddlePoint] = AverageRandomFloat(valueList[rightTopPoint], valueList[rightBottomPoint], valueList[middlePoint], valueList[rightMiddlePoint + verticesPerSquare / 2], amplitude);

                        }
                        if (rightMiddlePoint == 0)
                        {
                            Console.WriteLine("rightMiddlePoint");
                        }
                        if (z == iterSquaresRow - 1)
                        {
                            if (bottomEdge == null)
                            {
                                valueList[bottomMiddlePoint] = AverageRandomFloat(valueList[leftBottomPoint], valueList[rightBottomPoint], valueList[middlePoint], amplitude);
                            }
                            else
                            {
                                valueList[bottomMiddlePoint] = AverageRandomFloat(valueList[leftBottomPoint], valueList[rightBottomPoint], valueList[middlePoint],
                                    bottomEdge.valueList[(verticesPerSquare / 2) * pointsPerRow + x * verticesPerSquare + verticesPerSquare / 2], amplitude);
                            }
                        }
                        else
                        {
                            valueList[bottomMiddlePoint] = AverageRandomFloat(valueList[leftBottomPoint], valueList[rightBottomPoint], valueList[middlePoint], valueList[bottomMiddlePoint + (verticesPerSquare / 2) * pointsPerRow], amplitude);
                        }
                        if (bottomMiddlePoint == 0)
                        {
                            Console.WriteLine("bottomMiddlePoint");
                        }
                    }
                }
            }/*
            if (clamp)
            {
                float highestValue = 0.5f;
                float lowestValue = 0.5f;
                for (int i = 0; i < valueList.Length; i++)
                {
                    if (valueList[i] < lowestValue)
                    {
                        lowestValue = valueList[i];
                    }

                    if (valueList[i] > highestValue)
                    {
                        highestValue = valueList[i];
                    }
                }
                for (int i = 0; i < valueList.Length; i++)
                {
                    valueList[i] -= lowestValue;
                }
                float multiplier = 1 / (highestValue - lowestValue);

                for (int i = 0; i < valueList.Length; i++)
                {
                    valueList[i] = valueList[i] * multiplier;
                }
            }*/

            Console.Write("after: ");
            Console.WriteLine(valueList[0] + ", " + valueList[pointsPerRow - 1] + ", " + valueList[pointsPerGrid - pointsPerRow] + ", " + valueList[pointsPerGrid - 1]);
        }

        private void RegenerateRightEdge()
        {

        }

        private void RegenerateLeftEdge()
        {

        }

        private void RegenerateTopEdge()
        {

        }

        private void RegenerateBottomEdge()
        {

        }

        private float AverageRandomFloat(float value1, float value2, float value3, float amplitude)
        {
            float newY = (value1 + value2 + value3) / 3;
            newY += (((float)rng.NextDouble() - 0.5f) * amplitude);
            return newY;
        }

        private float AverageRandomFloat(float value1, float value2, float value3, float value4, float amplitude)
        {
            float newY = (value1 + value2 + value3 + value4) / 4;
            newY += (((float)rng.NextDouble() - 0.5f) * amplitude);
            return newY;
        }
    }
}
