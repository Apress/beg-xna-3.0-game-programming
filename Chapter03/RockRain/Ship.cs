#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace RockRain
{
    /// <summary>
    /// This is a game component that implements the player ship.
    /// </summary>
    public class Ship : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected Texture2D texture;
        protected Rectangle spriteRectangle;
        protected Vector2 position;
        protected SpriteBatch sBatch;

        // Width and Heigh of sprite in texture
        protected const int SHIPWIDTH = 30;
        protected const int SHIPHEIGHT = 30;

        // Screen Area
        protected Rectangle screenBounds;

        public Ship(Game game, ref Texture2D theTexture)
            : base(game)
        {
            texture = theTexture;
            position = new Vector2();
            // Get the current spritebatch
            sBatch =
                (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // Create the source rectangle.
            // This represents where is the sprite picture in surface
            spriteRectangle = new Rectangle(31, 83, SHIPWIDTH, SHIPHEIGHT);

#if XBOX360
            // On the 360, we need take care about the tv "safe" area.
            screenBounds = new Rectangle(
                                (int)(Game.Window.ClientBounds.Width * 0.03f),
                                (int)(Game.Window.ClientBounds.Height * 0.03f),
                                Game.Window.ClientBounds.Width - 
                                (int)(Game.Window.ClientBounds.Width * 0.03f),
                                Game.Window.ClientBounds.Height - 
                                (int)(Game.Window.ClientBounds.Height * 0.03f));            
#else
            screenBounds = new Rectangle(0,0,
                Game.Window.ClientBounds.Width,
                Game.Window.ClientBounds.Height);
#endif
        }

        /// <summary>
        /// Put the ship in your start position in screen
        /// </summary>
        public void PutinStartPosition()
        {
            position.X = screenBounds.Width / 2;
            position.Y = screenBounds.Height - SHIPHEIGHT;
        }

        /// <summary>
        /// Update the ship position 
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Move the ship with xbox controller
            GamePadState gamepadstatus = GamePad.GetState(PlayerIndex.One);
            position.Y += (int)((gamepadstatus.ThumbSticks.Left.Y * 3) * -2);
            position.X += (int)((gamepadstatus.ThumbSticks.Left.X * 3) * 2);

            // Move the ship with keyboard
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Up))
            {
                position.Y -= 3;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                position.Y += 3;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                position.X -= 3;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                position.X += 3;
            }

            // Keep the ship inside the screen
            if (position.X < screenBounds.Left)
            {
                position.X = screenBounds.Left;
            }
            if (position.X > screenBounds.Width - SHIPWIDTH)
            {
                position.X = screenBounds.Width - SHIPWIDTH;
            }
            if (position.Y < screenBounds.Top)
            {
                position.Y = screenBounds.Top;
            }
            if (position.Y > screenBounds.Height - SHIPHEIGHT)
            {
                position.Y = screenBounds.Height - SHIPHEIGHT;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the ship sprite
        /// </summary>
        public override void Draw(GameTime gameTime)
        {            
            // Draw the ship
            sBatch.Draw(texture, position, spriteRectangle, Color.White);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Get the bound rectangle of ship position in screen
        /// </summary>
        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, 
                SHIPWIDTH, SHIPHEIGHT);
        }
    }
}