using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlightSim
{
    class SkyBox
    {
        private Game1 game;
        public Vector3 position { get; set; }
        private Model model;
        private Texture2D[] textures;

        public SkyBox(Game1 game, Model model, Texture2D texture)
        {
            this.game = game;
            this.model = model;
            this.textures = SplitTexture(texture);
        }
        public SkyBox(Game1 game, Model model, Texture2D[] textures)
        {
            this.game = game;
            this.model = model;
            this.textures = textures;
        }

        public void Draw(DrawHelper drawHelper)
        {
            drawHelper.Draw(model, textures , position);
        }

        private Texture2D[] SplitTexture(Texture2D texture)
        {
            Texture2D[] textures = new Texture2D[6];
            List<Color[]> newTextCol = new List<Color[]>();
            Rectangle[] rects = new Rectangle[6];
            int newTextureWidth = texture.Width / 4;
            int newTextureHeight = texture.Height / 3;
            int newTextureIndex = newTextureWidth * newTextureHeight;

            Color[] textColors = new Color[texture.Width * texture.Height];
            texture.GetData(textColors);

            for (int i = 0; i < 6; i++)
            {
                textures[i] = new Texture2D(game.GraphicsDevice, newTextureHeight, newTextureWidth);
                newTextCol.Add(new Color[newTextureIndex]);
            }

            rects[0] = new Rectangle(newTextureWidth, 0, newTextureWidth, newTextureHeight);
            rects[2] = new Rectangle(0, newTextureHeight, newTextureWidth, newTextureHeight);
            rects[3] = new Rectangle(newTextureWidth, newTextureHeight, newTextureWidth, newTextureHeight);
            rects[1] = new Rectangle(newTextureWidth * 2, newTextureHeight, newTextureWidth, newTextureHeight);
            rects[4] = new Rectangle(newTextureWidth * 3, newTextureHeight, newTextureWidth, newTextureHeight);
            rects[5] = new Rectangle(newTextureWidth, newTextureHeight * 2, newTextureWidth, newTextureHeight);



            for (int i = 0; i < 6; i++)
            {
                texture.GetData(0, rects[i], newTextCol[i], 0, newTextureIndex);

                textures[i].SetData(newTextCol[i]);

                /*//save to test the created splitted image ( saved in bin/windows/debug )
                Stream stream = File.Create("skyboxtexturetest" + i + ".png");
                textures[i].SaveAsPng(stream, textures[i].Width, textures[i].Height);
                stream.Dispose();*/
            }


            return textures;
        }
    }
}
