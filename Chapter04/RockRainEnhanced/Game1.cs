using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RockRainEnhanced.Core;

namespace RockRainEnhanced
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Textures
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;
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
        protected KeyboardState oldKeyboardState;
        protected GamePadState oldGamePadState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Used for input handling
            oldKeyboardState = Keyboard.GetState();
            oldGamePadState = GamePad.GetState(PlayerIndex.One);

#if XBOX360
    // On the 360, we are always fullscreen and we always render to the user's 
    // prefered resolution
            graphics.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = this.Window.ClientBounds.Height;
            // We also get multisampling essentially for free on the 360, 
            // so turn it on
            graphics.PreferMultiSampling = true;
#endif
        }        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof (SpriteBatch), spriteBatch);

            // Load Audio Elements
            audio = new AudioLibrary();
            audio.LoadContent(Content);
            Services.AddService(typeof(AudioLibrary), audio);

            // Create the Credits / Instruction Scene
            helpBackgroundTexture = Content.Load<Texture2D>("helpbackground");
            helpForegroundTexture = Content.Load<Texture2D>("helpForeground");
            helpScene = new HelpScene(this, helpBackgroundTexture, 
                    helpForegroundTexture);
            Components.Add(helpScene);

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
                if (CheckEnterA())
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
        private bool CheckEnterA()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool result = (oldKeyboardState.IsKeyDown(Keys.Enter) && 
                (keyboardState.IsKeyUp(Keys.Enter)));
            result |= (oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                      (gamepadState.Buttons.A == ButtonState.Released);

            oldKeyboardState = keyboardState;
            oldGamePadState = gamepadState;

            return result;
        }

        /// <summary>
        /// Check if the Enter Key ou 'A' button was pressed
        /// </summary>
        /// <returns>true, if enter key ou 'A' button was pressed</returns>
        private void HandleActionInput()
        {
            // Get the Keyboard and GamePad state
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool backKey = (oldKeyboardState.IsKeyDown(Keys.Escape) && 
                (keyboardState.IsKeyUp(Keys.Escape)));
            backKey |= (oldGamePadState.Buttons.Back == ButtonState.Pressed) &&
                       (gamepadState.Buttons.Back == ButtonState.Released);

            bool enterKey = (oldKeyboardState.IsKeyDown(Keys.Enter) && 
                (keyboardState.IsKeyUp(Keys.Enter)));
            enterKey |= (oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                        (gamepadState.Buttons.A == ButtonState.Released);

            oldKeyboardState = keyboardState;
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
        /// Handle buttons and keyboard in StartScene
        /// </summary>
        private void HandleStartSceneInput()
        {
            if (CheckEnterA())
            {
                audio.MenuSelect.Play();                
                switch (startScene.SelectedMenuIndex)
                {
                    case 0:
                        actionScene.TwoPlayers = false;
                        ShowScene(actionScene);
                        break;
                    case 1:
                        actionScene.TwoPlayers = true;
                        ShowScene(actionScene);
                        break;
                    case 2:
                        ShowScene(helpScene);
                        break;
                    case 3:
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