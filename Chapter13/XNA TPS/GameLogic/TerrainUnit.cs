using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_TPS.GameBase;
using XNA_TPS.GameBase.Cameras;
using XNA_TPS.GameBase.Shapes;

namespace XNA_TPS.GameLogic
{
    public class TerrainUnit : DrawableGameComponent
    {
        public static float MIN_GRAVITY = -1.5f;
        public static float GRAVITY_ACCELERATION = 4.0f;

        int life;
        int maxLife;
        float speed;

        AnimatedModel animatedModel;
        int currentAnimationId;
        BoundingBox boundingBox;
        BoundingSphere boundingSphere;
        bool needUpdateCollision;

        Vector3 linearVelocity;
        Vector3 angularVelocity;
        float gravityVelocity;

        Vector3 headingVec;
        Vector3 strafeVec;
        Vector3 upVec;

        bool adjustJumpChanges;
        bool isOnTerrain;
        bool isDead;

        // Necessary services
        protected CameraManager cameraManager;
        protected Terrain terrain;

        #region Properties
        public int Life
        {
            get
            {
                return life;
            }
            set
            {
                life = value;
            }
        }

        public int MaxLife
        {
            get
            {
                return maxLife;
            }
            set
            {
                maxLife = value;
            }
        }

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public Vector3 LinearVelocity
        {
            get
            {
                return linearVelocity;
            }
            set
            {
                linearVelocity = value;
            }
        }

        public Vector3 AngularVelocity
        {
            get
            {
                return angularVelocity;
            }
            set
            {
                angularVelocity = value;
            }
        }

        public float GravityVelocity
        {
            get
            {
                return gravityVelocity;
            }
            set
            {
                gravityVelocity = value;
            }
        }


        public Vector3 HeadingVector
        {
            get
            {
                return headingVec;
            }
        }

        public Vector3 StrafeVector
        {
            get
            {
                return strafeVec;
            }
        }

        public Vector3 UpVector
        {
            get
            {
                return upVec;
            }
        }

        public AnimatedModel AnimatedModel
        {
            get
            {
                return animatedModel;
            }
        }

        protected int CurrentAnimationId
        {
            get
            {
                return currentAnimationId;
            }
        }

        public virtual Transformation Transformation
        {
            get
            {
                return animatedModel.Transformation;
            }
            set
            {
                animatedModel.Transformation = value;

                // Upate
                UpdateHeight(0);
                NormalizeBaseVectors();
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                if (needUpdateCollision)
                    UpdateCollision();

                return boundingBox;
            }
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                if (needUpdateCollision)
                    UpdateCollision();

                return boundingSphere;
            }
        }

        public bool IsOnTerrain
        {
            get
            {
                return isOnTerrain;
            }
        }

        public bool IsDead
        {
            get
            {
                return isDead;
            }
        }

        #endregion

        public TerrainUnit(Game game)
            : base(game)
        {
            gravityVelocity = 0.0f;
            isOnTerrain = false;
            isDead = false;
            adjustJumpChanges = false;

            needUpdateCollision = true;
        }

        protected void Load(string unitModelFileName)
        {
            animatedModel = new AnimatedModel(Game);
            animatedModel.Initialize();
            animatedModel.Load(unitModelFileName);

            // Put the player above the terrain
            UpdateHeight(0);
            isOnTerrain = true;

            NormalizeBaseVectors();
        }

        public override void Initialize()
        {
            cameraManager = Game.Services.GetService(typeof(CameraManager)) as CameraManager;
            terrain = Game.Services.GetService(typeof(Terrain)) as Terrain;

            base.Initialize();
        }

        protected void SetAnimation(int animationId, bool reset, bool enableLoop, bool waitFinish)
        {
            if (reset || currentAnimationId != animationId)
            {
                if (waitFinish && !AnimatedModel.IsAnimationFinished)
                    return;

                AnimatedModel.ActiveAnimation = AnimatedModel.Animations[animationId];
                AnimatedModel.EnableAnimationLoop = enableLoop;
                currentAnimationId = animationId;
            }
        }

        private void NormalizeBaseVectors()
        {
            headingVec = Transformation.Matrix.Forward;
            strafeVec = Transformation.Matrix.Right;
            upVec = Transformation.Matrix.Up;

            headingVec.Normalize();
            strafeVec.Normalize();
            upVec.Normalize();
        }

        public override void Update(GameTime time)
        {
            float elapsedTimeSeconds = (float)time.ElapsedGameTime.TotalSeconds;
            animatedModel.Update(time, Matrix.Identity);

            // Update the height and collision volumes
            if (linearVelocity != Vector3.Zero || gravityVelocity != 0.0f)
            {
                Transformation.Translate += linearVelocity * elapsedTimeSeconds * speed;
                UpdateHeight(elapsedTimeSeconds);
                needUpdateCollision = true;
            }

            // Update coordinate system when the unit rotates
            if (angularVelocity != Vector3.Zero)
            {
                Transformation.Rotate += angularVelocity * elapsedTimeSeconds * speed;
                NormalizeBaseVectors();
            }

            base.Update(time);

        }

        public void Jump(float jumpHeight)
        {
            if (isOnTerrain)
            {
                // Update camera chase speed and unit speed
                ThirdPersonCamera camera = cameraManager.ActiveCamera as ThirdPersonCamera;
                camera.ChaseSpeed *= 4.0f;
                speed *= 1.5f;
                adjustJumpChanges = true;

                gravityVelocity = (float)GRAVITY_ACCELERATION * jumpHeight * 0.1f;
                isOnTerrain = false;
            }
        }

        private void UpdateHeight(float elapsedTimeSeconds)
        {
            // Get terrain height
            float terrainHeight = terrain.GetHeight(Transformation.Translate);
            Vector3 newPosition = Transformation.Translate;

            // Unit is on terrain
            float HEIGHT_EPSILON = 2.0f;
            if (Transformation.Translate.Y <= (terrainHeight + HEIGHT_EPSILON) && gravityVelocity <= 0)
            {
                isOnTerrain = true;
                gravityVelocity = 0.0f;
                newPosition.Y = terrainHeight;
                
                // Update camera chase speed and unit movement speed (hack)
                if (adjustJumpChanges)
                {
                    ThirdPersonCamera camera = cameraManager.ActiveCamera as ThirdPersonCamera;
                    camera.ChaseSpeed /= 4.0f;
                    speed /= 1.5f;
                    adjustJumpChanges = false;
                }
            }
            // Unit is above the terrain
            else
            {
                isOnTerrain = false;
                // Gravity
                if (gravityVelocity > MIN_GRAVITY)
                    gravityVelocity -= GRAVITY_ACCELERATION * elapsedTimeSeconds;
                newPosition.Y = Math.Max(terrainHeight, Transformation.Translate.Y + gravityVelocity);
            }
            Transformation.Translate = newPosition;
        }

        public override void Draw(GameTime time)
        {
            animatedModel.Draw(time);
        }

        public virtual void ReceiveDamage(int damageValue)
        {
            life = Math.Max(0, life - damageValue);
            if (life == 0)
                isDead = true;
        }

        private void UpdateCollision()
        {
            // Do not support scale

            // Update bounding box
            boundingBox = animatedModel.BoundingBox;
            boundingBox.Min += Transformation.Translate;
            boundingBox.Max += Transformation.Translate;

            // Update bounding sphere
            boundingSphere = animatedModel.BoundingSphere;
            boundingSphere.Center += Transformation.Translate;

            needUpdateCollision = false;
        }

        public float? BoxIntersects(Ray ray)
        {
            Matrix inverseTransformation = Matrix.Invert(Transformation.Matrix);
            ray.Position = Vector3.Transform(ray.Position, inverseTransformation);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransformation);

            return animatedModel.BoundingBox.Intersects(ray);
        }
    }
}
