using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace AnimatedModelContent
{
    public class AnimatedModelData
    {
        Matrix[] bonesBindPose;
        Matrix[] bonesInverseBindPose;
        int[] bonesParent;

        AnimationData[] animations;

        #region Properties
        public int[] BonesParent
        {
            get
            {
                return bonesParent;
            }
            set
            {
                bonesParent = value;
            }
        }

        public Matrix[] BonesBindPose
        {
            get
            {
                return bonesBindPose;
            }
            set
            {
                bonesBindPose = value;
            }
        }

        public Matrix[] BonesInverseBindPose
        {
            get
            {
                return bonesInverseBindPose;
            }
            set
            {
                bonesInverseBindPose = value;
            }
        }

        public AnimationData[] Animations
        {
            get
            {
                return animations;
            }
            set
            {
                animations = value;
            }
        }
        #endregion

        public AnimatedModelData(Matrix[] bonesBindPose, Matrix[] bonesInverseBindPose,
            int[] bonesParent, AnimationData[] animations)
        {
            this.bonesParent = bonesParent;
            this.bonesBindPose = bonesBindPose;
            this.bonesInverseBindPose = bonesInverseBindPose;
            this.animations = animations;
        }

    }
}
