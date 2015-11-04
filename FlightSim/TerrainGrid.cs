using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class TerrainGrid
    {
        Random rng;
        private Game1 game;
        private DiamondSquare heightMap;
        private Terrain[] terrains;
        private int gridWidth;
        private int iterations;

        private int terrainWidth;

        public TerrainGrid(Game1 game, int iterations, int terrainWidth)
        {
            rng = new Random();
            heightMap = new DiamondSquare(rng, 5, 1, false, new float[4] {0,0,0,0});
            for(int i = 0; i < heightMap.valueList.Length; i++)
            {
                heightMap.valueList[i] *= 10;
            }
            this.game = game;
            this.iterations = iterations;
            this.terrainWidth = terrainWidth;
            gridWidth = (int)Math.Sqrt((double)heightMap.valueList.Length)-1;
            terrains = new Terrain[gridWidth*gridWidth];
            
        }

        public void LoadTerrain(int x, int z)
        {
            int index = z * gridWidth + x + terrains.Length / 2;
            if (terrains[index] == null)
            {
                terrains[index] = new Terrain(rng, index, game, this, new Vector2(x, z), terrainWidth, 64, iterations, 3, 1f, 
                    new float[4] { heightMap.valueList[z*gridWidth + x + terrains.Length/2], heightMap.valueList[z * gridWidth + x + terrains.Length / 2 + 1], heightMap.valueList[(z + 1) * gridWidth + x + terrains.Length / 2], heightMap.valueList[(z+1) * gridWidth + x + terrains.Length / 2 + 1] });
            }
        }

        public Terrain GetTerrain(int index, int x, int z)
        {
                return terrains[index + z * gridWidth + x];
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
