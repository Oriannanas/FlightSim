using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FlightSim
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public enum CollisionType { None, Building, Boundary, Target }

    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect effect;

        Vector3 lightDirection = new Vector3(3, -2, 5);

        public Model targetModel { get; private set; }

        public BoundingBox[] buildingBoundingBoxes { get; set; }
        public BoundingBox completeCityBox { get; set; }

        Plane xwing;
        City city;
        SkyBox skyBox;
        public List<Target> targetList { get; set; } = new List<Target>();

        Model xwingModel;
        Texture2D xwingTexture;

        Texture2D[] skyboxTextures;
        Model skyboxModel;

        Texture2D sceneryTexture;
        Texture2D texture;

        DrawHelper drawHelper;

        public float gameSpeed { get; set; } = 1.0f;

        Texture2D bulletTexture;

        public Vector3 cameraPosition { get; private set; }
        public Vector3 cameraUpDirection { get; private set; }


        Quaternion cameraRotation = Quaternion.Identity;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            
            Window.Title = "Flight Simulator";

            lightDirection.Normalize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("effects");
            drawHelper = new DrawHelper(this,  effect, lightDirection);

            texture = Content.Load<Texture2D>("riemerstexture");
            bulletTexture = Content.Load<Texture2D>("bullet");

            sceneryTexture = Content.Load<Texture2D>("texturemap");
            xwingTexture = Content.Load<Texture2D>("xwingText");
            skyboxModel = LoadModel("skybox", out skyboxTextures);

            xwingModel = LoadModel("xwing");
            targetModel = LoadModel("target");

            city = new City(this, sceneryTexture);
            xwing = new Plane(this, xwingModel, xwingTexture, bulletTexture);
            skyBox = new SkyBox(skyboxModel, skyboxTextures);

            // TODO: use this.Content to load your game content here
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);
            foreach (ModelMesh mesh in newModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }
            return newModel;
        }

        private Model LoadModel(string assetName, out Texture2D[] textures)
        {
            int o = 0;
            Model newModel = Content.Load<Model>(assetName);
            foreach(ModelMesh mesh in newModel.Meshes)
            {
                    o += mesh.Effects.Count;
            }
            textures = new Texture2D[o];
            Console.WriteLine(newModel.Meshes[0]);
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    textures[i] = currentEffect.Texture;
                    i++;
                }

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            UpdateCamera();

            xwing.Update(gameTime);

            BoundingSphere xwingSpere = new BoundingSphere(xwing.position, 0.04f);

            

            if (CheckCollision(xwingSpere) != CollisionType.None)
            {
                xwing.ResetPosition();
                gameSpeed /= 1.1f;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            cameraRotation = Quaternion.Lerp(cameraRotation, xwing.rotation, 0.3f);

            Vector3 campos = new Vector3(0, 0.05f, 0.6f);
            campos = Vector3.Transform(campos, Matrix.CreateFromQuaternion(cameraRotation));
            campos += xwing.position;

            Vector3 camup = new Vector3(0, 1, 0);
            camup = Vector3.Transform(camup, Matrix.CreateFromQuaternion(cameraRotation));

            drawHelper.viewMatrix = Matrix.CreateLookAt(campos, xwing.position, camup);
            drawHelper.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.2f, 500.0f);

            cameraPosition = campos;
            cameraUpDirection = camup;

            skyBox.position = xwing.position;

        }

        public CollisionType CheckCollision(BoundingSphere sphere)
        {
            for (int i = 0; i < buildingBoundingBoxes.Length; i++)
                if (buildingBoundingBoxes[i].Contains(sphere) != ContainmentType.Disjoint)
                    return CollisionType.Building;

            if (completeCityBox.Contains(sphere) != ContainmentType.Contains)
                return CollisionType.Boundary;

            for (int i = 0; i < targetList.Count;)
            {
                if (targetList[i].boundingSphere.Contains(sphere) != ContainmentType.Disjoint)
                {
                    targetList.Remove(targetList[i]);
                    return CollisionType.Target;
                }
                else {
                    i++;
                }
            }

            return CollisionType.None;
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            
            skyBox.Draw(drawHelper);
            city.Draw(drawHelper);
            xwing.Draw(drawHelper);
            foreach( Target target in targetList)
            {
                target.Draw(drawHelper);
            }
            base.Draw(gameTime);
        }
        
        
    }
}

