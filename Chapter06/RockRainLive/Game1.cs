using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using RockRainLive.Core;

namespace RockRainLive
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
        protected Texture2D networkBackgroundTexture;

        // Game Scenes
        protected HelpScene helpScene;
        protected StartScene startScene;
        protected ActionScene actionScene;
        protected GameScene activeScene;
        protected NetworkScene networkScene;

        // Audio Library
        private AudioLibrary audio;

        // Fonts
        private SpriteFont smallFont, largeFont, scoreFont;

        // Used for handle input
        protected KeyboardState oldKeyboardState;
        protected GamePadState oldGamePadState;

        // Network stuff
        private readonly NetworkHelper networkHelper;
        private const int maxLocalPlayers = 1;
        private const int maxSessionPlayers = 2;

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
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            // We also get multisampling essentially for free on the 360, 
            // so turn it on
            graphics.PreferMultiSampling = true;
#endif  
            // Add Live Support
            Components.Add(new GamerServicesComponent(this));
            networkHelper = new NetworkHelper();
            Services.AddService(typeof(NetworkHelper), networkHelper);
        }        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
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

            // Create the Network Scene
            networkBackgroundTexture = Content.Load<Texture2D>("NetworkBackground");
            networkScene = new NetworkScene(this,smallFont,largeFont,
                                    networkBackgroundTexture);
            Components.Add(networkScene);

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
            if (!Guide.IsVisible)
            {
                HandleScenesInput();
            }

            // Handle the network session
            if (networkHelper.NetworkGameSession != null)
            {
                // Only send if we are not the server. There is no point sending 
                // packets to ourselves, because we already know what they will 
                // contain!
                if (!networkHelper.NetworkGameSession.IsHost)
                {
                    networkHelper.SendClientData();                    
                }
                else
                {
                    // If we are the server, transmit the game state
                    networkHelper.SendServerData();
                }

                // Pump the data
                networkHelper.NetworkGameSession.Update();

                // Read any incoming network packets.
                foreach (LocalNetworkGamer gamer in
                        networkHelper.NetworkGameSession.LocalGamers)
                {
                    // Keep reading as long as incoming packets are available.
                    while (gamer.IsDataAvailable)
                    {
                        NetworkGamer sender;
                        if (gamer.IsHost)
                        {
                            sender = networkHelper.ReadClientData(gamer);
                            if (!sender.IsLocal)
                            {
                                actionScene.HandleClientData();
                            }
                        }
                        else
                        {
                            sender = networkHelper.ReadServerData(gamer);
                            if (!sender.IsLocal)
                            {
                                actionScene.HandleServerData();
                            }
                        }
                    }
                }

            }

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
            else
            {
                // Handle Network scene input
                HandleNetworkSceneInput();
            }
        }

        /// <summary>
        /// Handle Newtwork Scene menu
        /// </summary>
        private void HandleNetworkSceneInput()
        {
            if (CheckEnterA())
            {
                audio.MenuSelect.Play();
                if (Gamer.SignedInGamers.Count == 0)
                {
                    HandleNotSigned();
                }
                else
                {
                    HandleSigned();
                }
            }            
        }

        /// <summary>
        /// Handle Newtwork Scene menu for an unsigned user
        /// </summary>
        private void HandleNotSigned()
        {
            switch (networkScene.SelectedMenuIndex)
            {
                case 0:
                    if (!Guide.IsVisible)
                    {
                        Guide.ShowSignIn(1, false);
                        break;
                    }
                    break;
                case 1:
                    ShowScene(startScene);
                    break;
            }
        }
        
        /// <summary>
        /// Handle Newtwork Scene menu for a signed user
        /// </summary>
        private void HandleSigned()
        {
            switch (networkScene.State)
            {
                case NetworkScene.NetworkGameState.idle:
                    switch (networkScene.SelectedMenuIndex)
                    {
                        case 0:
                            // Join a network game
                            JoinSession();
                            break;
                        case 1:
                            // Create a network game
                            CreateSession();
                            break;
                        case 2:
                            // Show the guide to change user
                            if (!Guide.IsVisible)
                            {
                                Guide.ShowSignIn(1, false);
                                break;
                            }
                            break;
                        case 3:
                            // Back to start scene
                            ShowScene(startScene);
                            break;
                    }
                    break;
                case NetworkScene.NetworkGameState.creating:
                    // Close the session created
                    CloseSession();
                    // Wait for a new command
                    networkScene.State = NetworkScene.NetworkGameState.idle;
                    networkScene.Message = "";
                    break;
            }
        }

        /// <summary>
        /// Create a session for a game server
        /// </summary>
        private void CreateSession()
        {
            networkHelper.NetworkGameSession = NetworkSession.Create(
                                                NetworkSessionType.SystemLink, 
                                                maxLocalPlayers, maxSessionPlayers);
            HookSessionEvents();
            networkScene.State = NetworkScene.NetworkGameState.creating;
            networkScene.Message = "Waiting another player...";
        }

        /// <summary>
        /// Quit the game session
        /// </summary>
        private void CloseSession()
        {
            networkHelper.NetworkGameSession.Dispose();
            networkHelper.NetworkGameSession = null;
        }

        /// <summary>
        /// After creating or joining a network session, we must subscribe to
        /// some events so we will be notified when the session changes state.
        /// </summary>
        void HookSessionEvents()
        {
            networkHelper.NetworkGameSession.GamerJoined += 
                GamerJoinedEventHandler;
            networkHelper.NetworkGameSession.SessionEnded += 
                SessionEndedEventHandler;
        }

        /// <summary>
        /// This event handler will be called whenever a new gamer joins the 
        /// session.
        /// </summary>
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            // Associate the ship with the joined player
            if (actionScene.Player1.Gamer == null)
            {
                actionScene.Player1.Gamer = e.Gamer;
            }
            else
            {
                actionScene.Player2.Gamer = e.Gamer;
            }

            if (networkHelper.NetworkGameSession.AllGamers.Count == 
                maxSessionPlayers)
            {
                actionScene.TwoPlayers = true;
                ShowScene(actionScene);
            }
        }

        /// <summary>
        /// Event handler notifies us when the network session has ended.
        /// </summary>
        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            networkScene.Message = e.EndReason.ToString();
            networkScene.State = NetworkScene.NetworkGameState.idle;

            CloseSession();

            if (activeScene != networkScene)
            {
                ShowScene(networkScene);
            }
        }

        /// <summary>
        /// Joins an existing network session.
        /// </summary>
        void JoinSession()
        {
            networkScene.Message = "Joining an game...";
            networkScene.State = NetworkScene.NetworkGameState.joining;

            try
            {
                // Search for sessions.
                using (AvailableNetworkSessionCollection availableSessions =
                            NetworkSession.Find(NetworkSessionType.SystemLink,
                                                maxLocalPlayers, null))
                {
                    if (availableSessions.Count == 0)
                    {
                        networkScene.Message = "No network sessions found.";
                        networkScene.State = NetworkScene.NetworkGameState.idle;
                        return;
                    }

                    // Join the first session we found.
                    networkHelper.NetworkGameSession = NetworkSession.Join(
                                                            availableSessions[0]);

                    HookSessionEvents();
                }
            }
            catch (Exception e)
            {
                networkScene.Message = e.Message;
                networkScene.State = NetworkScene.NetworkGameState.idle;
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
                    // Send the pause command to the other Player
                    if (networkHelper.NetworkGameSession != null)
                    {
                        // If we are the server, send using the server packets
                        if (networkHelper.NetworkGameSession.IsHost)
                        {
                            networkHelper.ServerPacketWriter.Write('P');
                            networkHelper.ServerPacketWriter.Write(
                                                                actionScene.Paused);
                        }
                        else
                        {
                            networkHelper.ClientPacketWriter.Write('P');
                            networkHelper.ClientPacketWriter.Write(
                                                                actionScene.Paused);
                        }
                    }
                    
                }
            }

            if (backKey)
            {
                if (networkHelper.NetworkGameSession != null)
                {
                    CloseSession();
                    networkScene.State = NetworkScene.NetworkGameState.idle;
                    networkScene.Message = "";
                    ShowScene(networkScene);
                }
                else
                {
                    ShowScene(startScene);    
                }                
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
                        ShowScene(networkScene);
                        break;
                    case 3:
                        ShowScene(helpScene);
                        break;
                    case 4:
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