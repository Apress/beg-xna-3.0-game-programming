using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_TPS.GameBase.Shapes;
using XNA_TPS.GameBase;
using XNA_TPS.GameLogic;

namespace XNA_TPS.GameLogic
{
    public class PlayerWeapon : DrawableGameComponent
    {
        static int WEAPON_AIM_BONE = 2;

        UnitTypes.PlayerWeaponType weaponType;
        AnimatedModel weaponModel;
        int bulletDamage;
        int maxBullets;
        int bulletsCount;

        Vector3 firePosition;
        Vector3 targetDirection;

        #region Properties
        public int BulletDamage
        {
            get
            {
                return bulletDamage;
            }
            set
            {
                bulletDamage = value;
            }
        }

        public int MaxBullets
        {
            get
            {
                return maxBullets;
            }
            set
            {
                maxBullets = value;
            }
        }

        public int BulletsCount
        {
            get
            {
                return bulletsCount;
            }
            set
            {
                bulletsCount = value;
            }
        }

        public Transformation Transformation
        {
            get
            {
                return weaponModel.Transformation;
            }
            set
            {
                weaponModel.Transformation = value;
            }
        }

        public Vector3 FirePosition
        {
            get
            {
                return firePosition;
            }
        }

        public Vector3 TargetDirection
        {
            get
            {
                return targetDirection;
            }
            set
            {
                targetDirection = value;
            }
        }

        #endregion

        public PlayerWeapon(Game game, UnitTypes.PlayerWeaponType weaponType)
            : base(game)
        {
            this.weaponType = weaponType;

            // Weapon configuration
            bulletDamage = UnitTypes.BulletDamage[(int)weaponType];
            bulletsCount = UnitTypes.BulletsCount[(int)weaponType];
            maxBullets = bulletsCount;
        }

        protected override void LoadContent()
        {
            // Load weapon model
            weaponModel = new AnimatedModel(Game);
            weaponModel.Initialize();
            weaponModel.Load(UnitTypes.PlayerWeaponModelFileName[(int)weaponType]);

            base.LoadContent();
        }

        public override void Update(GameTime time)
        {
            Update(time, Matrix.Identity);
        }

        public void Update(GameTime time, Matrix parentBone)
        {
            weaponModel.Update(time, parentBone);
            firePosition = weaponModel.BonesAbsolute[WEAPON_AIM_BONE].Translation;
        }

        public override void Draw(GameTime time)
        {
            weaponModel.Draw(time);
        }
    }
}
