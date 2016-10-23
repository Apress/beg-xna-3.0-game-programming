using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using RockRainLive.Core;

namespace RockRainLive
{
    public class NetworkScene : GameScene
    {
        // Scene State
        public enum NetworkGameState
        {
            idle = 1,
            joining = 2,
            creating = 3
        }

        // Misc
        protected TextMenuComponent menu;
        private readonly SpriteFont messageFont;
        private Vector2 messagePosition,messageShadowPosition;
        private string message;
        protected TimeSpan elapsedTime = TimeSpan.Zero;

        // Spritebatch
        protected SpriteBatch spriteBatch = null;

        // Scene state
        private NetworkGameState state;
        // Used for message blink
        private bool showMessage = true;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="game">Main game object</param>
        /// <param name="smallFont">Font for the menu items</param>
        /// <param name="largeFont">Font for the menu selcted item</param>
        /// <param name="background">Texture for background image</param>
        public NetworkScene(Game game, SpriteFont smallFont, SpriteFont largeFont,
                            Texture2D background) : base(game)
        {
            messageFont = largeFont;
            Components.Add(new ImageComponent(game, background,
                                            ImageComponent.DrawMode.Stretch));

            // Create the menu component
            menu = new TextMenuComponent(game, smallFont, largeFont);
            Components.Add(menu);

            // Get the current spritebatch
            spriteBatch = (SpriteBatch)Game.Services.GetService(
                                            typeof(SpriteBatch));
        }

        /// <summary>
        /// Gets the selected menu option
        /// </summary>
        public int SelectedMenuIndex
        {
            get { return menu.SelectedIndex; }
        }

        /// <summary>
        /// Scene state
        /// </summary>
        public NetworkGameState State
        {
            get { return state; }
            set
            {
                state = value;
                menu.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Text of the message line
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                // Calculate the message position
                messagePosition = new Vector2();
                messagePosition.X = (Game.Window.ClientBounds.Width -
                    messageFont.MeasureString(message).X)/2;
                messagePosition.Y = 130;

                // Calculate the message shadow position
                messageShadowPosition = messagePosition;
                messageShadowPosition.Y++;
                messageShadowPosition.X--;

            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                showMessage = !showMessage;
            }

            // Set the menu for the current state
            UpdateMenus();

            base.Update(gameTime);
        }

        /// <summary>
        /// Show Scene
        /// </summary>
        public override void Show()
        {
            state = NetworkGameState.idle;

            base.Show();
        }

        /// <summary>
        /// Allows the game component draw your content in game screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!string.IsNullOrEmpty(message) && showMessage)
            {
                DrawMessage();
            }
        }

        /// <summary>
        /// Helper draws notification messages before calling blocking network methods.
        /// </summary>
        void DrawMessage()
        {
            // Draw the shadow
            spriteBatch.DrawString(messageFont, message, messageShadowPosition, 
                Color.Black);
         
            // Draw the message
            spriteBatch.DrawString(messageFont, message, messagePosition, 
                Color.DarkOrange);
        }

        /// <summary>
        /// Build a menu for each scene state and network status
        /// </summary>
        private void UpdateMenus()
        {
            if (Gamer.SignedInGamers.Count == 0)
            {
                string[] items = {"Sign in", "Back"};
                menu.SetMenuItems(items);
            }
            else
            {                
                if (state == NetworkGameState.idle)
                {
                    string[] items = {"Join a System Link Game", 
                        "Create a System Link Game", "Sign out", "Back"};
                    menu.SetMenuItems(items);
                }
                if (state == NetworkGameState.creating)
                {
                    string[] items = { "Cancel"};
                    menu.SetMenuItems(items);
                }
            }

            // Put the menu centered in screen
            menu.Position = new Vector2((Game.Window.ClientBounds.Width -
                                          menu.Width) / 2, 330);
        }
    }
}
