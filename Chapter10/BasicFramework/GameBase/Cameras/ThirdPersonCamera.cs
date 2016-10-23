using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace BasicEngine.GameBase.Cameras
{
    public class ThirdPersonCamera : BaseCamera
    {
        // Maximum allowed eye rotate
        public static float MAX_ROTATE = 30.0f;

        // Chase parameters
        float desiredChaseDistance;
        float minChaseDistance;
        float maxChaseDistance;
        float chaseSpeed;

        // Chase position and direction
        Vector3 chasePosition;
        Vector3 chaseDirection;

        // Rotate eye position
        Vector3 eyeRotate;
        Vector3 eyeRotateVelocity;

        // Start chasing now
        bool isFirstTimeChase;

        #region Properties
        public Vector3 EyeRotate
        {
            get
            {
                return eyeRotate;
            }
            set
            {
                eyeRotate = value;
                needUpdateView = true;
            }
        }

        public Vector3 EyeRotateVelocity
        {
            get
            {
                return eyeRotateVelocity;
            }
            set
            {
                eyeRotateVelocity = value;
            }
        }

        public bool IsFirstTimeChase
        {
            get
            {
                return isFirstTimeChase;
            }
            set
            {
                isFirstTimeChase = value;
            }
        }

        public Vector3 ChasePosition
        {
            get
            {
                return chasePosition;
            }
            set
            {
                chasePosition = value;
            }
        }

        public Vector3 ChaseDirection
        {
            get
            {
                return chaseDirection;
            }
            set
            {
                chaseDirection = value;
            }
        }

        public float ChaseSpeed
        {
            get
            {
                return chaseSpeed;
            }
            set
            {
                chaseSpeed = value;
            }
        }
        #endregion

        public ThirdPersonCamera()
        {
            SetChaseParameters(2.0f, 10.0f, 5.0f, 15.0f);
            isFirstTimeChase = true;
        }

        public void SetChaseParameters(float chaseSpeed, float desiredChaseDistance,
            float minChaseDistance, float maxChaseDistance)
        {
            this.chaseSpeed = chaseSpeed;
            this.desiredChaseDistance = desiredChaseDistance;
            this.minChaseDistance = minChaseDistance;
            this.maxChaseDistance = maxChaseDistance;
        }

        private void UpdateFollowPosition(float elapsedTimeSeconds, bool interpolate)
        {
            Vector3 TargetPosition = chasePosition;
            Vector3 desiredCameraPosition = chasePosition - chaseDirection * desiredChaseDistance;

            if (interpolate)
            {
                float interpolatedSpeed = MathHelper.Clamp(chaseSpeed * elapsedTimeSeconds, 0.0f, 1.0f);

                // Correct the rotate interpolation problem
                Vector3 TargetVec = desiredCameraPosition - Position;
                TargetVec.Normalize();
                if (Vector3.Dot(TargetVec, chaseDirection) < 0.5)
                    interpolatedSpeed += chaseSpeed * 0.005f;

                desiredCameraPosition = Vector3.Lerp(Position, desiredCameraPosition, interpolatedSpeed);

                // Clamp the min and max follow distances
                Vector3 TargetVector = desiredCameraPosition - TargetPosition;
                float TargetLength = TargetVector.Length();
                TargetVector /= TargetLength;

                if (TargetLength < minChaseDistance)
                {
                    desiredCameraPosition = TargetPosition +
                        TargetVector * minChaseDistance;
                }
                else if (TargetLength > maxChaseDistance)
                {
                    desiredCameraPosition = TargetPosition +
                        TargetVector * maxChaseDistance;
                }
            }

            // Needed to recalculate heading, strafe and up vectors
            SetLookAt(desiredCameraPosition, TargetPosition, UpVector);
        }

        public override void Update(GameTime time)
        {
            float elapsedTimeSeconds = (float)time.ElapsedGameTime.TotalSeconds;

            UpdateFollowPosition(elapsedTimeSeconds, !isFirstTimeChase);

            if (isFirstTimeChase)
            {
                eyeRotate = Vector3.Zero;
                eyeRotateVelocity = Vector3.Zero;
                needUpdateView = true;

                isFirstTimeChase = false;
            }

            if (eyeRotateVelocity != Vector3.Zero)
            {
                eyeRotate += eyeRotateVelocity * elapsedTimeSeconds;
                eyeRotate.X = MathHelper.Clamp(eyeRotate.X, -MAX_ROTATE, MAX_ROTATE);
                eyeRotate.Y = MathHelper.Clamp(eyeRotate.Y, -MAX_ROTATE, MAX_ROTATE);
                eyeRotate.Z = MathHelper.Clamp(eyeRotate.Z, -MAX_ROTATE, MAX_ROTATE);
                needUpdateView = true;
            }
        }

        protected override void UpdateView()
        {
            Vector3 newPosition = Position - Target;

            newPosition = Vector3.Transform(newPosition,
                Matrix.CreateFromAxisAngle(UpVector, MathHelper.ToRadians(eyeRotate.Y)) *
                Matrix.CreateFromAxisAngle(RightVector, MathHelper.ToRadians(eyeRotate.X)) *
                Matrix.CreateFromAxisAngle(HeadingVector, MathHelper.ToRadians(eyeRotate.Z))
                );
            viewMatrix = Matrix.CreateLookAt(newPosition + Target, Target, UpVector);

            needUpdateView = false;
            needUpdateFrustum = true;
        }
    }
}
