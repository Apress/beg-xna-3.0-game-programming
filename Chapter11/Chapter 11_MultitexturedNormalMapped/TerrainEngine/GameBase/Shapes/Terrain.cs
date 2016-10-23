using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TerrainEngine.GameBase.Cameras;
using TerrainEngine.GameBase.Effects;
using TerrainEngine.Helpers;
using TerrainEngine.GameBase.Lights;
using TerrainEngine.GameBase.Materials;

namespace TerrainEngine.GameBase.Shapes
{
    public class Terrain : DrawableGameComponent
    {
        public static float INTERSECT_EPSILON = 0.01f;

        Color[] heightMap;
        VertexBuffer vb;
        IndexBuffer ib;
        int numVertices;
        int numTriangles;

        int vertexCountX;
        int vertexCountZ;
        float blockScale;
        float heightScale;

        Transformation transformation;
        VertexDeclaration vertexDeclaration;

        TerrainEffect effect;
        TerrainMaterial terrainMaterial;

        // Necessary services
        bool isInitialized;
        CameraManager cameraManager;
        LightManager lightManager;

        #region Properties
        public Vector2 StartPosition
        {
            get
            {
                float terrainHalfWidth = (vertexCountX - 1) * blockScale * 0.5f;
                float terrainHalfDepth = (vertexCountZ - 1) * blockScale * 0.5f;

                return new Vector2(-terrainHalfWidth, -terrainHalfDepth);
            }
        }

        public Vector2 EndPosition
        {
            get
            {
                float terrainHalfWidth = (vertexCountX - 1) * blockScale * 0.5f;
                float terrainHalfDepth = (vertexCountZ - 1) * blockScale * 0.5f;

                return new Vector2(terrainHalfWidth, terrainHalfDepth);
            }
        }

        public int VertexCountX
        {
            get
            {
                return vertexCountX;
            }
        }

        public int VertexCountZ
        {
            get
            {
                return vertexCountZ;
            }
        }

        public float BlockScale
        {
            get
            {
                return blockScale;
            }
        }

        public float HeightScale
        {
            get
            {
                return heightScale;
            }
        }

        public TerrainEffect Effect
        {
            get
            {
                return effect;
            }
            set
            {
                effect = value;
            }
        }

        public TerrainMaterial Material
        {
            get
            {
                return terrainMaterial;
            }
            set
            {
                terrainMaterial = value;
            }
        }

        public Transformation Transformation
        {
            get
            {
                return transformation;
            }
            set
            {
                transformation = value;
            }
        }
        #endregion

        public Terrain(Game game)
            : base(game)
        {
            isInitialized = false;
        }

        public override void Initialize()
        {
            cameraManager = Game.Services.GetService(typeof(CameraManager)) as CameraManager;
            lightManager = Game.Services.GetService(typeof(LightManager)) as LightManager;

            if (cameraManager == null || lightManager == null)
                throw new InvalidOperationException();

            isInitialized = true;

            base.Initialize();
        }

        public void Load(ContentManager Content, string heightMapFileName, float blockScale, float heightScale)
        {
            if (!isInitialized)
                Initialize();
            
            // Load heightMap file
            Texture2D heightMapTexture = Content.Load<Texture2D>(heightMapFileName);
            int heightMapSize = heightMapTexture.Width*heightMapTexture.Height;
            heightMap = new Color[heightMapSize];
            heightMapTexture.GetData<Color>(heightMap);
            
            this.vertexCountX = heightMapTexture.Width;
            this.vertexCountZ = heightMapTexture.Height;
            this.blockScale = blockScale;
            this.heightScale = heightScale;            

            // Generate terrain mesh
            GenerateTerrainMesh();
            transformation = new Transformation();

            // Load effect
            effect = new TerrainEffect(Game.Content.Load<Effect>(TerrainEffect.EFFECT_FILENAME));
            terrainMaterial = new TerrainMaterial();

            // Load vertex declaration once
            this.vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTangentBinormalTexture.VertexElements);
        }

        private void GenerateTerrainMesh()
        {
            numVertices = vertexCountX * vertexCountZ;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;

            // We must generate the terrain indices first
            int[] indices = GenerateTerrainIndices();

            // Generate terrain vertices
            VertexPositionNormalTangentBinormalTexture[] vertices = GenerateTerrainVertices(indices);

            // Create vertex and index buffers
            vb = new VertexBuffer(GraphicsDevice, numVertices * VertexPositionNormalTangentBinormalTexture.SizeInBytes,
                BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTangentBinormalTexture>(vertices);

            ib = new IndexBuffer(GraphicsDevice, numTriangles * 3 * sizeof(int), BufferUsage.WriteOnly,
                IndexElementSize.ThirtyTwoBits);
            ib.SetData<int>(indices);

        }

        /**
         * Generate terrain mesh indices
        */
        private int[] GenerateTerrainIndices()
        {
            int numIndices = numTriangles * 3;
            int[] indices = new int[numIndices];

            int indicesCount = 0;
            for (int i = 0; i < (vertexCountZ - 1); i++)
            {
                for (int j = 0; j < (vertexCountX - 1); j++)
                {
                    int index = j + i * vertexCountZ;
                    indices[indicesCount++] = index;
                    indices[indicesCount++] = index + 1;
                    indices[indicesCount++] = index + vertexCountX + 1;

                    indices[indicesCount++] = index + vertexCountX + 1;
                    indices[indicesCount++] = index + vertexCountX;
                    indices[indicesCount++] = index;

                }
            }

            return indices;
        }

        /**
         * Generate terrain mesh vertices and indices
         * The vertex is currently VertexPositionNormalTangentBinormalTexture
         */
        private VertexPositionNormalTangentBinormalTexture[] GenerateTerrainVertices(int[] terrainIndices)
        {
            float halfTerrainWidth = (vertexCountX - 1) * blockScale * 0.5f;
            float halfTerrainDepth = (vertexCountZ - 1) * blockScale * 0.5f;

            // Texture coordinates
            float tu = 0;
            float tv = 0;
            float tuDerivative = 1.0f / (vertexCountX - 1);
            float tvDerivative = 1.0f / (vertexCountZ - 1);

            int vertexCount = 0;
            VertexPositionNormalTangentBinormalTexture[] vertices =
                new VertexPositionNormalTangentBinormalTexture[vertexCountX * vertexCountZ];

            // Set vertices position and texture coordinate
            for (float i = -halfTerrainDepth; i <= halfTerrainDepth; i += blockScale)
            {
                tu = 0.0f;
                for (float j = -halfTerrainWidth; j <= halfTerrainWidth; j += blockScale)
                {
                    vertices[vertexCount].Position = new Vector3(j, heightMap[vertexCount].R * heightScale, i);
                    vertices[vertexCount].TextureCoordinate = new Vector2(tu, tv);

                    tu += tuDerivative;
                    vertexCount++;
                }

                tv += tvDerivative;
            }

            // Generate vertice's normal, tangent and binormal
            GenerateTerrainNormals(vertices, terrainIndices);
            GenerateTerrainTangentBinormal(vertices, terrainIndices);

            return vertices;
        }

        private void GenerateTerrainNormals(VertexPositionNormalTangentBinormalTexture[] vertices, int[] indices)
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 v1 = vertices[indices[i]].Position;
                Vector3 v2 = vertices[indices[i + 1]].Position;
                Vector3 v3 = vertices[indices[i + 2]].Position;

                Vector3 vu = v3 - v1;
                Vector3 vt = v2 - v1;
                Vector3 normal = Vector3.Cross(vu, vt);
                normal.Normalize();

                vertices[indices[i]].Normal += normal;
                vertices[indices[i + 1]].Normal += normal;
                vertices[indices[i + 2]].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
        }

        public void GenerateTerrainTangentBinormal(VertexPositionNormalTangentBinormalTexture[] vertices, int[] indices)
        {
            for (int i = 0; i < vertexCountZ; i++)
            {
                for (int j = 0; j < vertexCountX; j++)
                {
                    int vertexIndex = j + i * vertexCountX;
                    Vector3 v1 = vertices[vertexIndex].Position;

                    if (j < vertexCountX - 1)
                    {
                        Vector3 v2 = vertices[vertexIndex + 1].Position;
                        vertices[vertexIndex].Tanget = (v2 - v1);
                    }
                    else
                    {
                        Vector3 v2 = vertices[vertexIndex - 1].Position;
                        vertices[vertexIndex].Tanget = (v1 - v2);
                    }

                    vertices[vertexIndex].Tanget.Normalize();
                    vertices[vertexIndex].Binormal = Vector3.Cross(vertices[vertexIndex].Tanget, vertices[vertexIndex].Normal);
                }
            }
        }

        public float GetHeight(Vector2 position)
        {
            return GetHeight(position.X, position.Y);
        }

        public float GetHeight(Vector3 position)
        {
            return GetHeight(position.X, position.Z);
        }

        private float GetHeight(float positionX, float positionZ)
        {
            float height = -999999.0f;
            if (heightMap == null) return height;

            // Get the position of the object in the terrain grid
            Vector2 positionInGrid = new Vector2(
                positionX - (StartPosition.X + Transformation.Translate.X),
                positionZ - (StartPosition.Y + Transformation.Translate.Z));

            // Calculate the start block position
            Vector2 blockPosition = new Vector2(
                positionInGrid.X / blockScale,
                positionInGrid.Y / blockScale);

            // Check if the object is inside the grid
            if (blockPosition.X >= 0 && blockPosition.X < (vertexCountX - 1) &&
                blockPosition.Y >= 0 && blockPosition.Y < (vertexCountZ - 1))
            {
                Vector2 blockOffset = new Vector2(
                    blockPosition.X - (int)blockPosition.X,
                    blockPosition.Y - (int)blockPosition.Y);

                // Get the height of the four vertices of the grid block
                int vertexIndex = (int)blockPosition.X + (int)blockPosition.Y * vertexCountX;
                float height1 = heightMap[vertexIndex + 1].R;
                float height2 = heightMap[vertexIndex].R;
                float height3 = heightMap[vertexIndex + vertexCountX + 1].R;
                float height4 = heightMap[vertexIndex + vertexCountX].R;
                
                // Bottom triangle
                float heightIncX, heightIncY;
                if (blockOffset.X > blockOffset.Y)
                {
                    heightIncX = height1 - height2;
                    heightIncY = height3 - height1;
                }
                // Top triangle
                else
                {
                    heightIncX = height3 - height4;
                    heightIncY = height4 - height2;
                }

                // Linear interpolation to find the height inside the triangle
                float lerpHeight = height2 + heightIncX * blockOffset.X + heightIncY * blockOffset.Y;
                height = lerpHeight * heightScale;
            }

            return height;
        }

        public float? Intersects(Ray ray)
        {
            float? collisionDistance = null;
            Vector3 rayStep = ray.Direction * blockScale * 0.5f;
            Vector3 rayStartPosition = ray.Position;

            // Linear search. Loop until find a point inside the terrain
            Vector3 lastRayPosition = ray.Position;
            ray.Position += rayStep;
            float height = GetHeight(ray.Position);
            while (ray.Position.Y > height && height >= 0)
            {
                lastRayPosition = ray.Position;
                ray.Position += rayStep;
                height = GetHeight(ray.Position);
            }

            // If the ray collide with the terrain
            if (height >= 0)
            {
                Vector3 startPosition = lastRayPosition;
                Vector3 endPosition = ray.Position;
                // Binary search. Find the exact collision point
                for (int i = 0; i < 32; i++)
                {
                    // Bynary search pass
                    Vector3 middlePoint = (startPosition + endPosition) * 0.5f;
                    if (middlePoint.Y < height)
                        endPosition = middlePoint;
                    else
                        startPosition = middlePoint;
                }
                Vector3 collisionPoint = (startPosition + endPosition) * 0.5f;
                collisionDistance = Vector3.Distance(rayStartPosition, collisionPoint);
            }
            return collisionDistance;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void SetEffectMaterial()
        {
            // Get the first two lights from the light manager
            PointLight light0 = PointLight.NoLight;
            PointLight light1 = PointLight.NoLight;
            if (lightManager.Count > 0)
            {
                light0 = lightManager[0] as PointLight;
                if (lightManager.Count > 1)
                    light1 = lightManager[1] as PointLight;
            }
            
            // Lights
            effect.AmbientLightColor = lightManager.AmbientLightColor;
            effect.Light1Position = light0.Position;
            effect.Light1Color = light0.Color;
            effect.Light2Position = light1.Position;
            effect.Light2Color = light1.Color;

            // Material
            effect.DiffuseColor = terrainMaterial.LightMaterial.DiffuseColor;
            effect.SpecularColor = terrainMaterial.LightMaterial.SpecularColor;
            effect.SpecularPower = terrainMaterial.LightMaterial.SpecularPower;
            // Textures
            effect.DiffuseTexture1 = terrainMaterial.DiffuseTexture1.Texture;
            effect.DiffuseTexture2 = terrainMaterial.DiffuseTexture2.Texture;
            effect.DiffuseTexture3 = terrainMaterial.DiffuseTexture3.Texture;
            effect.DiffuseTexture4 = terrainMaterial.DiffuseTexture4.Texture;
            effect.NormalMapTexture = terrainMaterial.NormalMapTexture.Texture;
            effect.AlphaMapTexture = terrainMaterial.AlphaMapTexture.Texture;
            // Texturas UVs
            effect.TextureUV1Tile = terrainMaterial.DiffuseTexture1.UVTile;
            effect.TextureUV2Tile = terrainMaterial.DiffuseTexture2.UVTile;
            effect.TextureUV3Tile = terrainMaterial.DiffuseTexture3.UVTile;
            effect.TextureUV4Tile = terrainMaterial.DiffuseTexture4.UVTile;
            effect.TextureUVBumpTile = terrainMaterial.NormalMapTexture.UVTile;
            // Camera and world transformations
            effect.World = transformation.Matrix;
            effect.View = cameraManager.ActiveCamera.View;
            effect.Projection = cameraManager.ActiveCamera.Projection;
        }

        public override void Draw(GameTime time)
        {
            SetEffectMaterial();

            // Set mesh vertex and index
            GraphicsDevice.VertexDeclaration = this.vertexDeclaration;
            GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexPositionNormalTangentBinormalTexture.SizeInBytes);
            GraphicsDevice.Indices = ib;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechniquePasses)
            {
                pass.Begin();
                // Draw the mesh
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);
                pass.End();
            }
            effect.End();
        }
    }
}
