using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.Rays.Actors;

namespace Merthsoft.Moose.Rays;
public class Ray3DRenderer : GraphicsDeviceRenderer
{
    protected List<VertexPositionColorTexture> VertexBuffer = new();
    protected List<int> IndexBuffer = new();
    protected int PrimitiveCount;

    protected RayPlayer Player = null!;
    protected float TextureWidth = 0;

    public Ray3DRenderer(GraphicsDevice graphics, BasicEffect effect) : base(graphics, effect)
    {
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        TextureWidth = RayGame.Instance.TextureAtlas.Width;
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
                if (wall <= 0)
                {
                    var floor = floorLayer.GetTileValue(x, y);
                    var ceiling = ceilingLayer.GetTileValue(x, y);
                    CreateWall(x * 16, y * 16, ceiling, 4);
                    CreateWall(x * 16, y * 16, ceiling, 5);
                    continue;
                }
                CreateWalls(map, x * 16, y * 16, wall);
            }

        foreach (var obj in objectLayer.Objects.Cast<RayGameObject>().OrderByDescending(o => o.DistanceSquaredTo(Player)))
        {
            if (obj is RayPlayer)
                continue;

            var (x, y) = obj.Position;
            switch (obj.ObjectRenderMode)
            {
                case ObjectRenderMode.Sprite:
                case ObjectRenderMode.Directional:
                    CreateSprite(x, y, obj);
                    break;
                case ObjectRenderMode.Wall:
                    CreateWalls(map, x - 8, y - 8, obj.TextureIndex + obj.TextureIndexOffset);
                    break;
                case ObjectRenderMode.Door:
                    CreateDoor(x, y, (obj as Door)!);
                    break;
            }
        }
    }

    private void CreateDoor(float x, float y, Door door)
{
        x -= 8;
        y -= 8;
        if (door.Horizontal)
        {
            CreateWall(x - 16 * door.OpenPercent, y + 8, door.TextureIndex, 0);
            CreateWall(x - .01f, y, 57, 1);
            CreateWall(x + .01f, y, 57, 3);
        }
        else
        {
            CreateWall(x + 8, y - 16 * door.OpenPercent, door.TextureIndex, 3);
            CreateWall(x, y + .01f, 57, 0);
            CreateWall(x, y - .01f, 57, 2);
        }
    }

    private bool IsWallRedundant(RayMap map, float x, float y)
    {
        var wall = map.WallLayer.GetTileValue((int)x, (int)y);
        if (wall != -1)
            return true;
        //var wallCell = new Point((int)x, (int)y);
        //var o = map.ObjectLayer.Objects.Where(o => o.GetCell() == wallCell);
        //if (o.Any(o => o.ObjectRenderMode == ObjectRenderMode.Door))
        //    return true;
        return false;
    }

    private void CreateWalls(RayMap map, float x, float y, int wall)
    {
        CreateWall(x, y, wall, 0);
        CreateWall(x, y, wall, 1);
        CreateWall(x, y, wall, 2);
        CreateWall(x, y, wall, 3);
        CreateWall(x, y, wall, 4);
        CreateWall(x, y, wall, 5);
    }

    private void CreateSprite(float x, float y, RayGameObject obj)
    {
        var textureIndex = obj.TextureIndex + obj.TextureIndexOffset;

        if (obj.ObjectRenderMode == ObjectRenderMode.Directional)
        {
            var frames = (obj as Actor)?.CurrentState?.Count ?? 1;
            var objectRotation = (obj.FacingDirection.Atan2() - (Player.Position - obj.Position).Atan2())
                .ToDegrees().CardinalDirection8IndexDegrees();

            textureIndex += frames * objectRotation;
        }

        var vectors = new Vector3[4];
        vectors[0] = new Vector3(-4, 0, obj.RayGameObjectDef.RenderBottom);
        vectors[1] = new Vector3(-4, 0, obj.RayGameObjectDef.RenderTop);
        vectors[2] = new Vector3(+4, 0, obj.RayGameObjectDef.RenderTop);
        vectors[3] = new Vector3(+4, 0, obj.RayGameObjectDef.RenderBottom); 
        
        var currIndex = VertexBuffer.Count;
        var xStart = (textureIndex * 16) / TextureWidth;
        var xEnd = ((textureIndex + 1) * 16) / TextureWidth;
        var yStart = 0;
        var yEnd = 1;

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

    private void CreateWall(float x, float y, int wall, int direction, Color? color = null)
    {
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
            case 6:
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
        var yEnd = 1;

        var c = color ?? (direction is 1 or 3 
            ? new Color(170, 170, 170) 
            : direction is 4 
                ? new Color(125, 125, 170)
                : direction is 5 
                    ? new Color(70, 70, 70)
                    : Color.White);
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[0], c, new(xStart, yStart)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[1], c, new(xStart, yEnd)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[2], c, new(xEnd, yEnd)));
        VertexBuffer.Add(new VertexPositionColorTexture(vectors[3], c, new(xEnd, yStart)));
        

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

        Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(100), .85f, 1f, 1000f);
        Effect.View = Matrix.CreateLookAt(
            RayPlayer.Instance.PositionIn3dSpace,
            RayPlayer.Instance.PositionIn3dSpace + RayPlayer.Instance.FacingDirection, Vector3.Forward);
        //var map = RayGame.Instance.RayMap;
        //Effect.View = Matrix.CreateLookAt(
        //    new Vector3(map.Width * 8, map.Height * 8, 450),
        //    new Vector3(map.Width * 8, map.Height * 8, 0), Vector3.Up);
        //Effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

        GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };

        foreach (var pass in Effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VertexBuffer.ToArray(), 0, VertexBuffer.Count, IndexBuffer.ToArray(), 0, PrimitiveCount);
        }
    }
}
