using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
public abstract class GraphicsDevice3DMapRenderer : GraphicsDeviceMapRenderer
{
    protected VertexPositionColorTexture[] VertexBuffer;
    protected int[] IndexBuffer;
    protected int PrimitiveCount;

    protected int VertexBufferIndex = 0;
    protected int IndexBufferIndex = 0;

    public Vector3 CameraPosition { get; set; }
    public Vector3 CameraFacing { get; set; }

    protected GraphicsDevice3DMapRenderer(GraphicsDevice graphicsDevice, BasicEffect effect, int initialPrimitiveCount = 1_000_000) : base(graphicsDevice, effect)
    {
        VertexBuffer = new VertexPositionColorTexture[initialPrimitiveCount];
        IndexBuffer = new int[initialPrimitiveCount];
    }

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
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VertexBuffer, 0, VertexBufferIndex, IndexBuffer, 0, PrimitiveCount);
        }
    }

    protected virtual void SetVertexAndIncrementBufferIndex(Vector3 vector, Color c, float textureX, float textureY)
    {
        VertexBuffer[VertexBufferIndex].Position.X = vector.X; 
        VertexBuffer[VertexBufferIndex].Position.Y = vector.Y;
        VertexBuffer[VertexBufferIndex].Position.Z = vector.Z;
        VertexBuffer[VertexBufferIndex].Color = c;
        VertexBuffer[VertexBufferIndex].TextureCoordinate.X = textureX;
        VertexBuffer[VertexBufferIndex].TextureCoordinate.Y = textureY;
        VertexBufferIndex++;
    }

    protected virtual void InsertRectangle(Vector3[] vectors, float xStart, float xEnd, int yStart, int yEnd, Color c)
    {
        var currIndex = VertexBufferIndex;
        SetVertexAndIncrementBufferIndex(vectors[0], c, xStart, yStart);
        SetVertexAndIncrementBufferIndex(vectors[1], c, xStart, yEnd);

        IndexBuffer[IndexBufferIndex++] = currIndex + 0;
        IndexBuffer[IndexBufferIndex++] = currIndex + 1;
        IndexBuffer[IndexBufferIndex++] = currIndex + 2;
        PrimitiveCount++;

        SetVertexAndIncrementBufferIndex(vectors[2], c, xEnd, yEnd);
        SetVertexAndIncrementBufferIndex(vectors[3], c, xEnd, yStart);

        IndexBuffer[IndexBufferIndex++] = currIndex + 2;
        IndexBuffer[IndexBufferIndex++] = currIndex + 3;
        IndexBuffer[IndexBufferIndex++] = currIndex + 0;
        PrimitiveCount++;
    }
}
