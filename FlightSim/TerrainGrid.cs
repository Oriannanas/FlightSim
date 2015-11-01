using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class TerrainGrid
    {
        private Game1 game;

        private Terrain[] terrains;
        private int gridWidth;
        private int iterations;

        private int terrainWidth;

        public TerrainGrid(Game1 game, int iterations, int terrainWidth)
        {
            this.game = game;
            this.iterations = iterations;
            this.terrainWidth = terrainWidth;
            gridWidth = (int)Math.Sqrt((double)(short.MaxValue));
            terrains = new Terrain[short.MaxValue];
            
        }

        public void LoadTerrain(int x, int z)
        {
            if (terrains[z * gridWidth + x + terrains.Length/2] == null)
            {
                terrains[z * gridWidth + x + terrains.Length / 2] = new Terrain(z * gridWidth + x + terrains.Length / 2,game, this, new Vector2(x, z), terrainWidth, 1024, iterations, 0, 0.85f);
            }
        }

        public int CheckTerrain(int index, int x, int z)
        {
            if(terrains[index + z * gridWidth + x] != null)
            {
                return index + z * gridWidth + x;
            }
            else
            {
                return short.MinValue;
            }
        }

        public float[] GetTerrainEdge(int index, int x, int z)
        {
            if (x > 0)
            {
                return terrains[index].rightEdge;
            }
            else if (x < 0)
            {
                return terrains[index].leftEdge;
            }
            else if (z > 0)
            {
                return terrains[index].bottomEdge;
            }
            else
            {
                return terrains[index].topEdge;
            }
        }
        public void Draw(DrawHelper drawHelper)
        {
            foreach (Terrain terrain in terrains)
            {
                if (terrain != null)
                {
                    terrain.Draw(drawHelper);
                }
            }
        }
    }
}
