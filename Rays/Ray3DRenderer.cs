using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Rays;
public class Ray3DRenderer : GraphicsDeviceRenderer
{
    protected List<VertexPositionTexture> VertexBuffer = new();
    protected List<int> IndexBuffer = new();
    protected int PrimitiveCount;

    public Ray3DRenderer(GraphicsDevice graphics, BasicEffect effect) : base(graphics, effect)
    {
    }

    public override void Begin(Matrix camMatrix)
    {
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        VertexBuffer.Clear();
        IndexBuffer.Clear();
        PrimitiveCount = 0;

        var map = RayGame.Instance.RayMap;
        var wallLayer = map.WallLayer;

        for (var x = 0; x < wallLayer.Width; x++)
            for (var y = 0; y < wallLayer.Height; y++)
            {
                var wall = wallLayer.GetTileValue(x, y);
                if (wall < 0)
                {
                    CreateWall(x, y, 54, 5);
                    continue;
                }
                CreateWall(x, y, wall, 0);
                CreateWall(x, y, wall, 1);
                CreateWall(x, y, wall, 2);
                CreateWall(x, y, wall, 3);
                CreateWall(x, y, 0, 4);
                CreateWall(x, y, 0, 5);
            }
    }

    private void CreateWall(int x, int y, int wall, int direction)
    {
        x *= 16;
        y *= 16;

        var vectors = new Vector3[4];

        switch (direction)
        {
            case 0:
                vectors[0] = new Vector3(x, y, 0);
                vectors[1] = new Vector3(x, y, 16);
                vectors[2] = new Vector3(x + 16, y, 16);
                vectors[3] = new Vector3(x + 16, y, 0);
                break;
            case 1:
                vectors[0] = new Vector3(x + 16, y, 0);
                vectors[1] = new Vector3(x + 16, y, 16);
                vectors[2] = new Vector3(x + 16, y + 16, 16);
                vectors[3] = new Vector3(x + 16, y + 16, 0);
                break;
            case 2:
                vectors[0] = new Vector3(x + 16, y + 16, 0);
                vectors[1] = new Vector3(x + 16, y + 16, 16);
                vectors[2] = new Vector3(x, y + 16, 16);
                vectors[3] = new Vector3(x, y + 16, 0);
                break;
            case 3:
                vectors[0] = new Vector3(x, y, 0);
                vectors[1] = new Vector3(x, y, 16);
                vectors[2] = new Vector3(x, y + 16, 16);
                vectors[3] = new Vector3(x, y + 16, 0);
                break;
            case 4:
                vectors[0] = new Vector3(x, y, 0);
                vectors[1] = new Vector3(x + 16, y, 0);
                vectors[2] = new Vector3(x + 16, y + 16, 0);
                vectors[3] = new Vector3(x, y + 16, 0);
                break;
            case 5:
                vectors[0] = new Vector3(x, y, 16);
                vectors[1] = new Vector3(x + 16, y, 16);
                vectors[2] = new Vector3(x + 16, y + 16, 16);
                vectors[3] = new Vector3(x, y + 16, 16);
                break;
        }

        var currIndex = VertexBuffer.Count;
        var xStart = (wall * 16) / 944f;
        var xEnd = ((wall + 1) * 16) / 944f;
        
        VertexBuffer.Add(new VertexPositionTexture(vectors[0], new(xStart, 0)));
        VertexBuffer.Add(new VertexPositionTexture(vectors[1], new(xStart, .5f)));
        VertexBuffer.Add(new VertexPositionTexture(vectors[2], new(xEnd, .5f)));
        VertexBuffer.Add(new VertexPositionTexture(vectors[3], new(xEnd, 0)));

        IndexBuffer.Add(currIndex + 0);
        IndexBuffer.Add(currIndex + 1);
        IndexBuffer.Add(currIndex + 2);
        PrimitiveCount++;

        IndexBuffer.Add(currIndex + 2);
        IndexBuffer.Add(currIndex + 3);
        IndexBuffer.Add(currIndex + 0);
        PrimitiveCount++;
    }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer, Vector2 drawOffset)
    {
        if (VertexBuffer == null || VertexBuffer.Count == 0)
            return;

        var rayGame = RayGame.Instance;

        Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
        var matrix = Matrix.CreateFromYawPitchRoll(rayGame.CamTarget.X, rayGame.CamTarget.Y, rayGame.CamTarget.Z);
        Effect.View = Matrix.CreateLookAt(rayGame.CamPosition + matrix.Forward, rayGame.CamPosition + matrix.Up, Vector3.Up);
        Effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

        //Effect.Projection = rayGame.Camera.Projection;
        //Effect.View = rayGame.Camera.View;
        //Effect.World = rayGame.Camera.World;

        GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };

        foreach (var pass in Effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VertexBuffer.ToArray(), 0, VertexBuffer.Count, IndexBuffer.ToArray(), 0, PrimitiveCount);
        }
    }
}
