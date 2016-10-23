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

using XNA_TPS.Helpers;
using XNA_TPS.GameBase;
using XNA_TPS.GameLogic.Levels;

namespace XNA_TPS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TPSGame : Microsoft.Xna.Framework.Game
    {
        static String GAME_TITLE = "XNA TPS v1.0";
        GraphicsDeviceManager graphics;

        // Game stuff
        InputHelper inputHelper;

        public TPSGame()
        {
            Window.Title = GAME_TITLE;
            Content.RootDirectory = "Content";

            // Creating and configuring graphics device
            graphics = new GraphicsDeviceManager(this);
            GameSettings gameSettings = SettingsManager.Read(Content.RootDirectory + "/" +
                GameAssetsPath.SETTINGS_PATH + "GameSettings.xml");
            ConfigureGraphicsManager(gameSettings);
            
            // Input helper
            inputHelper = new InputHelper(PlayerIndex.One,
                SettingsManager.GetKeyboardDictionary(gameSettings.KeyboardSettings[0]));
            Services.AddService(typeof(InputHelper), inputHelper);

            // Game Screen
            Components.Add(new GameScreen(this, LevelCreator.Levels.AlienPlanet));
        }

        /// <summary>
        /// Configure the graphics device manager and checks for shader compatibility
        /// </summary>
        private void ConfigureGraphicsManager(GameSettings gameSettings)
        {
#if XBOX360
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
#else
            graphics.PreferredBackBufferWidth = gameSettings.PreferredWindowWidth;
            graphics.PreferredBackBufferHeight = gameSettings.PreferredWindowHeight;
            graphics.IsFullScreen = gameSettings.PreferredFullScreen;
#endif

            // Multi sampling
            graphics.PreferMultiSampling = true;

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
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Update input
            inputHelper.Update();
            if (inputHelper.IsKeyJustPressed(Buttons.Back)) this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
