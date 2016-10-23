using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockRainEnhanced.Core;

namespace RockRainEnhanced
{
    /// <summary>
    /// This class is the Animated Sprite for a Meteor
    /// </summary>
    public class Meteor : Sprite
    {
        // Vertical velocity
        protected int Yspeed;
        // Horizontal velocity
        protected int Xspeed;
        protected Random random;
        // Unique id for this meteor
        private int index;

        public Meteor(Game game, ref Texture2D theTexture) : 
            base(game, ref theTexture)
        {
            Frames = new List<Rectangle>();
            Rectangle frame = new Rectangle();
            frame.X = 468;
            frame.Y = 0;
            frame.Width = 49;
            frame.Height = 44;
            Frames.Add(frame);

            frame.Y = 50;
            Frames.Add(frame);

            frame.Y = 98;
            frame.Height = 45;
            Frames.Add(frame);

            frame.Y = 146;
            frame.Height = 49;
            Frames.Add(frame);

            frame.Y = 200;
            frame.Height = 44;
            Frames.Add(frame);

            frame.Y = 250;
            Frames.Add(frame);

            frame.Y = 299;
            Frames.Add(frame);

            frame.Y = 350;
            frame.Height = 49;
            Frames.Add(frame);

            // Initialize the random number generator and put the meteor in your
            // start position
            random = new Random(GetHashCode());
            PutinStartPosition();
        }

        /// <summary>
        /// Initialize Meteor Position and Velocity
        /// </summary>
        public void PutinStartPosition()
        {
            position.X = random.Next(Game.Window.ClientBounds.Width - 
                currentFrame.Width);
            position.Y = 0;
            YSpeed = 1 + random.Next(9);
            XSpeed = random.Next(3) - 1;
        }

        /// Vertical velocity
        /// </summary>
        public int YSpeed
        {
            get { return Yspeed; }
            set
            {
                Yspeed = value;
                frameDelay = 200 - (Yspeed * 5);
            }
        }

        /// <summary>
        /// Horizontal Velocity
        /// </summary>
        public int XSpeed
        {
            get { return Xspeed; }
            set { Xspeed = value; }
        }

        /// <summary>
        /// Meteor Identifier
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Update the Meteor Position
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Check if the meteor still visible
            if ((position.Y >= Game.Window.ClientBounds.Height) || 
                (position.X >= Game.Window.ClientBounds.Width) ||
                (position.X <= 0))
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
            Rectangle spriterect =new Rectangle((int) position.X, (int) position.Y, 
                currentFrame.Width, currentFrame.Height);
            return spriterect.Intersects(rect);
        }
    }
}