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


        //  Sprite objects
        clsSprite mySprite1;
        clsSprite mySprite2;

        // Audio objects
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue myLoopingSound = null;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //  changing the back buffer size changes the window size (when in windowed mode)
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 300;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            audioEngine = new AudioEngine(@"Content\MySounds.xgs");

            //  Assume the default names for the wave and sound bank.  
            //   To change these names, change properties in XACT
            waveBank = new WaveBank(audioEngine, @"Content\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Sound Bank.xsb");

            myLoopingSound = soundBank.GetCue("notify");
            myLoopingSound.Play();

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

            mySprite1 = new clsSprite(Content.Load<Texture2D>("ball"), new Vector2(0f, 0f), new Vector2(64f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            mySprite2 = new clsSprite(Content.Load<Texture2D>("ball"), new Vector2(218f, 118f), new Vector2(64f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //  set the speed the sprites will move
            mySprite1.velocity = new Vector2(5, 5);
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


            // Move the sprite 
            mySprite1.Move();

            //  Change the sprite 2 position using the left thumbstick 
            //Vector2 LeftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            //mySprite2.position += new Vector2(LeftThumb.X, -LeftThumb.Y) * 5;

            //  Change the sprite 2 position using the keyboard
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Up))
                mySprite2.position += new Vector2(0, -5);
            if (keyboardState.IsKeyDown(Keys.Down))
                mySprite2.position += new Vector2(0, 5);
            if (keyboardState.IsKeyDown(Keys.Left))
                mySprite2.position += new Vector2(-5, 0);
            if (keyboardState.IsKeyDown(Keys.Right))
                mySprite2.position += new Vector2(5, 0);

            //  Make sprite 2 follow the mouse 
            //if (mySprite2.position.X < Mouse.GetState().X)
            //    mySprite2.position += new Vector2(5, 0);
            //if (mySprite2.position.X > Mouse.GetState().X)
            //    mySprite2.position += new Vector2(-5, 0);
            //if (mySprite2.position.Y < Mouse.GetState().Y)
            //    mySprite2.position += new Vector2(0, 5);
            //if (mySprite2.position.Y > Mouse.GetState().Y)
            //    mySprite2.position += new Vector2(0, -5);

            if (mySprite1.Collides(mySprite2))
            {
                mySprite1.velocity *= -1;
                GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                soundBank.PlayCue("chord");
            }
            else
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

            // Play or stop an infinite looping sound when pressing the "B" button
            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
            {
                if (myLoopingSound.IsPaused)
                    myLoopingSound.Resume();
                else
                    myLoopingSound.Pause();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the sprites
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            mySprite1.Draw(spriteBatch);
            mySprite2.Draw(spriteBatch);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
