using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_TPS.GameBase.Cameras;

namespace XNA_TPS.GameLogic
{
    public class Player : TerrainUnit
    {
        static float MAX_WAIST_BONE_ROTATE = 0.50f;
        static int WAIST_BONE_ID = 2;
        static int RIGHT_HAND_BONE_ID = 15;

        public enum PlayerAnimations
        {
            Idle = 0,
            Run,
            Aim,
            Shoot
        }

        // Player type
        UnitTypes.PlayerType playerType;
        // Player weapon
        PlayerWeapon playerWeapon;
        // Camera chase position
        Vector3[] chaseOffsetPosition;
        // Rotate torso bone
        float rotateWaistBone;
        float rotateWaistBoneVelocity;

        #region Properties
        public PlayerAnimations CurrentAnimation
        {
            get
            {
                return (PlayerAnimations)CurrentAnimationId;
            }
        }

        public Vector3[] ChaseOffsetPosition
        {
            get
            {
                return chaseOffsetPosition;
            }
            set
            {
                chaseOffsetPosition = value;
            }
        }

        public PlayerWeapon Weapon
        {
            get
            {
                return playerWeapon;
            }
        }

        public float RotateWaist
        {
            get
            {
                return rotateWaistBone;
            }
            set
            {
                rotateWaistBone = value;

                // Rotate torso bone
                Matrix rotate = Matrix.CreateRotationZ(rotateWaistBone);
                AnimatedModel.BonesTransform[WAIST_BONE_ID] = rotate;
            }
        }

        public float RotateWaistVelocity
        {
            get
            {
                return rotateWaistBoneVelocity;
            }
            set
            {
                rotateWaistBoneVelocity = value;
            }
        }
        #endregion

        public Player(Game game, UnitTypes.PlayerType playerType)
            : base(game)
        {
            this.playerType = playerType;
        }

        protected override void LoadContent()
        {
            Load(UnitTypes.PlayerModelFileName[(int)playerType]);

            // Unit configurations
            Life = UnitTypes.PlayerLife[(int)playerType];
            MaxLife = Life;
            Speed = UnitTypes.PlayerSpeed[(int)playerType];

            SetAnimation(Player.PlayerAnimations.Idle, false, true, false);

            base.LoadContent();
        }

        public void AttachWeapon(UnitTypes.PlayerWeaponType weaponType)
        {
            playerWeapon = new PlayerWeapon(Game, weaponType);
            playerWeapon.Initialize();
        }

        public void SetAnimation(PlayerAnimations animation, bool reset, bool enableLoop, bool waitFinish)
        {
            SetAnimation((int)animation, reset, enableLoop, waitFinish);
        }

        private void UpdateChasePosition()
        {
            ThirdPersonCamera camera = cameraManager.ActiveCamera as ThirdPersonCamera;
            if (camera != null)
            {
                // Get camera offset position for the active camera
                Vector3 cameraOffset = Vector3.Zero;
                if (chaseOffsetPosition != null)
                    cameraOffset = chaseOffsetPosition[cameraManager.ActiveCameraIndex];

                // Get the model center
                Vector3 center = BoundingSphere.Center;

                // Calculate chase position and direction
                camera.ChasePosition = center +
                    cameraOffset.X * StrafeVector +
                    cameraOffset.Y * UpVector +
                    cameraOffset.Z * HeadingVector;
                camera.ChaseDirection = HeadingVector;
            }
        }

        private void UpdateWaistBone(float elapsedTimeSeconds)
        {
            if (rotateWaistBoneVelocity != 0.0f)
            {
                rotateWaistBone += rotateWaistBoneVelocity * elapsedTimeSeconds;
                rotateWaistBone = MathHelper.Clamp(rotateWaistBone, -MAX_WAIST_BONE_ROTATE, MAX_WAIST_BONE_ROTATE);

                // Rotate torso bone
                Matrix rotate = Matrix.CreateRotationZ(rotateWaistBone);
                AnimatedModel.BonesTransform[WAIST_BONE_ID] = rotate;
            }
        }

        public override void Update(GameTime time)
        {
            float elapsedTimeSeconds = (float)time.ElapsedGameTime.TotalSeconds;
            UpdateWaistBone(elapsedTimeSeconds);

            base.Update(time);
            UpdateChasePosition();

            // Update player weapon
            Matrix transformedHand = AnimatedModel.BonesAnimation[RIGHT_HAND_BONE_ID] * Transformation.Matrix;
            playerWeapon.TargetDirection = HeadingVector + UpVector * rotateWaistBone;
            playerWeapon.Update(time, transformedHand);
        }

        public override void Draw(GameTime time)
        {
            playerWeapon.Draw(time);

            base.Draw(time);
        }
    }
}
