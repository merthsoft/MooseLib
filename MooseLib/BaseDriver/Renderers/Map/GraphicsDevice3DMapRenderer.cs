using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
public abstract class GraphicsDevice3DMapRenderer : GraphicsDeviceMapRenderer
{
    protected List<VertexPositionColorTexture> VertexBuffer = new();
    protected List<int> IndexBuffer = new();
    protected int PrimitiveCount;

    public Vector3 CameraPosition { get; set; }
    public Vector3 CameraFacing { get; set; }

    protected GraphicsDevice3DMapRenderer(GraphicsDevice graphicsDevice, BasicEffect effect) : base(graphicsDevice, effect)
    {

    }

    public override abstract void Update(MooseGame game, GameTime gameTime, IMap map);

    public override void Draw(MooseGame game, GameTime gameTime, IMap map)
    {
        if (VertexBuffer == null || VertexBuffer.Count == 0)
            return;

        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(100), .85f, 1f, 1000f);
        Effect.View = Matrix.CreateLookAt(CameraPosition, CameraPosition + CameraFacing, Vector3.Forward);
        Effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

        GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };

        foreach (var pass in Effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VertexBuffer.ToArray(), 0, VertexBuffer.Count, IndexBuffer.ToArray(), 0, PrimitiveCount);
        }
    }
}
