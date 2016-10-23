using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNADemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        //  3D objects
        cls3DAxis my3DAxis;
        Model myModel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            my3DAxis = new cls3DAxis(graphics.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //  Create the 3D axis
            my3DAxis.LoadContent();
            //  Load the model
            myModel = Content.Load<Model>("Cube");

            //  calculate the aspect ratio for the model
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;

            //  Configure basic lighting and do a simple rotation for the model;
            //    (so it can be seen on screen)
            foreach (ModelMesh mesh in myModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //  Make the model a little smaller for aestetical reasons  ;)
                    effect.World = Matrix.CreateScale(0.5f) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(45.0f)) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(45.0f));
                    //  Set the projection Matrix forthe model
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45.0f),
                               aspectRatio, 1.0f, 10.0f);
                    effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 3.0f), Vector3.Zero,
                                Vector3.Up);
                    effect.EnableDefaultLighting();
                }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //   Free the resources allocated for 3D drawing
            my3DAxis.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //  Move the 3D axis
            my3DAxis.worldMatrix *= Matrix.CreateRotationY(0.01f) *
                                   Matrix.CreateRotationX(0.01f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            //  Draw the 3D axis
            my3DAxis.Draw();

            //  Loop through each mesh of the model
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // Draw the current mesh
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
