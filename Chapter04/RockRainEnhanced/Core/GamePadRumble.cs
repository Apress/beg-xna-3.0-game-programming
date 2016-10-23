#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace RockRainEnhanced.Core
{
    /// <summary>
    /// This component helps shake your Joystick
    /// </summary>
    public class SimpleRumblePad : GameComponent
    {
        private int time;
        private int lastTickCount;

        public SimpleRumblePad(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (time > 0)
            {
                int elapsed = System.Environment.TickCount - lastTickCount;
                if (elapsed >= time)
                {
                    time = 0;
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                    GamePad.SetVibration(PlayerIndex.Two, 0, 0);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Turn off the rumble
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);

            base.Dispose(disposing);
        }

        /// <summary>
        /// Set the vibration
        /// </summary>
        /// <param name="playerIndex">Pad ID</param>
        /// <param name="Time">Vibration time</param>
        /// <param name="LeftMotor">Left Motor Intensity</param>
        /// <param name="RightMotor">Right Motor Intensity</param>
        public void RumblePad(PlayerIndex playerIndex, int Time, float LeftMotor, 
            float RightMotor)
        {
            lastTickCount = System.Environment.TickCount;
            time = Time;
            GamePad.SetVibration(playerIndex, LeftMotor, RightMotor);
        }

        /// <summary>
        /// Stop the vibration
        /// </summary>
        /// <param name="playerIndex">Pad ID</param>
        public void Stop(PlayerIndex playerIndex)
        {
            GamePad.SetVibration(playerIndex, 0, 0);
        }
    }
}