using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    class PerlinNoise
    {
        Random rng = new Random();
        /// Perlin Noise Constructot
        public PerlinNoise(int width, int height)
        {
            this.MAX_WIDTH = width;
            this.MAX_HEIGHT = height;
        }

        public int MAX_WIDTH = 256;
        public int MAX_HEIGHT = 256;

        /// Gets the value for a specific X and Y coordinate
        /// results in range [-1, 1] * maxHeight
        public float GetRandomHeight(float X, float Y, float MaxHeight,
            float Frequency, float Amplitude, float Persistance,
            int Octaves)
        {
            GenerateNoise();
            float FinalValue = 0.0f;
            for (int i = 0; i < Octaves; ++i)
            {
                FinalValue += GetSmoothNoise(X * Frequency, Y * Frequency) * Amplitude;
                Frequency *= 2.0f;
                Amplitude *= Persistance;
            }
            if (FinalValue < -1.0f)
            {
                FinalValue = -1.0f;
            }
            else if (FinalValue > 1.0f)
            {
                FinalValue = 1.0f;
            }
            return FinalValue * MaxHeight;
        }

        //This function is a simple bilinear filtering function which is good (and easy) enough.        
        private float GetSmoothNoise(float X, float Y)
        {
            float FractionX = X - (int)X;
            float FractionY = Y - (int)Y;
            int X1 = ((int)X + MAX_WIDTH) % MAX_WIDTH;
            int Y1 = ((int)Y + MAX_HEIGHT) % MAX_HEIGHT;
            //for cool art deco looking images, do +1 for X2 and Y2 instead of -1...
            int X2 = ((int)X + MAX_WIDTH - 1) % MAX_WIDTH;
            int Y2 = ((int)Y + MAX_HEIGHT - 1) % MAX_HEIGHT;
            float FinalValue = 0.0f;
            FinalValue += FractionX * FractionY * Noise[X1, Y1];
            FinalValue += FractionX * (1 - FractionY) * Noise[X1, Y2];
            FinalValue += (1 - FractionX) * FractionY * Noise[X2, Y1];
            FinalValue += (1 - FractionX) * (1 - FractionY) * Noise[X2, Y2];
            return FinalValue;
        }

        float[,] Noise;
        bool NoiseInitialized = false;
        /// create a array of randoms
        private void GenerateNoise()
        {
            if (NoiseInitialized)                //A boolean variable in the class to make sure we only do this once
                return;
            Noise = new float[MAX_WIDTH, MAX_HEIGHT];    //Create the noise table where MAX_WIDTH and MAX_HEIGHT are set to some value>0            
            for (int x = 0; x < MAX_WIDTH; ++x)
            {
                for (int y = 0; y < MAX_HEIGHT; ++y)
                {
                    Noise[x, y] = ((float)rng.NextDouble() - 0.5f) * 2.0f;  //Generate noise between -1 and 1
                }
            }
            NoiseInitialized = true;
        }
    }
}
