using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_TPS.GameBase.Effects
{
    public class AnimatedModelEffect
    {
        // List all the techniques present in this effect
        public enum Techniques
        {
            AnimatedModel = 0
        };

        // Effect file name
        public static string EFFECT_FILE_NAME = GameAssetsPath.EFFECTS_PATH + "AnimatedModel";

        // Shader effect
        Effect effect;

        // Store this matrices to help calculating auxiliar matrices (like World * View * Projection)
        Matrix worldMatrix;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        // Effect parameters (One entry for each parameter in the effect file)

        // Effect parameters - Matrices
        EffectParameter worldParam;
        EffectParameter viewParam;
        EffectParameter viewInverseParam;
        EffectParameter worldViewParam;
        EffectParameter worldViewProjectionParam;
        EffectParameter bonesParam;

        // Effect parameters - Material
        EffectParameter diffuseColorParam;
        EffectParameter specularColorParam;
        EffectParameter specularPowerParam;

        // Effect parameters - Lights
        EffectParameter ambientLightColorParam;
        EffectParameter light1PositionParam;
        EffectParameter light1ColorParam;
        EffectParameter light2PositionParam;
        EffectParameter light2ColorParam;

        #region Properties
        public AnimatedModelEffect.Techniques CurrentTechnique
        {
            set
            {
                effect.CurrentTechnique = effect.Techniques[(int)value];
            }
        }

        public EffectPassCollection CurrentTechniquePasses
        {
            get
            {
                return effect.CurrentTechnique.Passes;
            }
        }

        public Matrix World
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                worldParam.SetValue(worldMatrix);
                worldViewParam.SetValue(worldMatrix * viewMatrix);
                worldViewProjectionParam.SetValue(worldMatrix * viewMatrix * projectionMatrix);
            }
        }

        public Matrix View
        {
            get { return viewMatrix; }
            set
            {
                viewMatrix = value;
                Matrix viewInverseMatrix = Matrix.Invert(viewMatrix);
                viewInverseParam.SetValue(viewInverseMatrix);

                viewParam.SetValue(viewMatrix);
                worldViewParam.SetValue(worldMatrix * viewMatrix);
                worldViewProjectionParam.SetValue(worldMatrix * viewMatrix * projectionMatrix);
            }
        }

        public Matrix Projection
        {
            get { return projectionMatrix; }
            set
            {
                projectionMatrix = value;
                worldViewProjectionParam.SetValue(worldMatrix * viewMatrix * projectionMatrix);
            }
        }

        public Matrix[] Bones
        {
            set { bonesParam.SetValue(value); }
        }

        public Vector3 DiffuseColor
        {
            get { return diffuseColorParam.GetValueVector3(); }
            set { diffuseColorParam.SetValue(value); }
        }

        public Vector3 SpecularColor
        {
            get { return specularColorParam.GetValueVector3(); }
            set { specularColorParam.SetValue(value); }
        }

        public float SpecularPower
        {
            get { return specularPowerParam.GetValueSingle(); }
            set { specularPowerParam.SetValue(value); }
        }

        public Vector3 AmbientLightColor
        {
            get { return ambientLightColorParam.GetValueVector3(); }
            set { ambientLightColorParam.SetValue(value); }
        }

        public Vector3 Light1Position
        {
            get { return light1PositionParam.GetValueVector3(); }
            set { light1PositionParam.SetValue(value); }
        }

        public Vector3 Light1Color
        {
            get { return light1ColorParam.GetValueVector3(); }
            set { light1ColorParam.SetValue(value); }
        }

        public Vector3 Light2Position
        {
            get { return light2PositionParam.GetValueVector3(); }
            set { light2PositionParam.SetValue(value); }
        }

        public Vector3 Light2Color
        {
            get { return light2ColorParam.GetValueVector3(); }
            set { light2ColorParam.SetValue(value); }
        }
        #endregion

        public AnimatedModelEffect(Effect effect)
        {
            this.effect = effect;
            GetEffectParameters();
        }

        /// <summary>
        /// Get all effects parameters by name
        /// </summary>
        private void GetEffectParameters()
        {
            // Matrices
            worldParam = effect.Parameters["matW"];
            viewParam = effect.Parameters["matV"];
            viewInverseParam = effect.Parameters["matVI"];
            worldViewParam = effect.Parameters["matWV"];
            worldViewProjectionParam = effect.Parameters["matWVP"];
            bonesParam = effect.Parameters["matBones"];

            // Material
            diffuseColorParam = effect.Parameters["diffuseColor"];
            specularColorParam = effect.Parameters["specularColor"];
            specularPowerParam = effect.Parameters["specularPower"];

            // Lights
            ambientLightColorParam = effect.Parameters["ambientLightColor"];
            light1PositionParam = effect.Parameters["light1Position"];
            light1ColorParam = effect.Parameters["light1Color"];
            light2PositionParam = effect.Parameters["light2Position"];
            light2ColorParam = effect.Parameters["light2Color"];
        }

        /// <summary>
        /// Begin effect
        /// </summary>
        public void Begin()
        {
            effect.Begin();
        }

        /// <summary>
        /// End effect
        /// </summary>
        public void End()
        {
            effect.End();
        }
    }
}
