#region Using Statements

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using RockRainLive.Core;

#endregion

namespace RockRainLive
{
    /// <summary>
    /// This game component implements a manager for all Meteors in the game.
    /// </summary>
    public class MeteorsManager : DrawableGameComponent
    {
        // List of active meteors
        private readonly List<Meteor> meteors;
        // Constant for initial meteor count
        private const int STARTMETEORCOUNT = 10;
        // Time for a new meteor
        private const int ADDMETEORTIME = 5000;

        protected Texture2D meteorTexture;
        protected TimeSpan elapsedTime = TimeSpan.Zero;
        protected AudioLibrary audio;
        
        // Network Stuff
        private NetworkHelper networkHelper;

        public MeteorsManager(Game game, ref Texture2D theTexture)
            : base(game)
        {
            meteorTexture = theTexture;
            meteors = new List<Meteor>();
        }

        /// <summary>
        /// All Meteors in the game
        /// </summary>
        public List<Meteor> AllMeteors
        {
            get { return meteors; }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to 
        /// before starting to run.  This is where it can query for any required
        /// services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Get the audio library
            audio = (AudioLibrary)
                Game.Services.GetService(typeof(AudioLibrary));

            // Get the currente server state for a networked multiplayer game
            networkHelper = (NetworkHelper)
                Game.Services.GetService(typeof(NetworkHelper));

            meteors.Clear();

            Start();

            for (int i = 0; i < meteors.Count; i++)
            {
                meteors[i].Initialize();
            }

            base.Initialize();
        }

        /// <summary>
        /// Start the Meteors Rain
        /// </summary>
        public void Start()
        {
            if ((networkHelper.NetworkGameSession == null) ||
                (networkHelper.NetworkGameSession.IsHost))
            {
                // Initialize a counter
                elapsedTime = TimeSpan.Zero;
                
                // Add the meteors
                for (int i = 0; i < STARTMETEORCOUNT; i++)
                {
                    AddNewMeteor();
                }
            }
        }

        /// <summary>
        /// Check if is a moment for a new meteor
        /// </summary>
        private void CheckforNewMeteor(GameTime gameTime)
        {
            // Add a rock each ADDMETEORTIME
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromMilliseconds(ADDMETEORTIME))
            {
                elapsedTime -= TimeSpan.FromMilliseconds(ADDMETEORTIME);

                AddNewMeteor();
                // Play a sound for a new meteor
                audio.NewMeteor.Play();
            }
        }

        /// <summary>
        /// Add a new meteor in the scene
        /// </summary>
        /// <returns>the new meteor</returns>
        private Meteor AddNewMeteor()
        {
            Meteor newMeteor = new Meteor(Game, ref meteorTexture);
            newMeteor.Initialize();
            meteors.Add(newMeteor);
            newMeteor.Index = meteors.Count-1;

            // Send the new meteor info to the client
            if ((networkHelper.NetworkGameSession != null) &&
                (networkHelper.NetworkGameSession.IsHost))
            {
                networkHelper.ServerPacketWriter.Write('M');
                networkHelper.ServerPacketWriter.Write(newMeteor.Index);
                networkHelper.ServerPacketWriter.Write(newMeteor.Position);
                networkHelper.ServerPacketWriter.Write(newMeteor.XSpeed);
                networkHelper.ServerPacketWriter.Write(newMeteor.YSpeed);
            }

            return newMeteor;
        }

         /// <summary>
         /// Update a meteor in the scene
         /// </summary>
         public void AddNewMeteor(int index, Vector2 thePosition, int XSpeed,
                                     int YSpeed)
         {
             Meteor meteor = AddNewMeteor();
             if (meteor.Index != index)
             {
                 throw new ApplicationException("rock field is not sync!");
             }
            meteor.Position = thePosition;
            meteor.XSpeed = XSpeed;
            meteor.YSpeed = YSpeed;
         }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Only the server can add new meteors
            if ((networkHelper.NetworkGameSession == null) || 
                (networkHelper.NetworkGameSession.IsHost))
            {
                CheckforNewMeteor(gameTime);
            }
            // Update Meteors
            for (int i = 0; i < meteors.Count; i++)
            {
                meteors[i].Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Check if the ship collide with a meteor
        /// <returns>true, if has a collision</returns>
        /// </summary>
        public bool CheckForCollisions(Rectangle rect)
        {
            for (int i = 0; i < meteors.Count; i++)
            {
                if (meteors[i].CheckCollision(rect))
                {
                    // BOM !!
                    audio.Explosion.Play();

                    // Put the meteor back to your initial position
                    meteors[i].PutinStartPosition();

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Allows the game component draw your content in game screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Draw the meteors
            for (int i = 0; i < meteors.Count; i++)
            {
                meteors[i].Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}