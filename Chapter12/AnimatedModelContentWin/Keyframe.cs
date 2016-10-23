using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace AnimatedModelContent
{
    public class Keyframe : IComparable
    {
        TimeSpan time;
        int boneIndex;
        Matrix transform;

        #region Properties
        public TimeSpan Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public int Bone
        {
            get
            {
                return boneIndex;
            }
            set
            {
                boneIndex = value;
            }
        }
        public Matrix Transform
        {
            get
            {
                return transform;
            }
            set
            {
                transform = value;
            }
        }
        #endregion

        public Keyframe(TimeSpan time, int boneIndex, Matrix transform)
        {
            this.time = time;
            this.boneIndex = boneIndex;
            this.transform = transform;
        }

        public int CompareTo(object obj)
        {
            Keyframe keyframe = obj as Keyframe;
            if (obj == null)
                throw new ArgumentException("Object is not a Keyframe.");

            return time.CompareTo(keyframe.Time);
        }
    }
}
