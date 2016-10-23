#region Using Statements

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockRainLive.Core;

#endregion

namespace RockRainLive
{
    /// <summary>
    /// This is a game component that implements Power Source Element.
    /// </summary>
    public class PowerSource : Sprite
    {
        protected Texture2D texture;
        protected Random random;

        // Network stuff
        private readonly NetworkHelper networkHelper;

        public PowerSource(Game game, ref Texture2D theTexture)
            : base(game, ref theTexture)
        {
            // Get the currente server state for a networked multiplayer game
            networkHelper = (NetworkHelper)
                Game.Services.GetService(typeof(NetworkHelper));

            texture = theTexture;

            Frames = new List<Rectangle>();
            Rectangle frame = new Rectangle();
            frame.X = 291;
            frame.Y = 17;
            frame.Width = 14;
            frame.Height = 12;
            Frames.Add(frame);

            frame.Y = 30;
            Frames.Add(frame);

            frame.Y = 43;
            Frames.Add(frame);

            frame.Y = 57;
            Frames.Add(frame);

            frame.Y = 70;
            Frames.Add(frame);

            frame.Y = 82;
            Frames.Add(frame);

            frameDelay = 200;

            // Initialize the random number generator and put the power source in your
            // start position
            random = new Random(GetHashCode());
            PutinStartPosition();
        }

        /// <summary>
        /// Initialize Position and Velocity
        /// </summary>
        public void PutinStartPosition()
        {
            position.X = random.Next(Game.Window.ClientBounds.Width - 
                currentFrame.Width);
            position.Y = -10;
            Enabled = false;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if ((networkHelper.NetworkGameSession == null) || 
                (networkHelper.NetworkGameSession.IsHost))
            {
                // Check if the still visible
                if (position.Y >= Game.Window.ClientBounds.Height)
                {
                    PutinStartPosition();
                }

                // Move
                position.Y += 1;

                networkHelper.ServerPacketWriter.Write('L');
                networkHelper.ServerPacketWriter.Write(position);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Check if the object intersects with the specified rectangle
        /// </summary>
        /// <param name="rect">test rectangle</param>
        /// <returns>true, if has a collision</returns>
        public bool CheckCollision(Rectangle rect)
        {
            Rectangle spriterect =
                new Rectangle((int) position.X, (int) position.Y, 
                currentFrame.Width, currentFrame.Height);
            return spriterect.Intersects(rect);
        }
    }
}