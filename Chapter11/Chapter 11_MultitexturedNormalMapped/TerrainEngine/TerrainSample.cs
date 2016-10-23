using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using TerrainEngine.GameBase.Cameras;
using TerrainEngine.GameBase.Lights;
using TerrainEngine.GameBase.Shapes;
using TerrainEngine.GameBase.Materials;

namespace TerrainEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TerrainSample : Microsoft.Xna.Framework.Game
    {
        static String GAME_TITLE = "TerrainEngine Sample";
        GraphicsDeviceManager graphics;
        bool isFullScreen;

        CameraManager cameraManager;
        LightManager lightManager;
        Terrain terrain;

        public TerrainSample()
        {
            Window.Title = GAME_TITLE;
            Content.RootDirectory = "Content";
            isFullScreen = false;            

            graphics = new GraphicsDeviceManager(this);
            ConfigureGraphicsManager();
        }

        /// <summary>
        /// Configure the graphics device manager and checks for shader compatibility
        /// </summary>
        private void ConfigureGraphicsManager()
        {
            #if XBOX360
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            #else
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = isFullScreen;
            #endif

            // Minimum shader profile required
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Hit Q or W to change camera mode";
            base.Initialize();
        }

        private TextureMaterial LoadTextureMaterial(string textureFilename, Vector2 tile)
        {
            Texture2D texture = Content.Load<Texture2D>(GameAssetsPath.TEXTURES_PATH + textureFilename);
            return new TextureMaterial(texture, tile);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Camera Manager
            cameraManager = new CameraManager();
            Services.AddService(typeof(CameraManager), cameraManager);
            float aspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            // Camera 1
            FixedCamera camera1 = new FixedCamera(new Vector3(0, 4000, 1000), Vector3.Zero);
            camera1.AspectRatio = aspectRatio;
            cameraManager.Add("Camera1", camera1);

            // Light Manager
            lightManager = new LightManager();
            lightManager.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
            Services.AddService(typeof(LightManager), lightManager);
            // Light 1
            lightManager.Add("Light1", new PointLight(new Vector3(0, 1000.0f, 0), Vector3.One));

            // Terrain
            terrain = new Terrain(this);
            terrain.Load(Content, "Terrain1", 12.0f, 3.0f);

            // Terrain material
            TerrainMaterial material = new TerrainMaterial();
            material.DiffuseTexture1 = LoadTextureMaterial("Terrain1", new Vector2(50, 50));
            material.DiffuseTexture2 = LoadTextureMaterial("Terrain2", new Vector2(50, 50));
            material.DiffuseTexture3 = LoadTextureMaterial("Terrain3", new Vector2(30, 30));
            material.DiffuseTexture4 = LoadTextureMaterial("Terrain4", Vector2.One);
            material.AlphaMapTexture = LoadTextureMaterial("AlphaMap", Vector2.One);
            material.NormalMapTexture = LoadTextureMaterial("Rockbump", new Vector2(196, 196));
            material.LightMaterial = new LightMaterial(new Vector3(0.8f), new Vector3(0.3f), 32);
            terrain.Material = material;

            // Camera 2 - Positioned over the terrain
            Vector3 cameraPosition = new Vector3(-107, 0, 75);
            cameraPosition.Y = terrain.GetHeight(cameraPosition) + 30;

            FixedCamera camera2 = new FixedCamera();
            camera2.AspectRatio = aspectRatio;
            camera2.Position = cameraPosition;
            camera2.Target = camera2.Position + new Vector3(-3.0f, 0.0f, -1.0f);
            cameraManager.Add("Camera2", camera2);

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        private void UpdateInput()
        {
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Keyboard.GetState(PlayerIndex.One);

            // Exit
            if (gamePad.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            // Change camera
            if (gamePad.Buttons.LeftShoulder == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Q))
                cameraManager.SetActiveCamera(0);
            else if (gamePad.Buttons.RightShoulder == ButtonState.Pressed || keyboard.IsKeyDown(Keys.W))
                cameraManager.SetActiveCamera(1);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateInput();
            terrain.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {            
            GraphicsDevice.Clear(Color.White);

            terrain.Draw(gameTime);

            base.Draw(gameTime);
        }

        #region EntryPoint
        static void Main(string[] args)
        {
            using (TerrainSample game = new TerrainSample())
            {
                game.Run();
            }
        }
        #endregion

    }
}
