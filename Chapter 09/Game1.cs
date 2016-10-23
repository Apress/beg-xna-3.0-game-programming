using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using RockRainZune.Core;

namespace RockRainZune
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Textures
	protected Texture2D helpBackgroundTexture;
        protected Texture2D startBackgroundTexture, startElementsTexture;
        protected Texture2D actionElementsTexture, actionBackgroundTexture;

        // Game Scenes
        protected HelpScene helpScene;
        protected StartScene startScene;
        protected ActionScene actionScene;
        protected GameScene activeScene;

        // Audio Stuff
        private AudioLibrary audio;

        // Fonts
        private SpriteFont smallFont, largeFont, scoreFont;

	// Used for handle input
        protected GamePadState oldGamePadState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Used for input handling
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            // Create the audio bank
            audio = new AudioLibrary();
            audio.LoadContent(Content);
            Services.AddService(typeof(AudioLibrary), audio);

            // Create the Start Scene
            smallFont = Content.Load<SpriteFont>("menuSmall");
            largeFont = Content.Load<SpriteFont>("menuLarge");
            startBackgroundTexture = Content.Load<Texture2D>("startbackground");
            startElementsTexture = Content.Load<Texture2D>("startSceneElements");
            startScene = new StartScene(this, smallFont, largeFont,
                startBackgroundTexture, startElementsTexture);
            Components.Add(startScene);

            // Create the Action Scene
            actionElementsTexture = Content.Load<Texture2D>("rockrainenhanced");
            actionBackgroundTexture = Content.Load<Texture2D>("SpaceBackground");
            scoreFont = Content.Load<SpriteFont>("score");
            actionScene = new ActionScene(this, actionElementsTexture,
                actionBackgroundTexture, scoreFont);
            Components.Add(actionScene);

            // Create the Credits / Instruction Scene
            helpBackgroundTexture = Content.Load<Texture2D>("helpscreen");
            helpScene = new HelpScene(this, helpBackgroundTexture);
            Components.Add(helpScene);

            // Start the game in the start Scene :)
            startScene.Show();
            activeScene = startScene;
        }

        /// <summary>
        /// Open a new scene
        /// </summary>
        /// <param name="scene">Scene to be opened</param>
        protected void ShowScene(GameScene scene)
        {
            activeScene.Hide();
            activeScene = scene;
            scene.Show();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle Game Inputs
            HandleScenesInput();

            base.Update(gameTime);
        }

        /// <summary>
        /// Handle input of all game scenes
        /// </summary>
        private void HandleScenesInput()
        {
            // Handle Start Scene Input
            if (activeScene == startScene)
            {
                HandleStartSceneInput();
            }
            // Handle Help Scene input
            else if (activeScene == helpScene)
            {
                if (CheckActionButton())
                {
                    ShowScene(startScene);
                }
            }
            // Handle Action Scene Input
            else if (activeScene == actionScene)
            {
                HandleActionInput();
            }
        }

        /// <summary>
        /// Check if the Enter Key ou 'A' button was pressed
        /// </summary>
        /// <returns>true, if enter key ou 'A' button was pressed</returns>
        private void HandleActionInput()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            bool backKey = (oldGamePadState.Buttons.Back == ButtonState.Pressed) &&
                       (gamepadState.Buttons.Back == ButtonState.Released);

            bool enterKey = (oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                        (gamepadState.Buttons.A == ButtonState.Released);

            oldGamePadState = gamepadState;

            if (enterKey)
            {
                if (actionScene.GameOver)
                {
                    ShowScene(startScene);
                }
                else
                {
                    audio.MenuBack.Play();
                    actionScene.Paused = !actionScene.Paused;
                }
            }

            if (backKey)
            {
                ShowScene(startScene);
            }
        }

        /// <summary>
        /// Check if the Action Button was pressed
        /// </summary>
        /// <returns>true, if button was pressed</returns>
        private bool CheckActionButton()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            bool result = (oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                      (gamepadState.Buttons.A == ButtonState.Released);

            oldGamePadState = gamepadState;

            return result;
        }

        /// <summary>
        /// Handle buttons and keyboard in StartScene
        /// </summary>
        private void HandleStartSceneInput()
        {
            if (CheckActionButton())
            {
                audio.MenuSelect.Play();
                switch (startScene.SelectedMenuIndex)
                {
                    case 0:
                        ShowScene(actionScene);
                        break;
                    case 1:
                        ShowScene(helpScene);
                        break;
                    case 2:
                        Exit();
                        break;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Begin..
            spriteBatch.Begin();

            // Draw all Game Components..
            base.Draw(gameTime);

            // End.
            spriteBatch.End();
        }
    }
}
