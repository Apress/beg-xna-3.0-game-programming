#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace XNADemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        //  Sprite objects
        clsSprite mySprite1;
        clsSprite mySprite2;

        //  SpriteBatch which will draw (render) the sprite
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //  changing the back buffer size changes the window size (when in windowed mode)
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 300;

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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load a 2D texture sprite
            mySprite1 = new clsSprite(Content.Load<Texture2D>("ball"), new Vector2(0f, 0f), new Vector2(64f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            mySprite2 = new clsSprite(Content.Load<Texture2D>("ball"), new Vector2(218f, 118f), new Vector2(64f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            // Create a SpriteBatch to render the sprite 
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            //  set the speed the sprites will move
            mySprite1.velocity = new Vector2(5, 5);
            mySprite2.velocity = new Vector2(3, -3);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //  Free the previously alocated resources
            mySprite1.texture.Dispose();
            mySprite2.texture.Dispose();
            spriteBatch.Dispose();
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

            // Move the sprites 
            mySprite1.Move();
            mySprite2.Move();

            if (mySprite1.CircleCollides(mySprite2))
            {
                Vector2 tempVelocity = mySprite1.velocity;
                mySprite1.velocity = mySprite2.velocity;
                mySprite2.velocity = tempVelocity;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            mySprite1.Draw(spriteBatch);
            mySprite2.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
