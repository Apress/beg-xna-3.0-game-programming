using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using RockRainEnhanced.Core;

namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements the Action Scene.
    /// </summary>
    public class ActionScene : GameScene
    {
        // Basics
        protected Texture2D actionTexture;
        private AudioLibrary audio;
        protected SpriteBatch spriteBatch = null;

        // Game Elements
        protected Player player1;
        protected Player player2;
        protected MeteorsManager meteors;
        protected PowerSource powerSource;
        protected SimpleRumblePad rumblePad;
        protected ImageComponent background;
        protected Score scorePlayer1;
        protected Score scorePlayer2;

        // Gui Stuff
        protected Vector2 pausePosition;
        protected Vector2 gameoverPosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Rectangle gameoverRect = new Rectangle(1, 170, 350, 48);

        // GameState elements
        protected bool paused;
        protected bool gameOver;
        protected TimeSpan elapsedTime = TimeSpan.Zero;
        protected bool twoPlayers;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="theTexture">Texture with the sprite elements</param>
        /// <param name="backgroundTexture">Texture for the background</param>
        /// <param name="font">Font used in the score</param>
        public ActionScene(Game game, Texture2D theTexture,
            Texture2D backgroundTexture, SpriteFont font)
            : base(game)
        {
            background = new ImageComponent(game, backgroundTexture, 
                ImageComponent.DrawMode.Stretch);
            Components.Add(background);

            actionTexture = theTexture;

            // Get the current sprite batch
            spriteBatch = (SpriteBatch) 
                Game.Services.GetService(typeof (SpriteBatch));

            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            meteors = new MeteorsManager(Game, ref actionTexture);
            Components.Add(meteors);

            player1 = new Player(Game, ref actionTexture, PlayerIndex.One, 
                new Rectangle(323, 15, 30, 30));
            player1.Initialize();
            Components.Add(player1);

            player2 = new Player(Game, ref actionTexture, PlayerIndex.Two, 
                new Rectangle(360, 17, 30, 30));
            player2.Initialize();
            Components.Add(player2);

            scorePlayer1 = new Score(game, font, Color.Blue);
            scorePlayer1.Position = new Vector2(10, 10);
            Components.Add(scorePlayer1);
            scorePlayer2 = new Score(game, font, Color.Red);
            scorePlayer2.Position = new Vector2(
                Game.Window.ClientBounds.Width - 200, 10);
            Components.Add(scorePlayer2);

            rumblePad = new SimpleRumblePad(game);
            Components.Add(rumblePad);

            powerSource = new PowerSource(game, ref actionTexture);
            powerSource.Initialize();
            Components.Add(powerSource);
        }

        /// <summary>
        /// Show the action scene
        /// </summary>
        public override void Show()
        {
            MediaPlayer.Play(audio.BackMusic);

            meteors.Initialize();
            powerSource.PutinStartPosition();

            player1.Reset();
            player2.Reset();

            paused = false;
            pausePosition.X = (Game.Window.ClientBounds.Width - 
                pauseRect.Width)/2;
            pausePosition.Y = (Game.Window.ClientBounds.Height - 
                pauseRect.Height)/2;

            gameOver = false;
            gameoverPosition.X = (Game.Window.ClientBounds.Width - 
                gameoverRect.Width)/2;
            gameoverPosition.Y = (Game.Window.ClientBounds.Height - 
                gameoverRect.Height)/2;

            // Is a two-player game?
            player1.Visible = true;
            player2.Visible = twoPlayers;
            player2.Enabled = twoPlayers;
            scorePlayer2.Visible = twoPlayers;
            scorePlayer2.Enabled = twoPlayers;

            base.Show();
        }

        /// <summary>
        /// Hide the scene
        /// </summary>
        public override void Hide()
        {
            // Stop the background music
            MediaPlayer.Stop();
            // Stop the rumble
            rumblePad.Stop(PlayerIndex.One);
            rumblePad.Stop(PlayerIndex.Two);

            base.Hide();
        }

        /// <summary>
        /// Indicate the 2-players game mode
        /// </summary>
        public bool TwoPlayers
        {
            get { return twoPlayers; }
            set { twoPlayers = value; }
        }

        /// <summary>
        /// True, if the game is in GameOver state
        /// </summary>
        public bool GameOver
        {
            get { return gameOver; }
        }

        /// <summary>
        /// Paused mode
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (paused)
                {
                    MediaPlayer.Pause();
                }
                else
                {
                    MediaPlayer.Resume();
                }
            }
        }

        /// <summary>
        /// Handle collisions with a meteor
        /// </summary>
        private void HandleDamages()
        {
            // Check Collision for player 1
            if (meteors.CheckForCollisions(player1.GetBounds()))
            {
                // Shake!
                rumblePad.RumblePad(PlayerIndex.One, 500, 1.0f, 1.0f);
                // Player penalty
                player1.Power -= 10;
                player1.Score -= 10;
            }

            // Check Collision for player 2
            if (twoPlayers)
            {
                if (meteors.CheckForCollisions(player2.GetBounds()))
                {
                    // Shake!
                    rumblePad.RumblePad(PlayerIndex.Two, 500, 1.0f, 1.0f);
                    // Player penalty
                    player2.Power -= 10;
                    player2.Score -= 10;
                }

                // Check for collision between the players
                if (player1.GetBounds().Intersects(player2.GetBounds()))
                {
                    rumblePad.RumblePad(PlayerIndex.One, 500, 1.0f, 1.0f);
                    player1.Power -= 10;
                    player1.Score -= 10;
                    rumblePad.RumblePad(PlayerIndex.Two, 500, 1.0f, 1.0f);
                    player2.Power -= 10;
                    player2.Score -= 10;
                }
            }
        }

        /// <summary>
        /// Handle power-up stuff
        /// </summary>
        private void HandlePowerSourceSprite(GameTime gameTime)
        {
            // Player 1 get the power source
            if (powerSource.CheckCollision(player1.GetBounds()))
            {
                audio.PowerGet.Play();
                elapsedTime = TimeSpan.Zero;
                powerSource.PutinStartPosition();
                player1.Power += 50;
            }

            if (twoPlayers)
            {
                // Player 2 get the power source
                if (powerSource.CheckCollision(player2.GetBounds()))
                {
                    audio.PowerGet.Play();
                    elapsedTime = TimeSpan.Zero;
                    powerSource.PutinStartPosition();
                    player2.Power += 50;
                }
            }

            // Check for send a new Power source
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(15))
            {
                elapsedTime -= TimeSpan.FromSeconds(15);
                powerSource.Enabled = true;
                audio.PowerShow.Play();
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if ((!paused) && (!gameOver))
            {
                // Check collisions with meteors
                HandleDamages();

                // Check if a player get a power boost
                HandlePowerSourceSprite(gameTime);

                // Update score
                scorePlayer1.Value = player1.Score;
                scorePlayer1.Power = player1.Power;
                if (twoPlayers)
                {
                    scorePlayer2.Value = player2.Score;
                    scorePlayer2.Power = player2.Power;
                }

                // Check if player is dead
                gameOver = ((player1.Power <= 0) || (player2.Power <= 0));
                if (gameOver)
                {
                    player1.Visible = (player1.Power > 0);
                    player2.Visible = (player2.Power > 0) && twoPlayers;
                    // Stop the music
                    MediaPlayer.Stop();
                    // Stop rumble
                    rumblePad.Stop(PlayerIndex.One);
                    rumblePad.Stop(PlayerIndex.Two);
                }

                // Update all other game components
                base.Update(gameTime);
            }

            // In game over state, keep the meteors animation
            if (gameOver)
            {
                meteors.Update(gameTime);
            }
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // Draw all game components
            base.Draw(gameTime);

            if (paused)
            {
                // Draw the "pause" text
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect, 
                    Color.White);
            }
            if (gameOver)
            {
                // Draw the "gameover" text
                spriteBatch.Draw(actionTexture, gameoverPosition, gameoverRect, 
                    Color.White);
            }
}
    }
}