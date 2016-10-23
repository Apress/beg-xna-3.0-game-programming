using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using AnimatedModelContent;

namespace AnimatedModelProcessorWin
{
    [ContentTypeWriter]
    public class KeyframeWriter : ContentTypeWriter<Keyframe>
    {
        protected override void Write(ContentWriter output, Keyframe value)
        {
            output.WriteObject(value.Time);
            output.Write(value.Bone);
            output.Write(value.Transform);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(KeyframeReader).AssemblyQualifiedName;
        }
    }

    [ContentTypeWriter]
    public class AnimationDataWriter : ContentTypeWriter<AnimationData>
    {
        protected override void Write(ContentWriter output, AnimationData value)
        {
            output.Write(value.Name);
            output.WriteObject(value.Duration);
            output.WriteObject(value.Keyframes);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimationDataReader).AssemblyQualifiedName;
        }
    }

    [ContentTypeWriter]
    public class AnimatedModelDataWriter : ContentTypeWriter<AnimatedModelData>
    {
        protected override void Write(ContentWriter output, AnimatedModelData value)
        {
            output.WriteObject(value.BonesBindPose);
            output.WriteObject(value.BonesInverseBindPose);
            output.WriteObject(value.BonesParent);
            output.WriteObject(value.Animations);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimatedModelDataReader).AssemblyQualifiedName;
        }
    }
}
