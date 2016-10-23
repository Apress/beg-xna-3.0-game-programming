using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNA_TPS.GameBase.Cameras;
using XNA_TPS.GameBase.Materials;

namespace XNA_TPS.GameBase.Shapes
{
    public class SkyDome : DrawableGameComponent
    {
        Model model;
        Transformation transformation;

        TextureMaterial textureMaterial;

        // Necessary services
        CameraManager cameraManager;
        bool isInitialized;

        #region Properties
        public Transformation Transformation
        {
            get
            {
                return transformation;
            }
            set
            {
                transformation = value;
            }
        }

        public TextureMaterial TextureMaterial
        {
            get
            {
                return textureMaterial;
            }
            set
            {
                textureMaterial = value;
            }
        }

        #endregion

        public SkyDome(Game game)
            : base(game)
        {
            transformation = new Transformation();
            isInitialized = false;
        }

        public override void Initialize()
        {
            cameraManager = Game.Services.GetService(typeof(CameraManager)) as CameraManager;

            if (cameraManager == null)
                throw new InvalidOperationException();
            isInitialized = true;

            base.Initialize();
        }

        public void Load(string modelFileName)
        {
            if (!isInitialized)
                Initialize();

            model = Game.Content.Load<Model>(GameAssetsPath.MODELS_PATH + modelFileName);
        }

        private void SetEffectMaterial(BasicEffect basicEffect)
        {
            // Fix the skydome texture coordinate
            //GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            // Texture Material
            basicEffect.Texture = textureMaterial.Texture;
            basicEffect.TextureEnabled = true;

            // Transformation
            basicEffect.World = transformation.Matrix;
            basicEffect.View = cameraManager.ActiveCamera.View;
            basicEffect.Projection = cameraManager.ActiveCamera.Projection;
        }

        public override void Update(GameTime time)
        {
            BaseCamera camera = cameraManager.ActiveCamera;
            transformation.Translate = new Vector3(camera.Position.X, 0.0f, camera.Position.Z);
            transformation.Rotate += new Vector3(0, (float)time.ElapsedGameTime.TotalSeconds * 0.5f, 0);

            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            GraphicsDevice.RenderState.DepthBufferEnable = false;
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                // We are only rendering models with BasicEffect
                foreach (BasicEffect basicEffect in modelMesh.Effects)
                    SetEffectMaterial(basicEffect);
                
                modelMesh.Draw();
            }
            GraphicsDevice.RenderState.DepthBufferEnable = true;

            base.Draw(time);
        }
    }
}
