using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNADemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        NetworkHelper networkHelper;
        SpriteFont Arial;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            networkHelper = new NetworkHelper();

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
            Arial = Content.Load<SpriteFont>("Arial");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //  Presents the LIVE Guide
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                networkHelper.SignInGamer();

            //  Creates a Session
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
                networkHelper.CreateSession();

            //  Find a session synchronously
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
                networkHelper.FindSession();		

            //  Find a session asynchronously
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
                networkHelper.AsyncFindSession();

            // Set the local players to Ready state (enter the game Lobby)
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
                networkHelper.SetPlayerReady();

                   if (networkHelper.SessionState == NetworkSessionState.Playing)
                   {
                     //    Send any key pressed to the remote player
                       foreach (Keys key in Keyboard.GetState().GetPressedKeys())
                           networkHelper.SendMessage(key.ToString());

                    //     Receive the keys from the remote player
                       networkHelper.ReceiveMessage();
                   }
                    networkHelper.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            //  Show the current session state
            spriteBatch.Begin();
            spriteBatch.DrawString(Arial, "Game State: " + "Waiting for user command...", new Vector2(20, 20), Color.Yellow);
            spriteBatch.DrawString(Arial, "Press:", new Vector2(20, 100), Color.Snow);
            spriteBatch.DrawString(Arial, " - F1 to sign in", new Vector2(20, 120), Color.Snow);
            spriteBatch.DrawString(Arial, " - F2 to create a session", new Vector2(20, 140), Color.Snow);
            spriteBatch.DrawString(Arial, " - F3 to find a session", new Vector2(20, 160), Color.Snow);
            spriteBatch.DrawString(Arial, " - F4 to asynchronously find a session", new Vector2(20, 180), Color.Snow);
            spriteBatch.DrawString(Arial, "After the game starts, press other keys to send messages", new Vector2(20, 220), Color.Snow);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
