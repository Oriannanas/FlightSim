using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    class Plane
    {
        Game1 game;
        /*** the xwing variables ***/
        Model model;
        Texture2D texture;
        Texture2D bulletTexture;

        double lastBulletTime = 0;
        public Vector3 position{ get; private set; } = new Vector3(8, 1, -3);

        public float moveSpeed { get; private set; }
        // Up and down rotational variables
        public Quaternion rotation { get; private set; } = Quaternion.Identity;
        float UpDownAcceleration = 0.003f;
        float curUpDownRot = 0;
        float desUpDownRot = 0;

        List<Bullet> bulletList = new List<Bullet>();

        public Plane(Game1 game, Model xwingModel, Texture2D xwingTexture, Texture2D bulletTexture)
        {
            this.game = game;
            this.model = xwingModel;
            this.texture = xwingTexture;
            this.bulletTexture = bulletTexture;
        }

        public void Update(GameTime gameTime)
        {
            UpdateBullets();
            position = position.MoveForward(rotation, moveSpeed);

            moveSpeed = (gameTime.ElapsedGameTime.Milliseconds / 750.0f) * game.gameSpeed;
            float turningSpeed = ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f) * game.gameSpeed;

            float leftRightRot = 0;
            
            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Right))
                leftRightRot += turningSpeed;
            if (keys.IsKeyDown(Keys.Left))
                leftRightRot -= turningSpeed;

            if (keys.IsKeyDown(Keys.W))
            {
                moveSpeed *= 1.5f;
            }
            if (keys.IsKeyDown(Keys.S))
            {
                moveSpeed *= 0.5f;
            }

            if (keys.IsKeyDown(Keys.Down))
                desUpDownRot += turningSpeed;
            else if (keys.IsKeyDown(Keys.Up))
                desUpDownRot -= turningSpeed;
            else
            {
                desUpDownRot = 0;
            }
            if (keys.IsKeyDown(Keys.Space))
            {
                double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                if (currentTime - lastBulletTime > 100)
                {
                    Bullet newBullet = new Bullet(position, rotation);
                    bulletList.Add(newBullet);

                    lastBulletTime = currentTime;
                }
            }

            if (curUpDownRot < desUpDownRot - UpDownAcceleration)
            {
                if (curUpDownRot < turningSpeed)
                {
                    curUpDownRot += UpDownAcceleration;
                }
            }
            else if (curUpDownRot > desUpDownRot + UpDownAcceleration)
            {
                if (curUpDownRot > -turningSpeed)
                {
                    curUpDownRot -= UpDownAcceleration;
                }
            }
            else
            {
                curUpDownRot = desUpDownRot;
            }
            Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), curUpDownRot);
            rotation *= additionalRot;
        }
        
        public void ResetPosition()
        {
            position = new Vector3(8, 1, -3);
            rotation = Quaternion.Identity;
        }
        public void Draw(DrawHelper drawHelper)
        {
            Matrix worldMatrix = Matrix.CreateScale(0.06f, 0.06f, 0.06f) * Matrix.CreateRotationX(MathHelper.Pi / 2) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            DrawBullets(drawHelper);
            drawHelper.Draw(model, texture, worldMatrix);
        }
        private void UpdateBullets()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                Bullet currentBullet = bulletList[i];
                currentBullet.position = currentBullet.position.MoveForward(currentBullet.rotation, moveSpeed * 4.0f);
                bulletList[i] = currentBullet;
                BoundingSphere bulletSphere = new BoundingSphere(currentBullet.position, 0.05f);
                CollisionType colType = game.CheckCollision(bulletSphere);
                if (colType != CollisionType.None)
                {
                    bulletList.RemoveAt(i);
                    i--;

                    if (colType == CollisionType.Target)
                        game.gameSpeed *= 1.1f;
                }

            }
        }
        private void DrawBullets(DrawHelper drawHelper)
        {
            if (bulletList.Count > 0)
            {
                VertexPositionTexture[] bulletVertices = new VertexPositionTexture[bulletList.Count * 6];
                int i = 0;
                foreach (Bullet currentBullet in bulletList)
                {
                    Vector3 center = currentBullet.position;

                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 0));

                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                }

                Matrix worldMatrix = Matrix.Identity;
                VertexBuffer bulletVertexBuffer = new VertexBuffer(game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, bulletVertices.Length, BufferUsage.WriteOnly);
                drawHelper.DrawSprite(bulletVertices, bulletTexture, worldMatrix);
            }
        }
    }
}
