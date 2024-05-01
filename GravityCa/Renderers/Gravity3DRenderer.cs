using GravityCa;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.GravityCa.Renderers;
internal class Gravity3DRenderer : GraphicsDevice3DColorMapRenderer
{
    private static Vector3[] Vectors = new Vector3[4];

    public static string RenderKey => "3DPlane";

    public Point ScreenSize { get; set; }
    public GravityMap GravityMap { get; set; } = null!; // LoadContent
    public Camera3D Camera { get; } = Camera3D.CreateDefaultOrthographic();

    public Gravity3DRenderer(GraphicsDevice graphicsDevice, BasicEffect effect, int initialPrimitiveCount = 10000000) : base(graphicsDevice, effect, initialPrimitiveCount)
    {
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        GravityMap = GravityGame.Map;
        ScreenSize = GravityGame.ScreenScale;
        Camera.MoveTo(new(-45, 135, 82));
        Camera.LookAt(new(GravityGame.MapSize / 2, GravityGame.MapSize / 2, 0));
    }

    public override void Update(MooseGame game, GameTime gameTime, IMap map)
    {
        if (map.RendererKey != RenderKey)
                return;

        Camera.HandleControls(gameTime);

        if (!GravityGame.DrawMass && !GravityGame.DrawGravity)
            return;

        PrimitiveType = GravityGame.GravityRendererMode switch
        {
            GravityRendererMode.ThreeDimmensionalDots => PrimitiveType.PointList,
            _ => PrimitiveType.TriangleList,
        };

        VertexBufferIndex = 0;
        IndexBufferIndex = 0;
        PrimitiveCount = 0;
        
        Effect.View = Camera.View;

        var (divisor, multiplier, reducer) = GravityGame.GravityHeightLerpMode switch
        {
            LerpMode.ZeroToSystemMax => (GravityMap.MaxGravity, 20f, 0),
            LerpMode.SystemMinToSystemMax => (GravityMap.MaxGravity, (float)GravityGame.MapSize, GravityMap.MinGravity),
            _ => ((double)GravityGame.MaxGravity, (float)GravityGame.MapSize, 0)
        };

        for (int i = 0; i < GravityMap.Width; i++)
            for (int j = 0; j < GravityMap.Height; j++)
                AddCell(i, j, divisor, multiplier, reducer);
    }

    private void AddCell(int i, int j, double divisor, float multiplier, double reducer)
    {
        //var gravity = GravityMap.GetGravityAt(i, j, 0);
        var allGravities = GravityMap.GetGravityAdjacent(i, j).Select(a => (double)a.Value).ToArray();
        var cellGravity = allGravities[4];
        var gravity = (float)((cellGravity - reducer) / (divisor - reducer)) * multiplier;
        var color = GravityMap.GetColorAt(i, j);
        var x = (float)i;
        var z = (float)j;

        switch (GravityGame.GravityRendererMode)
        {
            case GravityRendererMode.ThreeDimmensionalPlane:
                Vectors[0] = new Vector3(x, gravity, z);
                Vectors[1] = new Vector3(x + 1, gravity, z);
                Vectors[2] = new Vector3(x + 1, gravity, z + 1);
                Vectors[3] = new Vector3(x, gravity, z + 1);
                AddQuad(Vectors, color);
                break;
            case GravityRendererMode.ThreeDimmensionalSheet:
                AddSheetQuad(x, z, color, allGravities, divisor, multiplier, reducer);
                break;
            case GravityRendererMode.ThreeDimmensionalDots:
                for (var q = 0f; q < 1; q++)
                {
                    Vectors[0] = new Vector3(x + q/20f, gravity + q/20f, z + q/20f);
                    PushVertex(Vectors[0], color);
                    PrimitiveCount++;
                }
                break;
            case GravityRendererMode.ThreeDimmensionalCube:
                CreateCube(x, gravity, z, 1, 1, 1, color);
                break;
            case GravityRendererMode.ThreeDimmensionalRectangularPrism1:
                CreateCube(x, gravity, z, 1, GravityGame.MapSize- gravity, 1, color);
                break;
            case GravityRendererMode.ThreeDimmensionalRectangularPrism2:
                CreateCube(x, 0, z, 1, gravity, 1, color);
                break;
        }
    }

    private void AddSheetQuad(float x, float z, Color color, double[] tiles, double divisor, float multiplier, double reducer)
    {
        double[] sums = [
            tiles[3] + tiles[4] + tiles[0] + tiles[1],
            tiles[3] + tiles[4] + tiles[6] + tiles[7],
            tiles[4] + tiles[5] + tiles[7] + tiles[8],
            tiles[1] + tiles[2] + tiles[4] + tiles[5],
        ];
        var averages = sums.Select(sum => (float)((((double)sum / 4.0) - reducer) / (divisor - reducer))*multiplier).ToArray();
        Vectors[0] = new Vector3(x, averages[0], z);
        Vectors[1] = new Vector3(x + 1, averages[1], z);
        Vectors[2] = new Vector3(x + 1, averages[2], z + 1);
        Vectors[3] = new Vector3(x, averages[3], z + 1);
        AddQuad(Vectors, color);
    }

    private void CreateCube(float x, float y, float z, float cellSizeX, float cellSizeY, float cellSizeZ, Color color)
    {
        CreateWall(x, y, z, cellSizeX, cellSizeY, cellSizeZ, 0, color);
        CreateWall(x, y, z, cellSizeX, cellSizeY, cellSizeZ, 1, color);
        CreateWall(x, y, z, cellSizeX, cellSizeY, cellSizeZ, 2, color);
        CreateWall(x, y, z, cellSizeX, cellSizeY, cellSizeZ, 3, color);
        CreateWall(x, y, z, cellSizeX, cellSizeY, cellSizeZ, 4, color);
        CreateWall(x, y, z, cellSizeX, cellSizeY, cellSizeZ, 5, color);
    }

    private void CreateWall(float x, float y, float z, float cellSizeX, float cellSizeY, float cellSizeZ, int direction, Color? color = null)
    {
        switch (direction)
        {
            case 0:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x, y, z + cellSizeZ);
                Vectors[2] = new Vector3(x + cellSizeX, y, z + cellSizeZ);
                Vectors[3] = new Vector3(x + cellSizeX, y, z);
                break;
            case 1:
                Vectors[0] = new Vector3(x + cellSizeX, y, z);
                Vectors[1] = new Vector3(x + cellSizeX, y, z + cellSizeZ);
                Vectors[2] = new Vector3(x + cellSizeX, y + cellSizeY, z + cellSizeZ);
                Vectors[3] = new Vector3(x + cellSizeX, y + cellSizeY, z);
                break;
            case 2:
                Vectors[0] = new Vector3(x + cellSizeX, y + cellSizeY, z);
                Vectors[1] = new Vector3(x + cellSizeX, y + cellSizeY, z + cellSizeZ);
                Vectors[2] = new Vector3(x, y + cellSizeY, z + cellSizeZ);
                Vectors[3] = new Vector3(x, y + cellSizeY, z);
                break;
            case 3:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x, y, z + cellSizeZ);
                Vectors[2] = new Vector3(x, y + cellSizeY, z + cellSizeZ);
                Vectors[3] = new Vector3(x, y + cellSizeY, z);
                break;
            case 4:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x + cellSizeX, y, z);
                Vectors[2] = new Vector3(x + cellSizeX, y + cellSizeY, z);
                Vectors[3] = new Vector3(x, y + cellSizeY, z);
                break;
            case 5:
                Vectors[0] = new Vector3(x, y, z + cellSizeZ);
                Vectors[1] = new Vector3(x + cellSizeX, y, z + cellSizeZ);
                Vectors[2] = new Vector3(x + cellSizeX, y + cellSizeY, z + cellSizeZ);
                Vectors[3] = new Vector3(x, y + cellSizeY, z + cellSizeZ);
                break;
            case 6:
            case 8:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x, y, z + cellSizeZ);
                Vectors[2] = new Vector3(x + cellSizeX, y + cellSizeY, z + cellSizeZ);
                Vectors[3] = new Vector3(x + cellSizeX, y + cellSizeY, z);
                break;
            case 7:
            case 9:
                Vectors[0] = new Vector3(x + cellSizeX, y, z);
                Vectors[1] = new Vector3(x + cellSizeX, y, z + cellSizeZ);
                Vectors[2] = new Vector3(x, y + cellSizeY, z + cellSizeZ);
                Vectors[3] = new Vector3(x, y + cellSizeY, z);
                break;
        }

        AddQuad(Vectors, color ?? Color.Transparent);
    }
}
