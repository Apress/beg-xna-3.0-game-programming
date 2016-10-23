using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_TPS.GameBase;
using XNA_TPS.Helpers;

namespace XNA_TPS.GameLogic
{
    public class Enemy : TerrainUnit
    {
        public enum EnemyAnimations
        {
            Idle = 0,
            Run,
            Bite,
            TakeDamage,
            Die
        }

        public enum EnemyState
        {
            Wander = 0,
            ChasePlayer,
            AttackPlayer,
            Dead
        }

        static float DISTANCE_EPSILON = 1.0f;
        static float LINEAR_VELOCITY_CONSTANT = 35.0f;
        static float ANGULAR_VELOCITY_CONSTANT = 100.0f;

        static int WANDER_MAX_MOVES = 3;
        static int WANDER_DISTANCE = 80;
        static float WANDER_DELAY_SECONDS = 4.0f;
        static float ATTACK_DELAY_SECONDS = 1.5f;

        EnemyState state;
        float nextActionTime;

        // Wander
        int wanderMovesCount;
        Vector3 wanderPosition;
        Vector3 wanderStartPosition;

        // Chase
        float perceptionDistance;
        Vector3 chaseVector;

        // Attack
        bool isHited;
        Player player;
        float attackDistance;
        int attackDamage;

        UnitTypes.EnemyType enemyType;

        #region Properties
        public EnemyAnimations CurrentAnimation
        {
            get
            {
                return (EnemyAnimations)CurrentAnimationId;
            }
        }

        public EnemyState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public Player Player
        {
            set
            {
                player = value;
            }
        }

        public override Transformation Transformation
        {
            get
            {
                return AnimatedModel.Transformation;
            }
            set
            {
                base.Transformation = value;
                wanderPosition = value.Translate;
                wanderStartPosition = value.Translate;
            }
        }
        #endregion

        public Enemy(Game game, UnitTypes.EnemyType enemyType)
            : base(game)
        {
            this.enemyType = enemyType;

            isHited = false;
            wanderMovesCount = 0;
        }

        protected override void LoadContent()
        {
            Load(UnitTypes.EnemyModelFileName[(int)enemyType]);

            // Unit configurations
            Life = UnitTypes.EnemyLife[(int)enemyType];
            MaxLife = Life;
            Speed = UnitTypes.EnemySpeed[(int)enemyType];
            perceptionDistance = UnitTypes.EnemyPerceptionDistance[(int)enemyType];
            attackDamage = UnitTypes.EnemyAttackDamage[(int)enemyType];
            attackDistance = UnitTypes.EnemyAttackDistance[(int)enemyType];

            wanderPosition = Transformation.Translate;
            SetAnimation(EnemyAnimations.Idle, false, true, false);

            base.LoadContent();
        }

        public override void ReceiveDamage(int damageValue)
        {
            base.ReceiveDamage(damageValue);

            // Chase
            isHited = true;

            if (Life > 0)
            {
                if (CurrentAnimation != EnemyAnimations.Bite)
                    if (CurrentAnimation == EnemyAnimations.TakeDamage)
                        SetAnimation(EnemyAnimations.TakeDamage, true, false, true);
                    else
                        SetAnimation(EnemyAnimations.TakeDamage, false, false, false);
            }
            else
            {
                state = EnemyState.Dead;
                SetAnimation(EnemyAnimations.Die, false, false, false);
            }
        }

        public void SetAnimation(EnemyAnimations animation, bool reset, bool enableLoop, bool waitFinish)
        {
            SetAnimation((int)animation, reset, enableLoop, waitFinish);
        }

        private void Move(Vector3 direction)
        {
            SetAnimation(EnemyAnimations.Run, false, true, (CurrentAnimation == EnemyAnimations.TakeDamage));
            LinearVelocity = direction * LINEAR_VELOCITY_CONSTANT;

            // Angle between heading and move direction
            float radianAngle = (float)Math.Acos(Vector3.Dot(HeadingVector, direction));
            if (radianAngle >= 0.1f)
            {
                // Find short side to rodade CW or CCW
                float sideToRotate = Vector3.Dot(StrafeVector, direction);

                Vector3 rotationVector = new Vector3(0, ANGULAR_VELOCITY_CONSTANT * radianAngle, 0);
                if (sideToRotate > 0)
                    AngularVelocity = -rotationVector;
                else
                    AngularVelocity = rotationVector;
            }
        }

        private void Wander(GameTime time)
        {
            // Calculate wander vector on X, Z axis
            Vector3 wanderVector = wanderPosition - Transformation.Translate;
            wanderVector.Y = 0;
            float wanderVectorLength = wanderVector.Length();

            // Reached the destination position
            if (wanderVectorLength < DISTANCE_EPSILON)
            {
                SetAnimation(EnemyAnimations.Idle, false, true, false);

                // Generate new random position
                if (wanderMovesCount < WANDER_MAX_MOVES)
                {
                    wanderPosition = Transformation.Translate +
                        RandomHelper.GeneratePositionXZ(WANDER_DISTANCE);

                    wanderMovesCount++;
                }
                // Go back to the start position
                else
                {
                    wanderPosition = wanderStartPosition;
                    wanderMovesCount = 0;
                }

                // Next time wander
                nextActionTime = (float)time.TotalGameTime.TotalSeconds + WANDER_DELAY_SECONDS +
                    WANDER_DELAY_SECONDS * (float)RandomHelper.RandomGenerator.NextDouble();
            }

            // Wait for the next action time
            if (time.TotalGameTime.TotalSeconds > nextActionTime)
            {
                wanderVector *= (1.0f / wanderVectorLength);
                Move(wanderVector);
            }
        }

        private void ChasePlayer(GameTime time)
        {
            Move(chaseVector);
        }

        private void AttackPlayer(GameTime time)
        {
            float elapsedTimeSeconds = (float)time.TotalGameTime.TotalSeconds;
            if (elapsedTimeSeconds > nextActionTime)
            {
                SetAnimation(EnemyAnimations.Bite, false, true, false);

                player.ReceiveDamage(attackDamage);
                nextActionTime = elapsedTimeSeconds + ATTACK_DELAY_SECONDS;
            }
        }

        public override void Update(GameTime time)
        {
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;

            chaseVector = player.Transformation.Translate - Transformation.Translate;
            float distanceToPlayer = chaseVector.Length();
            
            // Normalize chase vector
            chaseVector *= (1.0f / distanceToPlayer);

            switch (state)
            {
                case EnemyState.Wander:
                    if (isHited || distanceToPlayer < perceptionDistance)
                        // Change state
                        state = EnemyState.ChasePlayer;
                    else
                        Wander(time);
                    break;

                case EnemyState.ChasePlayer:
                    if (distanceToPlayer <= attackDistance)
                    {
                        // Change state
                        state = EnemyState.AttackPlayer;
                        nextActionTime = 0;
                    }
                    else
                        ChasePlayer(time);
                    break;

                case EnemyState.AttackPlayer:
                    if (distanceToPlayer > attackDistance * 1.5f)
                        // Change state
                        state = EnemyState.ChasePlayer;
                    else
                        AttackPlayer(time);
                    break;

                default:
                    break;
            }

            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            base.Draw(time);
        }
    }
}
