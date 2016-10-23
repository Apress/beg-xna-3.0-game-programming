using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; // BasicEffect, Vertex*, GraphicsDevice
using Microsoft.Xna.Framework;  // Matrix

namespace XNADemo
{
class cls3DAxis
{
private GraphicsDevice device;
    
// Vertex buffer
private VertexBuffer vertexBuffer;

private BasicEffect effect;
    
public Matrix worldMatrix = Matrix.Identity;

public cls3DAxis(GraphicsDevice graphicsDevice)
{
    device = graphicsDevice;
}

public void UnloadContent()
{
    if (vertexBuffer != null)
    {
        vertexBuffer.Dispose();
        vertexBuffer = null;
    }

    if (effect != null)
    {
        effect.Dispose();
        effect = null;
    }
}

public void LoadContent()
{ 
    //  Create the effect that will be used to draw the axis
    effect = new BasicEffect(device, null);

    //  Calculates the effect aspect ratio, projection and view matrix
    float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
    effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 2.0f, 2.0f), Vector3.Zero,
                          Vector3.Up); 
    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                       MathHelper.ToRadians(45.0f),
                       aspectRatio, 1.0f, 10.0f);
    effect.LightingEnabled = false;

    //  Create the 3D axis 
    Create3DAxis();
}

private void Create3DAxis()
{
    //  size of 3D Axis 
    float axisLength = 1f;
    //  Number of vertices we´ll use
    int vertexCount = 22;

    VertexPositionColor[] vertices = new VertexPositionColor[vertexCount];
    // X axis
    vertices[0] = new VertexPositionColor(new Vector3(-axisLength, 0.0f, 0.0f), Color.White);
    vertices[1] = new VertexPositionColor(new Vector3(axisLength, 0.0f, 0.0f), Color.White);
    // Y axis
    vertices[2] = new VertexPositionColor(new Vector3(0.0f, -axisLength, 0.0f), Color.White);
    vertices[3] = new VertexPositionColor(new Vector3(0.0f, axisLength, 0.0f), Color.White);
    // Z axis
    vertices[4] = new VertexPositionColor(new Vector3(0.0f, 0.0f, -axisLength), Color.White);
    vertices[5] = new VertexPositionColor(new Vector3(0.0f, 0.0f, axisLength), Color.White);

    //  "X" letter near X Axis
    vertices[6] = new VertexPositionColor(new Vector3(axisLength - 0.1f, 0.05f, 0.0f), Color.White);
    vertices[7] = new VertexPositionColor(new Vector3(axisLength - 0.05f, 0.2f, 0.0f), Color.White);
    vertices[8] = new VertexPositionColor(new Vector3(axisLength - 0.05f, 0.05f, 0.0f), Color.White);
    vertices[9] = new VertexPositionColor(new Vector3(axisLength - 0.1f, 0.2f, 0.0f), Color.White);

    //  "Y" letter near Y Axis
    vertices[10] = new VertexPositionColor(new Vector3(0.075f, axisLength - 0.125f, 0.0f), Color.White);
    vertices[11] = new VertexPositionColor(new Vector3(0.075f, axisLength - 0.2f, 0.0f), Color.White);
    vertices[12] = new VertexPositionColor(new Vector3(0.075f, axisLength - 0.125f, 0.0f), Color.White);
    vertices[13] = new VertexPositionColor(new Vector3(0.1f, axisLength - 0.05f, 0.0f), Color.White);
    vertices[14] = new VertexPositionColor(new Vector3(0.075f, axisLength - 0.125f, 0.0f), Color.White);
    vertices[15] = new VertexPositionColor(new Vector3(0.05f, axisLength - 0.05f, 0.0f), Color.White);

    //  "Z" letter near Z Axis
    vertices[16] = new VertexPositionColor(new Vector3(0.0f, 0.05f, axisLength - 0.1f), Color.White);
    vertices[17] = new VertexPositionColor(new Vector3(0.0f, 0.05f, axisLength - 0.05f), Color.White);
    vertices[18] = new VertexPositionColor(new Vector3(0.0f, 0.05f, axisLength - 0.1f), Color.White);
    vertices[19] = new VertexPositionColor(new Vector3(0.0f, 0.2f, axisLength - 0.05f), Color.White);
    vertices[20] = new VertexPositionColor(new Vector3(0.0f, 0.2f, axisLength - 0.1f), Color.White);
    vertices[21] = new VertexPositionColor(new Vector3(0.0f, 0.2f, axisLength - 0.05f), Color.White);

    //  fill the vertex buffer with the vertices
    vertexBuffer = new VertexBuffer(device, vertexCount * VertexPositionColor.SizeInBytes, 
                                    BufferUsage.WriteOnly);
    vertexBuffer.SetData<VertexPositionColor>(vertices);
}

public void Draw()
{
    //  Set the World Matrix
    effect.World = worldMatrix;

    //  Create a vertex declaration to be used when drawing the vertices
    device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
    //  Set the vertex source
    device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
    
   //  Draw the 3D axis
    effect.Begin();
    foreach(EffectPass CurrentPass in effect.CurrentTechnique.Passes)
    {
       CurrentPass.Begin();
       //  We are drawing 22 vertices, grouped in 11 lines
       device.DrawPrimitives(PrimitiveType.LineList, 0, 11);
       CurrentPass.End();
    }
    effect.End();
}
}
}
