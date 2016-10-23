using System;
using System.Collections.Generic;
using System.Text;

namespace AnimatedModelContent
{
    public class AnimationData
    {
        string name;
        TimeSpan duration;
        Keyframe[] keyframes;

        #region Properties
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }

        public Keyframe[] Keyframes
        {
            get
            {
                return keyframes;
            }
            set
            {
                keyframes = value;
            }
        }
        #endregion

        public AnimationData(string name, TimeSpan duration, Keyframe[] keyframes)
        {
            this.name = name;
            this.duration = duration;
            this.keyframes = keyframes;
        }
    }
}
