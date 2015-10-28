using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class DrawHelper
    {
        private Game1 game;
        private GraphicsDevice GraphicsDevice;
        private Effect effect;
        public Matrix viewMatrix { get; set; }
        public Matrix projectionMatrix { get; set; }

        public Vector3 lightDirection { get; set; }
        public float ambientValue { get; set; } = 0.5f;

        public DrawHelper(Game1 game, Effect effect, Vector3 lightDirection)
        {
            this.game = game;
            this.GraphicsDevice = game.GraphicsDevice;
            this.effect = effect;
            this.lightDirection = lightDirection;
        }
        public void Draw(Model model, Matrix worldMatrix)
        {

            Matrix[] targetTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(targetTransforms);
            foreach (ModelMesh modmesh in model.Meshes)
            {
                foreach (Effect currentEffect in modmesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                    currentEffect.Parameters["xWorld"].SetValue(targetTransforms[modmesh.ParentBone.Index] * worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                    currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
                    currentEffect.Parameters["xAmbient"].SetValue(ambientValue);
                }
                modmesh.Draw();
            }
        }
        public void Draw(Model model, Texture2D texture, Matrix worldMatrix)
        {
            
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(modelTransforms[mesh.ParentBone.Index] * worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                    currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
                    currentEffect.Parameters["xTexture"].SetValue(texture);
                    currentEffect.Parameters["xAmbient"].SetValue(ambientValue);
                }
                mesh.Draw();
            }
        }

        public void Draw(VertexBuffer vertexBuffer, Texture2D texture, Matrix worldMatrix)
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xTexture"].SetValue(texture);
            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(ambientValue);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.SetVertexBuffer(vertexBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            }
        }
        public void Draw(Model model, Texture2D[] textures, Vector3 position)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            GraphicsDevice.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            GraphicsDevice.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(position);
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;
        }

        public void DrawSprite(VertexPositionTexture[] vertices, Texture2D texture, Matrix worldMatrix)
        {
            effect.CurrentTechnique = effect.Techniques["PointSprites"];
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xCamPos"].SetValue(game.cameraPosition);
            effect.Parameters["xTexture"].SetValue(texture);
            effect.Parameters["xCamUp"].SetValue(game.cameraUpDirection);
            effect.Parameters["xPointSpriteSize"].SetValue(0.1f);

            GraphicsDevice.BlendState = BlendState.Additive;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length /3);
            }
            game.GraphicsDevice.BlendState = BlendState.Opaque;
        }
        
    }
}
