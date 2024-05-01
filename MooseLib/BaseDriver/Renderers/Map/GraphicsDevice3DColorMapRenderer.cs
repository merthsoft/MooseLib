using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
public abstract class GraphicsDevice3DColorMapRenderer(
    GraphicsDevice graphicsDevice, 
    BasicEffect effect, 
    int initialPrimitiveCount = 10_000_000) 
    : GraphicsDeviceMapRenderer(graphicsDevice, effect)
{
    protected PrimitiveType PrimitiveType = PrimitiveType.TriangleList;
    protected VertexPositionColor[] VertexBuffer = new VertexPositionColor[initialPrimitiveCount];
    protected int[] IndexBuffer = new int[initialPrimitiveCount];
    protected int PrimitiveCount;

    protected int VertexBufferIndex = 0;
    protected int IndexBufferIndex = 0;

    public Vector3 CameraPosition { get; set; }
    public Vector3 CameraFacing { get; set; }

    public override abstract void Update(MooseGame game, GameTime gameTime, IMap map);

    public override void Draw(MooseGame game, GameTime gameTime, IMap map)
    {
        if (VertexBuffer == null || VertexBufferIndex == 0)
            return;

        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };

        foreach (var pass in Effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType, VertexBuffer, 0, VertexBufferIndex, IndexBuffer, 0, PrimitiveCount);
        }
    }

    protected virtual void PushVertex(Vector3 vector, Color c)
    {
        VertexBuffer[VertexBufferIndex].Position.X = vector.X; 
        VertexBuffer[VertexBufferIndex].Position.Y = vector.Y;
        VertexBuffer[VertexBufferIndex].Position.Z = vector.Z;
        VertexBuffer[VertexBufferIndex].Color = c;
        VertexBufferIndex++;
    }

    protected virtual void AddTri(Vector3[] vectors, Color c)
    {
        var currIndex = VertexBufferIndex;
        PushVertex(vectors[0], c);
        PushVertex(vectors[1], c);
        PushVertex(vectors[2], c);

        IndexBuffer[IndexBufferIndex++] = currIndex + 0;
        IndexBuffer[IndexBufferIndex++] = currIndex + 1;
        IndexBuffer[IndexBufferIndex++] = currIndex + 2;
        PrimitiveCount++;
    }

    protected virtual void AddQuad(Vector3[] vectors, Color c)
    {
        var currIndex = VertexBufferIndex;
        PushVertex(vectors[0], c);
        PushVertex(vectors[1], c);
        PushVertex(vectors[2], c);
        PushVertex(vectors[3], c);

        IndexBuffer[IndexBufferIndex++] = currIndex + 0;
        IndexBuffer[IndexBufferIndex++] = currIndex + 1;
        IndexBuffer[IndexBufferIndex++] = currIndex + 2;
        PrimitiveCount++;

        IndexBuffer[IndexBufferIndex++] = currIndex + 2;
        IndexBuffer[IndexBufferIndex++] = currIndex + 3;
        IndexBuffer[IndexBufferIndex++] = currIndex + 0;
        PrimitiveCount++;
    }

    protected virtual void AddQuad(Vector3[] vectors, Color[] colors)
    {
        var currIndex = VertexBufferIndex;
        PushVertex(vectors[0], colors[0]);
        PushVertex(vectors[1], colors[1]);
        PushVertex(vectors[2], colors[2]);
        PushVertex(vectors[3], colors[3]);

        IndexBuffer[IndexBufferIndex++] = currIndex + 0;
        IndexBuffer[IndexBufferIndex++] = currIndex + 1;
        IndexBuffer[IndexBufferIndex++] = currIndex + 2;
        PrimitiveCount++;

        IndexBuffer[IndexBufferIndex++] = currIndex + 2;
        IndexBuffer[IndexBufferIndex++] = currIndex + 3;
        IndexBuffer[IndexBufferIndex++] = currIndex + 0;
        PrimitiveCount++;
    }
}
