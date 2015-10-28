using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    class SkyBox
    {
        public Vector3 position { get; set; }
        private Model model;
        private Texture2D[] textures;

        public SkyBox(Model model, Texture2D[] textures)
        {
            this.model = model;
            this.textures = textures;
        }

        public void Draw(DrawHelper drawHelper)
        {
            drawHelper.Draw(model, textures , position);
        }
    }
}
