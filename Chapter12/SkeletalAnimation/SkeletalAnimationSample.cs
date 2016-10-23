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

using SkeletalAnimation.GameBase.Cameras;
using SkeletalAnimation.GameBase.Lights;
using SkeletalAnimation.GameBase.Shapes;
using SkeletalAnimation.GameBase.Materials;

namespace SkeletalAnimation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SkeletalAnimationSample : Microsoft.Xna.Framework.Game
    {
        static String GAME_TITLE = "SkeletalAnimation Sample";
        GraphicsDeviceManager graphics;
        bool isFullScreen;

        CameraManager cameraManager;
        LightManager lightManager;

        int activeModel;
        AnimatedModel[] animatedModel;

        public SkeletalAnimationSample()
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
            Window.Title = "Hit Q or W to change Model; Hit 0,1,2,3 or 4 at the top of the keyboard to change Animation";
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
            FixedCamera camera1 = new FixedCamera(new Vector3(0, 1.4f, 4), new Vector3(0, 1.4f, 0));
            camera1.AspectRatio = aspectRatio;
            cameraManager.Add("Camera1", camera1);
            // Camera 2
            FixedCamera camera2 = new FixedCamera(new Vector3(0, 4, 20), new Vector3(0, 5, 0));
            camera2.AspectRatio = aspectRatio;
            cameraManager.Add("Camera2", camera2);

            // Light Manager
            lightManager = new LightManager();
            lightManager.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
            Services.AddService(typeof(LightManager), lightManager);
            // Light 1
            lightManager.Add("Light1", new PointLight(new Vector3(0, 500.0f, 1000.0f), Vector3.One));

            // Animated Models
            //xxx
            activeModel = 1;
            animatedModel = new AnimatedModel[2];
            animatedModel[0] = new AnimatedModel(this);
            animatedModel[0].Load("PlayerMarine");
            animatedModel[1] = new AnimatedModel(this);
            animatedModel[1].Load("EnemyBeast");

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
                activeModel = 0;
            else if (gamePad.Buttons.RightShoulder == ButtonState.Pressed || keyboard.IsKeyDown(Keys.W))
                activeModel = 1;

            if (keyboard.IsKeyDown(Keys.D0))
            {
                if (animatedModel[activeModel].Animations.Length > 0)
                    animatedModel[activeModel].ActiveAnimation = animatedModel[activeModel].Animations[0];
            }
            else if (keyboard.IsKeyDown(Keys.D1))
            {
                if (animatedModel[activeModel].Animations.Length > 1)
                    animatedModel[activeModel].ActiveAnimation = animatedModel[activeModel].Animations[1];
            }
            else if (keyboard.IsKeyDown(Keys.D2))
            {
                if (animatedModel[activeModel].Animations.Length > 2)
                    animatedModel[activeModel].ActiveAnimation = animatedModel[activeModel].Animations[2];
            }
            else if (keyboard.IsKeyDown(Keys.D3))
            {
                if (animatedModel[activeModel].Animations.Length > 3)
                    animatedModel[activeModel].ActiveAnimation = animatedModel[activeModel].Animations[3];
            }
            else if (keyboard.IsKeyDown(Keys.D4))
            {
                if (animatedModel[activeModel].Animations.Length > 4)
                    animatedModel[activeModel].ActiveAnimation = animatedModel[activeModel].Animations[4];
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateInput();

            cameraManager.SetActiveCamera(activeModel);
            animatedModel[activeModel].Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            animatedModel[activeModel].Draw(gameTime);

            base.Draw(gameTime);
        }

        #region EntryPoint
        static void Main(string[] args)
        {
            using (SkeletalAnimationSample game = new SkeletalAnimationSample())
            {
                game.Run();
            }
        }
        #endregion

    }
}
