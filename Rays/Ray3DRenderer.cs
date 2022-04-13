using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Rays;
public class Ray3DRenderer : GraphicsDeviceRenderer
{
    protected List<VertexPositionColorTexture> VertexBuffer = new();
    protected List<int> IndexBuffer = new();
    protected int PrimitiveCount;

    protected float TextureWidth;
    protected float TextureHeight;

    protected RayPlayer Player = null!;

    public Ray3DRenderer(GraphicsDevice graphics, BasicEffect effect) : base(graphics, effect)
    {
        TextureWidth = effect.Texture.Width;
        TextureHeight = effect.Texture.Height;
    }

    public override void Begin(Matrix camMatrix)
    {
        
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        Player ??= RayGame.Instance.Player;

        VertexBuffer.Clear();
        IndexBuffer.Clear();
        PrimitiveCount = 0;

        var map = RayGame.Instance.RayMap;
        var floorLayer = map.FloorLayer;
        var ceilingLayer = map.CeilingLayer;
        var wallLayer = map.WallLayer;
        var objectLayer = map.ObjectLayer;

        for (var x = -1; x <= wallLayer.Width; x++)
            for (var y = -1; y <= wallLayer.Height; y++)
            {
                var wall = wallLayer.GetTileValue(x, y);
                if (wall < 0)
                {
                    var floor = floorLayer.GetTileValue(x, y);
                    var ceiling = ceilingLayer.GetTileValue(x, y);
                    CreateWall(x, y, ceiling, 4);
                    CreateWall(x, y, ceiling, 5);
                    continue;
                }
                CreateWall(x, y, wall, 0);
                CreateWall(x, y, wall, 1);
                CreateWall(x, y, wall, 2);
                CreateWall(x, y, wall, 3);
                CreateWall(x, y, 0, 4);
                CreateWall(x, y, 0, 5);
            }

        foreach (var obj in objectLayer.Objects.Cast<RayGameObject>())
        {
            if (obj is RayPlayer)
                continue;

            var (x, y) = obj.Position;
            switch (obj.RayGameObjectDef.RenderMode)
            {
                case ObjectRenderMode.Sprite:
                    CreateSprite(x, y, obj);
                    break;
            }
        }
    }

    private void CreateSprite(float x, float y, RayGameObject obj)
    {
        var textureIndex = obj.TextureIndex;

        var vectors = new Vector3[4];
        vectors[0] = new Vector3(-4, 0, obj.RayGameObjectDef.RenderBottom);
        vectors[1] = new Vector3(-4, 0, obj.RayGameObjectDef.RenderTop);
        vectors[2] = new Vector3(+4, 0, obj.RayGameObjectDef.RenderTop);
        vectors[3] = new Vector3(+4, 0, obj.RayGameObjectDef.RenderBottom); 
        
        var currIndex = VertexBuffer.Count;
        var xStart = (textureIndex * 16) / TextureWidth;
        var xEnd = ((textureIndex + 1) * 16) / TextureWidth;
        var yStart = .5f;
        var yEnd = .75f;

        var radians = MathF.Atan2(obj.Position.Y - Player.Position.Y, obj.Position.X - Player.Position.X);
        var rot = Matrix.CreateRotationZ(MathF.PI / 2 + radians);
        for (var i = 0; i < vectors.Length; i++)
            vectors[i] = Vector3.Transform(vectors[i], rot) + new Vector3(x, y, 0);

        VertexBuffer.Add(new VertexPositionColorTexture(vectors[0], Color.White, new(xStart, yStart)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[1], Color.White, new(xStart, yEnd)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[2], Color.White, new(xEnd, yEnd)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[3], Color.White, new(xEnd, yStart)));

        IndexBuffer.Add(currIndex + 0);
        IndexBuffer.Add(currIndex + 1);
        IndexBuffer.Add(currIndex + 2);
        PrimitiveCount++;

        IndexBuffer.Add(currIndex + 2);
        IndexBuffer.Add(currIndex + 3);
        IndexBuffer.Add(currIndex + 0);
        PrimitiveCount++;
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
        var xStart = (wall * 16) / TextureWidth;
        var xEnd = ((wall + 1) * 16) / TextureWidth;
        var yStart = 0;
        var yEnd = .25f;

        var color = direction is 1 or 3 or 5 ? new Color(170, 170, 170) : Color.White;
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[0], color, new(xStart, yStart)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[1], color, new(xStart, yEnd)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[2], color, new(xEnd, yEnd)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[3], color, new(xEnd, yStart)));

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

        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(100), 1, 1f, 1000f);
        Effect.View = Matrix.CreateLookAt(
            RayPlayer.Instance.PositionIn3dSpace, 
            RayPlayer.Instance.PositionIn3dSpace + RayPlayer.Instance.FacingDirection, Vector3.Forward);
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
