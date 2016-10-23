using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockRainLive.Core;

namespace RockRainLive
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
        private readonly Random random;
        private int index;

        // Network Stuff
        private readonly NetworkHelper networkHelper;

        public Meteor(Game game, ref Texture2D theTexture)
            : 
            base(game, ref theTexture)
        {
            // Get the currente server state for a networked multiplayer game
            networkHelper = (NetworkHelper)
                Game.Services.GetService(typeof(NetworkHelper));
            
            // Create the sprite frames
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
        /// Initialize Meteor Position and Velocity
        /// </summary>
        public void PutinStartPosition()
        {
            // Only the server can set the meteor attributes
            if ((networkHelper.NetworkGameSession == null) ||
                (networkHelper.NetworkGameSession.IsHost))
            {
                position.X = random.Next(Game.Window.ClientBounds.Width -
                                         currentFrame.Width);
                position.Y = 0;
                YSpeed = 1 + random.Next(9);
                XSpeed = random.Next(3) - 1;
            }
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

            // Send the meteor info to the client
            if ((networkHelper.NetworkGameSession != null) &&
                (networkHelper.NetworkGameSession.IsHost))
            {
                networkHelper.ServerPacketWriter.Write('R');
                networkHelper.ServerPacketWriter.Write(index);
                networkHelper.ServerPacketWriter.Write(position);                
            }

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