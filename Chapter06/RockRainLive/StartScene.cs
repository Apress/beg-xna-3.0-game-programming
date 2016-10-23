#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using RockRainLive.Core;

#endregion

namespace RockRainLive
{
    /// <summary>
    /// This is a game component that implements the Game Start Scene.
    /// </summary>
    public class StartScene : GameScene
    {
        // Misc
        protected TextMenuComponent menu;
        protected readonly Texture2D elements;
        // Audio
        protected AudioLibrary audio;
        // Spritebatch
        protected SpriteBatch spriteBatch = null;
        // Gui Stuff
        protected Rectangle rockRect = new Rectangle(0, 0, 536, 131);
        protected Vector2 rockPosition;
        protected Rectangle rainRect = new Rectangle(120, 165, 517, 130);
        protected Vector2 rainPosition;
        protected Rectangle enhancedRect = new Rectangle(8, 304, 375, 144);
        protected Vector2 enhancedPosition;
        protected bool showEnhanced;
        protected TimeSpan elapsedTime = TimeSpan.Zero;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="game">Main game object</param>
        /// <param name="smallFont">Font for the menu items</param>
        /// <param name="largeFont">Font for the menu selcted item</param>
        /// <param name="background">Texture for background image</param>
        /// <param name="elements">Texture with the foreground elements</param>
        public StartScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
                            Texture2D background,Texture2D elements)
            : base(game)
        {
            this.elements = elements;
            Components.Add(new ImageComponent(game, background, 
                                            ImageComponent.DrawMode.Center));

            // Create the Menu
            string[] items = {"One Player", "Two Players", "Network Game", 
                                "Help", "Quit"};
            menu = new TextMenuComponent(game, smallFont, largeFont);
            menu.SetMenuItems(items);
            Components.Add(menu);

            // Get the current spritebatch
            spriteBatch = (SpriteBatch) Game.Services.GetService(
                                            typeof (SpriteBatch));
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));
        }

        /// <summary>
        /// Show the start scene
        /// </summary>
        public override void Show()
        {
            audio.NewMeteor.Play();
            MediaPlayer.Play(audio.StartMusic);

            rockPosition.X = -1*rockRect.Width;
            rockPosition.Y = 40;
            rainPosition.X = Game.Window.ClientBounds.Width;
            rainPosition.Y = 180;
            // Put the menu centered in screen
            menu.Position = new Vector2((Game.Window.ClientBounds.Width - 
                                          menu.Width)/2, 330);

            // These elements will be visible when the 'Rock Rain' title
            // is done.
            menu.Visible = false;
            menu.Enabled = false;
            showEnhanced = false;

            base.Show();
        }

        /// <summary>
        /// Hide the start scene
        /// </summary>
        public override void Hide()
        {
            MediaPlayer.Stop();
            base.Hide();
        }

        /// <summary>
        /// Gets the selected menu option
        /// </summary>
        public int SelectedMenuIndex
        {
            get { return menu.SelectedIndex; }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!menu.Visible)
            {
                if (rainPosition.X >= (Game.Window.ClientBounds.Width - 595)/2)
                {
                    rainPosition.X -= 15;
                }

                if (rockPosition.X <= (Game.Window.ClientBounds.Width - 715)/2)
                {
                    rockPosition.X += 15;
                }
                else
                {
                    menu.Visible = true;
                    menu.Enabled = true;

                    MediaPlayer.Play(audio.StartMusic);
        #if XBOX360
                    enhancedPosition = new Vector2((rainPosition.X + 
                    rainRect.Width - enhancedRect.Width / 2), rainPosition.Y);
        #else
                    enhancedPosition =
                        new Vector2((rainPosition.X + rainRect.Width - 
                        enhancedRect.Width/2) - 80, rainPosition.Y);
        #endif
                    showEnhanced = true;
                }
            }
            else
            {
                elapsedTime += gameTime.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    showEnhanced = !showEnhanced;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Draw(elements, rockPosition, rockRect, Color.White);
            spriteBatch.Draw(elements, rainPosition, rainRect, Color.White);
            if (showEnhanced)
            {
                spriteBatch.Draw(elements, enhancedPosition, enhancedRect, 
                                 Color.White);
            }
        }
    }
}