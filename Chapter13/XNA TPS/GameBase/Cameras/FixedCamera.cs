using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace XNA_TPS.GameBase.Cameras
{
    public class FixedCamera : BaseCamera
    {
        public FixedCamera() : base()
        {
        }

        public FixedCamera(Vector3 cameraPosition, Vector3 cameraTarget) : base()
        {
            SetLookAt(cameraPosition, cameraTarget, UpVector);
        }

        public override void Update(GameTime time)
        {

        }
    }
}
