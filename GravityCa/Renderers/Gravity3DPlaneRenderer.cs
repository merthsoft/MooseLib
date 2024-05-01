using GravityCa;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.GravityCa.Renderers;
internal class Gravity3DPlaneRenderer : GraphicsDevice3DTriangleListColorMapRenderer
{
    private static Vector3[] Vectors = new Vector3[4];

    public static string RenderKey => "3DPlane";

    public Point ScreenSize { get; set; }
    public GravityMap GravityMap { get; set; } = null!; // LoadContent
    public Camera3D Camera { get; } = Camera3D.CreateDefaultOrthographic();

    public Gravity3DPlaneRenderer(GraphicsDevice graphicsDevice, BasicEffect effect, int initialPrimitiveCount = 10000000) : base(graphicsDevice, effect, initialPrimitiveCount)
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
        Camera.HandleControls(gameTime);

        if (!GravityGame.DrawMass && !GravityGame.DrawGravity)
            return;

        VertexBufferIndex = 0;
        IndexBufferIndex = 0;
        PrimitiveCount = 0;
        
        Effect.View = Camera.View;

        for (int i = 0; i < GravityMap.Width; i++)
            for (int j = 0; j < GravityMap.Height; j++)
            {
                var gravity = GravityMap.GetGravityAt(i, j, 0);
                var signedGravity = (double)gravity / (double)GravityGame.MaxGravity;
                var color = GravityMap.GetColorAt(i, j);
                AddCell(i, (float)signedGravity* (float)GravityGame.MapSize*1_000f, j, color);
            }
    }

    private void AddCell(float x, float y, float z, Color color)
    {
        //Vectors[0] = new Vector3(x, y, z);
        //Vectors[1] = new Vector3(x + 1, y, z);
        //Vectors[2] = new Vector3(x + 1, y, z + 1);
        //Vectors[3] = new Vector3(x, y, z + 1);
        //AddQuad(Vectors, color);
        CreateCube(x, y, z, 1, color);
    }

    private void CreateCube(float x, float y, float z, float cellSize, Color color)
    {
        CreateWall(x, y, z, cellSize, 0, color);
        CreateWall(x, y, z, cellSize, 1, color);
        CreateWall(x, y, z, cellSize, 2, color);
        CreateWall(x, y, z, cellSize, 3, color);
        CreateWall(x, y, z, cellSize, 4, color);
        CreateWall(x, y, z, cellSize, 5, color);
    }

    private void CreateWall(float x, float y, float z, float cellSize, int direction, Color? color = null)
    {
        switch (direction)
        {
            case 0:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x, y, z + cellSize);
                Vectors[2] = new Vector3(x + cellSize, y, z + cellSize);
                Vectors[3] = new Vector3(x + cellSize, y, z);
                break;
            case 1:
                Vectors[0] = new Vector3(x + cellSize, y, z);
                Vectors[1] = new Vector3(x + cellSize, y, z + cellSize);
                Vectors[2] = new Vector3(x + cellSize, y + cellSize, z + cellSize);
                Vectors[3] = new Vector3(x + cellSize, y + cellSize, z);
                break;
            case 2:
                Vectors[0] = new Vector3(x + cellSize, y + cellSize, z);
                Vectors[1] = new Vector3(x + cellSize, y + cellSize, z + cellSize);
                Vectors[2] = new Vector3(x, y + cellSize, z + cellSize);
                Vectors[3] = new Vector3(x, y + cellSize, z);
                break;
            case 3:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x, y, z + cellSize);
                Vectors[2] = new Vector3(x, y + cellSize, z + cellSize);
                Vectors[3] = new Vector3(x, y + cellSize, z);
                break;
            case 4:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x + cellSize, y, z);
                Vectors[2] = new Vector3(x + cellSize, y + cellSize, z);
                Vectors[3] = new Vector3(x, y + cellSize, z);
                break;
            case 5:
                Vectors[0] = new Vector3(x, y, z + cellSize);
                Vectors[1] = new Vector3(x + cellSize, y, z + cellSize);
                Vectors[2] = new Vector3(x + cellSize, y + cellSize, z + cellSize);
                Vectors[3] = new Vector3(x, y + cellSize, z + cellSize);
                break;
            case 6:
            case 8:
                Vectors[0] = new Vector3(x, y, z);
                Vectors[1] = new Vector3(x, y, z + cellSize);
                Vectors[2] = new Vector3(x + cellSize, y + cellSize, z + cellSize);
                Vectors[3] = new Vector3(x + cellSize, y + cellSize, z);
                break;
            case 7:
            case 9:
                Vectors[0] = new Vector3(x + cellSize, y, z);
                Vectors[1] = new Vector3(x + cellSize, y, z + cellSize);
                Vectors[2] = new Vector3(x, y + cellSize, z + cellSize);
                Vectors[3] = new Vector3(x, y + cellSize, z);
                break;
        }

        AddQuad(Vectors, color ?? Color.Transparent);
    }
}
