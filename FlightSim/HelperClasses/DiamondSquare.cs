using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class DiamondSquare
    {
        private Random rng = new Random();

        public float[] valueList;

        private int squaresPerRow;
        private int squaresPerGrid;
        private int pointsPerRow;
        private int pointsPerGrid;

        public DiamondSquare(int iterations, float roughness, bool clamp)
        {
            squaresPerRow = (int)Math.Pow(2, iterations);
            squaresPerGrid = (int)Math.Pow(squaresPerRow, 2);
            pointsPerRow = (int)(Math.Pow(2, iterations) + 1);
            pointsPerGrid = (int)Math.Pow(pointsPerRow, 2);

            valueList = new float[pointsPerGrid];

            valueList[0] = (float)(rng.NextDouble());
            valueList[pointsPerRow - 1] = (float)(rng.NextDouble());
            valueList[pointsPerGrid - pointsPerRow] = (float)(rng.NextDouble());
            valueList[pointsPerGrid - 1] = (float)(rng.NextDouble());

            for (int iter = 1; iter <= iterations; iter++)
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


                        valueList[middlePoint] = AverageRandomInt(valueList[leftTopPoint], valueList[rightTopPoint], valueList[leftBottomPoint], valueList[rightBottomPoint], amplitude);
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
                            valueList[leftMiddlePoint] = AverageRandomInt(valueList[leftTopPoint], valueList[leftBottomPoint], valueList[middlePoint], amplitude);
                        }
                        if (z == 0)
                        {
                            valueList[topMiddlePoint] = AverageRandomInt(valueList[leftTopPoint], valueList[rightTopPoint], valueList[middlePoint], amplitude);
                        }


                        if (x == iterSquaresRow - 1)
                        {
                            valueList[rightMiddlePoint] = AverageRandomInt(valueList[rightTopPoint], valueList[rightBottomPoint], valueList[middlePoint], amplitude);
                        }
                        else
                        {
                            valueList[rightMiddlePoint] = AverageRandomInt(valueList[rightTopPoint], valueList[rightBottomPoint], valueList[middlePoint], valueList[rightMiddlePoint + verticesPerSquare / 2], amplitude);
                        }
                        if (z == (iterSquaresRow - 1))
                        {
                            valueList[bottomMiddlePoint] = AverageRandomInt(valueList[leftBottomPoint], valueList[rightBottomPoint], valueList[middlePoint], amplitude);
                        }
                        else
                        {
                            valueList[bottomMiddlePoint] = AverageRandomInt(valueList[leftBottomPoint], valueList[rightBottomPoint], valueList[middlePoint], valueList[bottomMiddlePoint + (verticesPerSquare / 2) * pointsPerRow], amplitude);
                        }
                    }
                }

            }
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
            }            
        }
        private float AverageRandomInt(float value1, float value2, float value3, float amplitude)
        {
            float newY = (value1 + value2 + value3) / 3;
            newY += (((float)rng.NextDouble() - 0.5f) * amplitude);
            return newY;
        }

        private float AverageRandomInt(float value1, float value2, float value3, float value4, float amplitude)
        {
            float newY = (value1 + value2 + value3 + value4) / 4;
            newY += (((float)rng.NextDouble() - 0.5f) * amplitude);
            return newY;
        }
    }
}
