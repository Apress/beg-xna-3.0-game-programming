#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements the player ship.
    /// </summary>
    public class Player : DrawableGameComponent
    {
        protected Texture2D texture;
        protected Rectangle spriteRectangle;
        protected Vector2 position;
        protected TimeSpan elapsedTime = TimeSpan.Zero;
        protected PlayerIndex playerIndex;
        protected SpriteBatch sBatch;

        // Screen Area
        protected Rectangle screenBounds;

        // Game Stuff
        protected int score;
        protected int power;
        private const int INITIALPOWER = 100;

        public Player(Game game, ref Texture2D theTexture, PlayerIndex playerID, 
            Rectangle rectangle) : base(game)
        {
            texture = theTexture;
            position = new Vector2();
            playerIndex = playerID;
            // Get the current spritebatch
            sBatch = (SpriteBatch)
                Game.Services.GetService(typeof(SpriteBatch));

            // Create the source rectangle.
            // This represents where is the sprite picture in surface
            spriteRectangle = rectangle;

#if XBOX360
    // On the 360, we need take care about the tv "safe" area.
            screenBounds = new Rectangle((int)(Game.Window.ClientBounds.Width * 
                0.03f),(int)(Game.Window.ClientBounds.Height * 0.03f),
                Game.Window.ClientBounds.Width - 
                (int)(Game.Window.ClientBounds.Width * 0.03f),
                Game.Window.ClientBounds.Height - 
                (int)(Game.Window.ClientBounds.Height * 0.03f));
#else
            screenBounds = new Rectangle(0, 0, Game.Window.ClientBounds.Width, 
                Game.Window.ClientBounds.Height);
#endif
        }

        /// <summary>
        /// Put the ship in your start position in screen
        /// </summary>
        public void Reset()
        {
            if (playerIndex == PlayerIndex.One)
            {
                position.X = screenBounds.Width/3;
            }
            else
            {
                position.X = (int) (screenBounds.Width/1.5);
            }

            position.Y = screenBounds.Height - spriteRectangle.Height;
            score = 0;
            power = INITIALPOWER;
        }

        /// <summary>
        /// Total Points of the Player
        /// </summary>
        public int Score
        {
            get { return score; }
            set
            {
                if (value < 0)
                {
                    score = 0;
                }
                else
                {
                    score = value;
                }
            }
        }

        /// <summary>
        /// Remaining Power
        /// </summary>
        public int Power
        {
            get { return power; }
            set { power = value; }
        }

        /// <summary>
        /// Update the ship position, points and power 
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            HandleInput(playerIndex);
            UpdateShip(gameTime);

            base.Update(gameTime);
        }


        /// <summary>
        /// Get the ship position
        /// </summary>
        protected void HandleInput(PlayerIndex thePlayerIndex)
        {
            // Move the ship with xbox controller
            GamePadState gamepadstatus = GamePad.GetState(thePlayerIndex);
            position.Y += (int)((gamepadstatus.ThumbSticks.Left.Y * 3) * -2);
            position.X += (int)((gamepadstatus.ThumbSticks.Left.X * 3) * 2);

            if (thePlayerIndex == PlayerIndex.One)
            {
                HandlePlayer1KeyBoard();
            }
            else
            {
                HandlePlayer2KeyBoard();
            }
        }

        /// <summary>
        /// Update ship status
        /// </summary>
        private void UpdateShip(GameTime gameTime)
        {
            // Keep the player inside the screen
            KeepInBound();

            // Update score
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                score++;
                power--;
            }
        }

        /// <summary>
        /// Keep the ship inside the screen
        /// </summary>
        private void KeepInBound()
        {
            if (position.X < screenBounds.Left)
            {
                position.X = screenBounds.Left;
            }
            if (position.X > screenBounds.Width - spriteRectangle.Width)
            {
                position.X = screenBounds.Width - spriteRectangle.Width;
            }
            if (position.Y < screenBounds.Top)
            {
                position.Y = screenBounds.Top;
            }
            if (position.Y > screenBounds.Height - spriteRectangle.Height)
            {
                position.Y = screenBounds.Height - spriteRectangle.Height;
            }
        }

        /// <summary>
        /// Handle the keys for the player 1 (arrow keys)
        /// </summary>
        private void HandlePlayer1KeyBoard()
        {
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
        }

        /// <summary>
        /// Handle the keys for the player 2 (ASDW)
        /// </summary>
        private void HandlePlayer2KeyBoard()
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.W))
            {
                position.Y -= 3;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                position.Y += 3;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                position.X -= 3;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                position.X += 3;
            }
        }

        /// <summary>
        /// Draw the ship sprite
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Get the current spritebatch
            sBatch = (SpriteBatch) 
                Game.Services.GetService(typeof (SpriteBatch));

            // Draw the ship
            sBatch.Draw(texture, position, spriteRectangle, Color.White);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Get the bound rectangle of ship position in screen
        /// </summary>
        public Rectangle GetBounds()
        {
            return new Rectangle((int) position.X, (int) position.Y, 
                spriteRectangle.Width, spriteRectangle.Height);
        }
    }
}