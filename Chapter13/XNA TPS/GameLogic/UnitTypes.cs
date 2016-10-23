using System;
using System.Collections.Generic;
using System.Text;

namespace XNA_TPS.GameLogic
{
    public static class UnitTypes
    {
        // Player
        // ---------------------------------------------------------------------------
        public enum PlayerType
        {
            Marine
        }
        public static string[] PlayerModelFileName = { "PlayerMarine" };
        public static int[] PlayerLife = { 100 };
        public static float[] PlayerSpeed = { 1.0f };

        // Player Weapons
        // ---------------------------------------------------------------------------
        public enum PlayerWeaponType
        {
            MachineGun
        }
        public static string[] PlayerWeaponModelFileName = { "WeaponMachineGun" };
        public static int[] BulletDamage = { 12 };
        public static int[] BulletsCount = { 300 };

        // Enemies
        // ---------------------------------------------------------------------------
        public enum EnemyType
        {
            Beast
        }
        public static string[] EnemyModelFileName = { "EnemyBeast" };
        public static int[] EnemyLife = { 150 };
        public static float[] EnemySpeed = { 1.0f };
        public static int[] EnemyPerceptionDistance = { 120 };
        public static int[] EnemyAttackDistance = { 30 };
        public static int[] EnemyAttackDamage = { 8 };
    }
}
