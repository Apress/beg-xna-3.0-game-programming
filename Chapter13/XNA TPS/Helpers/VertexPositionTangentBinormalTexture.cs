using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_TPS.Helpers
{
    public struct VertexPositionNormalTangentBinormalTexture
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector3 Tanget;
        public Vector3 Binormal;        

        public static readonly VertexElement[] VertexElements = new VertexElement[] 
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
            new VertexElement(0, 24, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, 32, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Tangent, 0),
            new VertexElement(0, 44, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Binormal, 0)
        };
        public static readonly int SizeInBytes = sizeof(float) * (3 + 3 + 2 + 3 + 3);
    }
}
