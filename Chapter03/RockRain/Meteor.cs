#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace RockRain
{
    /// <summary>
    /// This is a game component that implements the rocks that player must avoid.
    /// </summary>
    public class Meteor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected Texture2D texture;
        protected Rectangle spriteRectangle;
        protected Vector2 position;
        protected int Yspeed;
        protected int Xspeed;
        protected Random random;
        protected SpriteBatch sBatch;

        // Width and Heigh of sprite in texture
        protected const int METEORWIDTH = 45;
        protected const int METEORHEIGHT = 45;

        public Meteor(Game game, ref Texture2D theTexture)
            : base(game)
        {
            texture = theTexture;            
            position = new Vector2();
            // Get the current spritebatch
            sBatch =
                (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // Create the source rectangle.
            // This represents where is the sprite picture in surface
            spriteRectangle = new Rectangle(20, 16, METEORWIDTH, METEORHEIGHT);

            // Initialize the random number generator and put the meteor in 
            // your start position
            random = new Random(this.GetHashCode());
            PutinStartPosition();
        }

        /// <summary>
        /// Initialize Meteor Position and Velocity
        /// </summary>
        protected void PutinStartPosition()
        {
            position.X = random.Next(Game.Window.ClientBounds.Width - METEORWIDTH);
            position.Y = 0;
            Yspeed = 1 + random.Next(9);
            Xspeed = random.Next(3) - 1;
        }

        /// <summary>
        /// Allows the game component draw your content in game screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {            
            // Draw the meteor
            sBatch.Draw(texture, position, spriteRectangle, Color.White);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check if the meteor still visible
            if ((position.Y >= Game.Window.ClientBounds.Height) || 
                (position.X >= Game.Window.ClientBounds.Width) || (position.X <= 0))
            {
                PutinStartPosition();
            }

            // Move meteor
            position.Y += Yspeed;
            position.X += Xspeed;
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Check if the meteor intersects with the specified rectangle
        /// </summary>
        /// <param name="rect">test rectangle</param>
        /// <returns>true, if has a collision</returns>
        public bool CheckCollision(Rectangle rect)
        {
            Rectangle spriterect = new Rectangle((int)position.X, (int)position.Y,
                METEORWIDTH, METEORHEIGHT);
            return spriterect.Intersects(rect);
        }
    }
}