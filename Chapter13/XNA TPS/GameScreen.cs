using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XNA_TPS.GameBase.Cameras;
using XNA_TPS.GameBase.Lights;
using XNA_TPS.GameBase.Shapes;
using XNA_TPS.GameLogic;
using XNA_TPS.GameLogic.Levels;
using XNA_TPS.Helpers;

namespace XNA_TPS
{
    public class GameScreen : DrawableGameComponent
    {
        // Modified level that we are playing
        LevelCreator.Levels currentLevel;
        GameLevel gameLevel;

        // Text
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        // Weapon target sprite
        Texture2D weaponTargetTexture;
        Vector3 weaponTargetPosition;

        // Aimed enemy
        Enemy aimEnemy;
        int numEnemiesAlive;

        // Frame counter helper
        FrameCounterHelper frameCounter;
        
        // Necessary services
        InputHelper inputHelper;

        public GameScreen(Game game, LevelCreator.Levels currentLevel)
            : base(game)
        {
            this.currentLevel = currentLevel;
        }

        public override void Initialize()
        {
            // Frame counter
            frameCounter = new FrameCounterHelper(Game);

            // Get services
            inputHelper = Game.Services.GetService(typeof(InputHelper)) as InputHelper;
            if (inputHelper == null)
                throw new InvalidOperationException("Cannot find an input service");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create SpriteBatch and add services
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Font 2D
            spriteFont = Game.Content.Load<SpriteFont>(GameAssetsPath.FONTS_PATH +
                "BerlinSans");

            // Weapon target
            weaponTargetTexture = Game.Content.Load<Texture2D>(GameAssetsPath.TEXTURES_PATH +
                "weaponTarget");

            // Load game level
            gameLevel = LevelCreator.CreateLevel(Game, currentLevel);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        private void UpdateInput()
        {
            ThirdPersonCamera fpsCamera = gameLevel.CameraManager["FPSCamera"] as ThirdPersonCamera;
            ThirdPersonCamera followCamera = gameLevel.CameraManager["FollowCamera"] as ThirdPersonCamera;

            Player player = gameLevel.Player;
            Vector2 leftThumb = inputHelper.GetLeftThumbStick();
            
            // Aim Mode
            if (inputHelper.IsKeyPressed(Buttons.LeftShoulder)&& player.IsOnTerrain)
            {
                // Reset follow camera 
                if (gameLevel.CameraManager.ActiveCamera != fpsCamera)
                {
                    gameLevel.CameraManager.SetActiveCamera("FPSCamera");
                    fpsCamera.IsFirstTimeChase = true;
                    player.SetAnimation(Player.PlayerAnimations.Aim, false, false, false);
                }
                
                // Rotate the camera and move the player's aim
                fpsCamera.EyeRotateVelocity = new Vector3(leftThumb.Y * 50.0f, 0.0f, 0.0f);
                player.LinearVelocity = Vector3.Zero;
                player.AngularVelocity = new Vector3(0.0f, -leftThumb.X * 70.0f, 0.0f);
                player.RotateWaistVelocity = leftThumb.Y * 0.8f;

                // Shoot
                if (inputHelper.IsKeyJustPressed(Buttons.A) && player.Weapon.BulletsCount > 0)
                {
                    // Wait the last shoot animation finish
                    if (player.AnimatedModel.IsAnimationFinished)
                    {
                        player.SetAnimation(Player.PlayerAnimations.Shoot, true, false, false);

                        // Damage the enemy
                        player.Weapon.BulletsCount--;
                        if (aimEnemy != null)
                            aimEnemy.ReceiveDamage(player.Weapon.BulletDamage);
                    }
                }
            }
            // Normal Mode
            else
            {
                bool isPlayerIdle = true;

                if (gameLevel.CameraManager.ActiveCamera != followCamera)
                {
                    // Reset fps camera 
                    gameLevel.CameraManager.SetActiveCamera("FollowCamera");
                    followCamera.IsFirstTimeChase = true;
                    player.RotateWaist = 0.0f;
                    player.RotateWaistVelocity = 0.0f;
                }
                
                followCamera.EyeRotateVelocity = new Vector3(leftThumb.Y * 50.0f, 0.0f, 0.0f);
                player.AngularVelocity = new Vector3(0.0f, -leftThumb.X * 70.0f, 0.0f);

                // Run foward 
                if (inputHelper.IsKeyPressed(Buttons.X))
                {
                    player.SetAnimation(Player.PlayerAnimations.Run, false, true, false);
                    player.LinearVelocity = player.HeadingVector * 30.0f;
                    isPlayerIdle = false;
                }
                // Run backward
                else if (inputHelper.IsKeyPressed(Buttons.A))
                {
                    player.SetAnimation(Player.PlayerAnimations.Run, false, true, false);
                    player.LinearVelocity = -player.HeadingVector * 20.0f;
                    isPlayerIdle = false;
                }
                else
                    player.LinearVelocity = Vector3.Zero;

                // Jump
                if (inputHelper.IsKeyJustPressed(Buttons.LeftStick))
                {
                    player.Jump(2.5f);
                    isPlayerIdle = false;
                }

                if (isPlayerIdle)
                    player.SetAnimation(Player.PlayerAnimations.Idle, false, true, false);
            }
        }

        private void UpdateWeaponTarget()
        {
            aimEnemy = null;
            numEnemiesAlive = 0;
            
            // Shoot ray
            Ray ray = new Ray(gameLevel.Player.Weapon.FirePosition, gameLevel.Player.Weapon.TargetDirection);
            
            // Distance from the ray start position to the terrain
            float? distance = gameLevel.Terrain.Intersects(ray);

            // Test intersection with enemies
            foreach (Enemy enemy in gameLevel.EnemyList)
            {
                if (!enemy.IsDead)
                {
                    numEnemiesAlive++;

                    float? enemyDistance = enemy.BoxIntersects(ray);
                    if (enemyDistance != null)
                    {
                        if (distance == null || enemyDistance <= distance)
                        {
                            distance = enemyDistance;
                            aimEnemy = enemy;
                        }
                    }
                }
            }

            // Weapon target position
            weaponTargetPosition = gameLevel.Player.Weapon.FirePosition +
                gameLevel.Player.Weapon.TargetDirection * 300;
        }

        public override void Update(GameTime gameTime)
        {
            // Restart game
            if (gameLevel.Player.IsDead || numEnemiesAlive == 0)
                gameLevel = LevelCreator.CreateLevel(Game, currentLevel);

            UpdateInput();

            // Update player
            gameLevel.Player.Update(gameTime);
            UpdateWeaponTarget();

            // Update camera
            BaseCamera activeCamera = gameLevel.CameraManager.ActiveCamera;
            activeCamera.Update(gameTime);

            // Update light position
            PointLight cameraLight = gameLevel.LightManager["CameraLight"] as PointLight;
            cameraLight.Position = activeCamera.Position;

            // Update enemies
            foreach (Enemy enemy in gameLevel.EnemyList)
            {
                if (enemy.BoundingSphere.Intersects(activeCamera.Frustum) ||
                    enemy.State == Enemy.EnemyState.ChasePlayer ||
                    enemy.State == Enemy.EnemyState.AttackPlayer)

                    enemy.Update(gameTime);

            }

            // Update scene objects
            gameLevel.SkyDome.Update(gameTime);
            gameLevel.Terrain.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1.0f, 255);

            BaseCamera activeCamera = gameLevel.CameraManager.ActiveCamera;

            gameLevel.SkyDome.Draw(gameTime);
            gameLevel.Terrain.Draw(gameTime);
            gameLevel.Player.Draw(gameTime);

            // Draw enemies
            foreach (Enemy enemy in gameLevel.EnemyList)
            {
                if (enemy.BoundingSphere.Intersects(activeCamera.Frustum))
                    enemy.Draw(gameTime);
            }

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

            // Project weapon target
            weaponTargetPosition = GraphicsDevice.Viewport.Project(weaponTargetPosition,
                activeCamera.Projection, activeCamera.View, Matrix.Identity);
            
            // Draw weapon target
            int weaponRectangleSize = GraphicsDevice.Viewport.Width / 40;
            if (activeCamera == gameLevel.CameraManager["FPSCamera"])
                spriteBatch.Draw(weaponTargetTexture, new Rectangle(
                    (int)(weaponTargetPosition.X - weaponRectangleSize * 0.5f),
                    (int)(weaponTargetPosition.Y - weaponRectangleSize * 0.5f),
                    weaponRectangleSize, weaponRectangleSize),
                    (aimEnemy == null)? Color.White : Color.Red);

            // Draw GUI text
            spriteBatch.DrawString(spriteFont, "Health: " + gameLevel.Player.Life + "/" +
                gameLevel.Player.MaxLife, new Vector2(10, 5), Color.Green);
            spriteBatch.DrawString(spriteFont, "Bullets: " + gameLevel.Player.Weapon.BulletsCount + "/" +
                gameLevel.Player.Weapon.MaxBullets, new Vector2(10, 25), Color.Green);
            spriteBatch.DrawString(spriteFont, "Enemies Alive: " + numEnemiesAlive + "/" +
                gameLevel.EnemyList.Count, new Vector2(10, 45), Color.Green);
            

            spriteBatch.DrawString(spriteFont, "FPS: " + frameCounter.LastFrameFps, new Vector2(10, 75),
                Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);

            frameCounter.Update(gameTime);
        }
    }
}
