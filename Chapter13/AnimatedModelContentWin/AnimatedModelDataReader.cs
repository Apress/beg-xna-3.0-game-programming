using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace AnimatedModelContent
{
    public class KeyframeReader : ContentTypeReader<Keyframe>
    {
        protected override Keyframe Read(ContentReader input, Keyframe existingInstance)
        {
            TimeSpan time = input.ReadObject<TimeSpan>();
            int boneIndex = input.ReadInt32();
            Matrix transform = input.ReadMatrix();

            return new Keyframe(time, boneIndex, transform);
        }
    }

    public class AnimationDataReader : ContentTypeReader<AnimationData>
    {
        protected override AnimationData Read(ContentReader input, AnimationData existingInstance)
        {
            string name = input.ReadString();
            TimeSpan duration = input.ReadObject<TimeSpan>();
            Keyframe[] keyframes = input.ReadObject<Keyframe[]>();

            return new AnimationData(name, duration, keyframes);
        }
    }

    public class AnimatedModelDataReader : ContentTypeReader<AnimatedModelData>
    {
        protected override AnimatedModelData Read(ContentReader input, AnimatedModelData existingInstance)
        {
            Matrix[] bonesBindPose = input.ReadObject<Matrix[]>();
            Matrix[] bonesInverseBindPose = input.ReadObject<Matrix[]>();
            int[] bonesParent = input.ReadObject<int[]>();
            AnimationData[] animations = input.ReadObject<AnimationData[]>();

            return new AnimatedModelData(bonesBindPose, bonesInverseBindPose, bonesParent, animations);
        }
    }
}
