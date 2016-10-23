using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using AnimatedModelContent;

namespace AnimatedModelProcessorWin
{
    [ContentProcessor(DisplayName = "AnimatedModel - Skinned Mesh")]
    public class AnimatedModelProcessor : ModelProcessor
    {
        public static string TEXTURES_PATH = "Textures/";
        public static string EFFECTS_PATH = "Effects/";
        public static string EFFECT_FILENAME = "AnimatedModel.fx";

        private void printNodeTree(NodeContent input, ContentProcessorContext context, string start)
        {
            context.Logger.LogImportantMessage(start + "- Name: {0} - {1}", input.Name, input.GetType());
            foreach (NodeContent node in input.Children)
                printNodeTree(node, context, start + "-");
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            // DEBUG
            printNodeTree(input, context, "");

            // Hack (THERES IS A BUG IN THE XNA 2 BETA VERSION. THIS LINE SHOULD BE REMOVED IN THE FINAL VERSION)
            Scale *= 0.1f;

            // Process model
            ModelContent model = base.Process(input, context);

            // Transform all mesh in a absolute coordinate system
            //FlattenMesh(skeleton, input, context);

            // Now extract the model skeleton and all animations
            AnimatedModelData animatedModelData = ExtractSkeletonAndAnimations(input, context);

            // Extract all points from the mesh
            List<Vector3> vertexList = new List<Vector3>();
            GetModelVertices(input, vertexList);
            
            // Generate bounding volumes
            BoundingBox modelBoundBox = BoundingBox.CreateFromPoints(vertexList);
            BoundingSphere modelBoundSphere = BoundingSphere.CreateFromPoints(vertexList);

            Dictionary<string, object> tagDictionary = new Dictionary<string, object>();
            tagDictionary.Add("AnimatedModelData", animatedModelData);
            tagDictionary.Add("ModelBoudingBox", modelBoundBox);
            tagDictionary.Add("ModelBoudingSphere", modelBoundSphere);
            model.Tag = tagDictionary;

            return model;
        }

        private void GetModelVertices(NodeContent node, List<Vector3> vertexList)
        {
            MeshContent meshContent = node as MeshContent;
            if (meshContent != null)
            {
                for (int i = 0; i < meshContent.Geometry.Count; i++)
                {
                    GeometryContent geometryContent = meshContent.Geometry[i];
                    for (int j = 0; j < geometryContent.Vertices.Positions.Count; j++)
                        vertexList.Add(geometryContent.Vertices.Positions[j]);
                }
            }

            foreach (NodeContent child in node.Children)
                GetModelVertices(child, vertexList);
        }

        private AnimatedModelData ExtractSkeletonAndAnimations(NodeContent input, ContentProcessorContext context)
        {
            // Find the root bone node
            BoneContent skeleton = MeshHelper.FindSkeleton(input);
            // Transform the hierarchy in a list
            IList<BoneContent> boneList = MeshHelper.FlattenSkeleton(skeleton);
            context.Logger.LogImportantMessage("{0} bones found.", boneList.Count);

            // Create skeleton bind pose and inverse bind pose
            Matrix[] bonesBindPose = new Matrix[boneList.Count];
            Matrix[] bonesInverseBindPose = new Matrix[boneList.Count];
            int[] bonesParentIndex = new int[boneList.Count];
            List<string> boneNameList = new List<string>(boneList.Count);

            bonesBindPose[0] = boneList[0].AbsoluteTransform;
            bonesInverseBindPose[0] = Matrix.Invert(boneList[0].AbsoluteTransform);
            bonesParentIndex[0] = boneNameList.IndexOf(boneList[0].Parent.Name);
            boneNameList.Add(boneList[0].Name);

            for (int i = 1; i < boneList.Count; i++)
            {
                bonesBindPose[i] = boneList[i].Transform;
                bonesInverseBindPose[i] = Matrix.Invert(boneList[i].AbsoluteTransform);

                int parentIndex = boneNameList.IndexOf(boneList[i].Parent.Name);
                bonesParentIndex[i] = parentIndex;
                boneNameList.Add(boneList[i].Name);
            }

            // DEBUG
            for (int i = 0; i < boneNameList.Count; i++)
                context.Logger.LogImportantMessage("Bone {0}: {1} ", i, boneNameList[i]);

            // Extract all animations
            AnimationData[] animations = ExtractAnimations(skeleton.Animations, boneNameList, context);
            return new AnimatedModelData(bonesBindPose, bonesInverseBindPose, bonesParentIndex, animations);
        }

        private AnimationData[] ExtractAnimations(AnimationContentDictionary animationDictionary, List<string> boneNameList,
            ContentProcessorContext context)
        {
            context.Logger.LogImportantMessage("{0} animations found.", animationDictionary.Count);
            AnimationData[] animations = new AnimationData[animationDictionary.Count];

            int count = 0;
            foreach (AnimationContent animationContent in animationDictionary.Values)
            {
                // Store all keyframes of the animation
                List<Keyframe> keyframes = new List<Keyframe>();

                // Go through all animation channels (Each bone has it's own channel)
                foreach (string animationKey in animationContent.Channels.Keys)
                {
                    AnimationChannel animationChannel = animationContent.Channels[animationKey];
                    int boneIndex = boneNameList.IndexOf(animationKey);

                    //context.Logger.LogImportantMessage("{0} - Bone: ", animationKey, boneIndex);
                    foreach (AnimationKeyframe keyframe in animationChannel)
                        keyframes.Add(new Keyframe(keyframe.Time, boneIndex, keyframe.Transform));
                }

                context.Logger.LogImportantMessage("Animation {0}: {1} channels found, {2} keyframes found.",
                    animationContent.Name, animationContent.Channels.Count, keyframes.Count);

                // Sort all animation frames by time
                keyframes.Sort();

                animations[count++] = new
                    AnimationData(animationContent.Name, animationContent.Duration, keyframes.ToArray());
            }

            return animations;
        }

        protected override MaterialContent ConvertMaterial(MaterialContent material,
            ContentProcessorContext context)
        {
            BasicMaterialContent basicMaterial = material as BasicMaterialContent;
            if (basicMaterial == null)
                context.Logger.LogImportantMessage("This mesh doesn't have a valid basic material.");

            // Only process meshs with basic material
            // Otherwise the mesh must be using the correct shader (AnimatedModel.fx)
            if (basicMaterial != null)
            {
                EffectMaterialContent effectMaterial = new EffectMaterialContent();
                effectMaterial.Effect = new ExternalReference<EffectContent>(EFFECTS_PATH +
                    EFFECT_FILENAME);

                // Correct the texture path
                if (basicMaterial.Texture != null)
                {
                    string textureFileName = Path.GetFileName(basicMaterial.Texture.Filename);
                    effectMaterial.Textures.Add("diffuseTexture1",
                        new ExternalReference<TextureContent>(TEXTURES_PATH + textureFileName));
                }
                return base.ConvertMaterial(effectMaterial, context);
            }
            else
                return base.ConvertMaterial(material, context);
        }
    }
}