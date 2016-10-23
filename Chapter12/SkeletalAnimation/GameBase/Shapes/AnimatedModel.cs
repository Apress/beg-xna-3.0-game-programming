using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkeletalAnimation.GameBase.Cameras;
using SkeletalAnimation.GameBase.Effects;
using SkeletalAnimation.GameBase.Lights;
using SkeletalAnimation.GameBase.Materials;

using AnimatedModelContent;

namespace SkeletalAnimation.GameBase.Shapes
{
    public class AnimatedModel : DrawableGameComponent
    {
        Model model;
        AnimatedModelData animatedModelData;
        Transformation transformation;

        AnimatedModelEffect animatedModelEffect;
        LightMaterial lightMaterial;

        bool enableAnimationLoop;
        float animationSpeed;
        int activeAnimationKeyframe;
        TimeSpan activeAnimationTime;
        AnimationData activeAnimation;

        Matrix[] bones;
        Matrix[] bonesAbsolute;
        Matrix[] bonesAnimation;
        Matrix[] bonesTransform;

        BoundingBox modelBoundingBox;
        BoundingSphere modelBoundingSphere;

        // Necessary services
        CameraManager cameraManager;
        LightManager lightManager;
        bool isInitialized;

        #region Properties
        public LightMaterial LightMaterial
        {
            get
            {
                return lightMaterial;
            }
            set
            {
                lightMaterial = value;
            }
        }

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

        public TimeSpan ActiveAnimationTime
        {
            get
            {
                return activeAnimationTime;
            }
            set
            {
                activeAnimationTime = value;
            }
        }

        public AnimationData ActiveAnimation
        {
            get
            {
                return activeAnimation;
            }
            set
            {
                activeAnimation = value;

                // Reset animation
                ResetAnimation();
            }
        }

        public bool IsAnimationFinished
        {
            get
            {
                return (activeAnimationTime >= activeAnimation.Duration);
            }
        }

        public AnimationData[] Animations
        {
            get
            {
                return animatedModelData.Animations;
            }
        }

        public bool EnableAnimationLoop
        {
            get
            {
                return enableAnimationLoop;
            }
            set
            {
                enableAnimationLoop = value;
            }
        }

        public float AnimationSpeed
        {
            get
            {
                return animationSpeed;
            }
            set
            {
                animationSpeed = value;
            }
        }

        public Matrix[] BonesTransform
        {
            get
            {
                return bonesTransform;
            }
            set
            {
                bonesTransform = value;
            }
        }

        public Matrix[] BonesAbsolute
        {
            get
            {
                return bonesAbsolute;
            }
            set
            {
                bonesAbsolute = value;
            }
        }

        public Matrix[] BonesAnimation
        {
            get
            {
                return bonesAnimation;
            }
            set
            {
                bonesAnimation = value;
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return modelBoundingBox;
            }
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                return modelBoundingSphere;
            }
        }
        #endregion

        public AnimatedModel(Game game)
            : base(game)
        {
            // Animation
            enableAnimationLoop = true;
            animationSpeed = 1.0f;
            activeAnimationKeyframe = 0;
            activeAnimationTime = TimeSpan.Zero;

            transformation = new Transformation();

            isInitialized = false;
        }

        public override void Initialize()
        {
            cameraManager = Game.Services.GetService(typeof(CameraManager)) as CameraManager;
            lightManager = Game.Services.GetService(typeof(LightManager)) as LightManager;

            if (cameraManager == null || lightManager == null)
                throw new InvalidOperationException();
            isInitialized = true;

            base.Initialize();
        }

        public void Load(string modelFileName)
        {
            if (!isInitialized)
                Initialize();

            model = Game.Content.Load<Model>(GameAssetsPath.MODELS_PATH + modelFileName);

            // Get animated model data
            Dictionary<string, object> modelTag = (Dictionary<string, object>)model.Tag;
            if (modelTag == null)
                throw new InvalidOperationException("This is not a valid animated model.");

            // Read tag data
            animatedModelData = (AnimatedModelData)modelTag["AnimatedModelData"];
            modelBoundingBox = (BoundingBox)modelTag["ModelBoudingBox"];
            modelBoundingSphere = (BoundingSphere)modelTag["ModelBoudingSphere"];

            // Animation
            animationSpeed = 1.0f;
            activeAnimationKeyframe = 0;
            activeAnimationTime = TimeSpan.Zero;

            if (animatedModelData.Animations.Length > 0)
                activeAnimation = animatedModelData.Animations[0];

            bones = new Matrix[animatedModelData.BonesBindPose.Length];
            bonesAbsolute = new Matrix[animatedModelData.BonesBindPose.Length];
            bonesAnimation = new Matrix[animatedModelData.BonesBindPose.Length];
            bonesTransform = new Matrix[animatedModelData.BonesBindPose.Length];

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] = animatedModelData.BonesBindPose[i];
                bonesTransform[i] = Matrix.Identity;
            }

            // Get the animated model effect - Shared by all meshes
            animatedModelEffect = new AnimatedModelEffect(model.Meshes[0].Effects[0]);

            // Create a default material
            lightMaterial = new LightMaterial();

        }

        private void ResetAnimation()
        {
            // Reset animation
            activeAnimationTime = TimeSpan.Zero;
            activeAnimationKeyframe = 0;

            for (int i = 0; i < bones.Length; i++)
                bones[i] = animatedModelData.BonesBindPose[i];
        }

        private void UpdateAnimation(GameTime time, Matrix parent)
        {
            activeAnimationTime += new TimeSpan((long)(time.ElapsedGameTime.Ticks * animationSpeed));

            if (activeAnimation != null)
            {
                // Loop the animation
                if (activeAnimationTime > activeAnimation.Duration && enableAnimationLoop)
                {
                    long elapsedTicks = activeAnimationTime.Ticks % activeAnimation.Duration.Ticks;
                    activeAnimationTime = new TimeSpan(elapsedTicks);
                    activeAnimationKeyframe = 0;

                    ResetAnimation();
                }

                // Read all animation keyframes until the current time
                // That's possible because we have sorted the keyframes by time in the model processor
                int index = 0;
                Keyframe[] keyframes = activeAnimation.Keyframes;

                while (index < keyframes.Length && keyframes[index].Time <= activeAnimationTime)
                {
                    int boneIndex = keyframes[index].Bone;
                    bones[boneIndex] = keyframes[index].Transform;
                    index++;
                }
                activeAnimationKeyframe = index - 1;
            }

            // Apply the custom transformation over all bones
            for (int i = 0; i < bones.Length; i++)
            {
                bonesAbsolute[i] = bones[i] * bonesTransform[i];
            }

            // Fill the bones with their absolute coordinate (Not relative to the parent bones)
            // Note that we don't need to worry about the hierarchy because our bone list 
            // was made using a depth-first search
            bonesAbsolute[0] = bonesAbsolute[0] * parent;
            for (int i = 1; i < bonesAnimation.Length; i++)
            {
                int boneParent = animatedModelData.BonesParent[i];

                // Here we are transforming the child bone by it's father position and orientation
                bonesAbsolute[i] = bonesAbsolute[i] * bonesAbsolute[boneParent];
            }

            // Before we could transform the vertices using the calculated bone matriz we
            // need to put the vertices in the coordinate system of the skeleton bind pose
            for (int i = 0; i < bonesAnimation.Length; i++)
            {
                bonesAnimation[i] = animatedModelData.BonesInverseBindPose[i] *
                    bonesAbsolute[i];
            }
        }

        public override void Update(GameTime time)
        {
            Update(time, Matrix.Identity);
        }

        public void Update(GameTime time, Matrix world)
        {
            float elapsedTimeSeconds = (float)time.ElapsedGameTime.TotalSeconds;

            UpdateAnimation(time, world);
        }

        private void SetEffectMaterial()
        {
            // Get the first two lights from the light manager
            PointLight light0 = PointLight.NoLight;
            PointLight light1 = PointLight.NoLight;
            if (lightManager.Count > 0)
            {
                light0 = lightManager[0] as PointLight;
                if (lightManager.Count > 1)
                    light1 = lightManager[1] as PointLight;
            }
            
            // Lights
            animatedModelEffect.AmbientLightColor = lightManager.AmbientLightColor;
            animatedModelEffect.Light1Position = light0.Position;
            animatedModelEffect.Light1Color = light0.Color;
            animatedModelEffect.Light2Position = light1.Position;
            animatedModelEffect.Light2Color = light1.Color;

            // Configure material
            animatedModelEffect.DiffuseColor = lightMaterial.DiffuseColor;
            animatedModelEffect.SpecularColor = lightMaterial.SpecularColor;
            animatedModelEffect.SpecularPower = lightMaterial.SpecularPower;
            
            // Camera and world transformations
            animatedModelEffect.World = transformation.Matrix;
            animatedModelEffect.View = cameraManager.ActiveCamera.View;
            animatedModelEffect.Projection = cameraManager.ActiveCamera.Projection;
            animatedModelEffect.Bones = bonesAnimation;
        }

        public override void Draw(GameTime gameTime)
        {
            SetEffectMaterial();

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                model.Meshes[i].Draw();
            }
        }
    }
}
