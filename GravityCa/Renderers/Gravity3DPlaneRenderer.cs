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

    public Gravity3DPlaneRenderer(GraphicsDevice graphicsDevice, BasicEffect effect, int initialPrimitiveCount = 10000000) : base(graphicsDevice, effect, initialPrimitiveCount)
    {
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        GravityMap = GravityGame.Map;
        ScreenSize = GravityGame.ScreenScale;
    }

    public override void Update(MooseGame game, GameTime gameTime, IMap map)
    {
        if (!GravityGame.DrawMass && !GravityGame.DrawGravity)
            return;

        VertexBufferIndex = 0;
        IndexBufferIndex = 0;
        PrimitiveCount = 0;

        Effect.View = Matrix.CreateLookAt(
            GravityGame.PositionIn3dSpace,
            new(GravityMap.Width / 2, GravityMap.Height / 2, 50),
            Vector3.Up);

        for (int i = 0; i < GravityMap.Width; i++)
            for (int j = 0; j < GravityMap.Height; j++)
            {
                var gravity = GravityMap.GetGravityAt(i, j, 0);
                var signedGravity = (double)gravity / (double)GravityMap.MaxGravity;
                var color = GravityMap.GetColorAt(i, j);
                AddCell(i*2, j*2, (float)signedGravity*100, color);
            }
    }

    private void AddCell(float x, float y, float z, Color color)
    {
        Vectors[0] = new Vector3(x, y, z);
        Vectors[1] = new Vector3(x + 2, y, z);
        Vectors[2] = new Vector3(x + 2, y + 2, z);
        Vectors[3] = new Vector3(x, y + 2, z);
        AddQuad(Vectors, color);
    }
}
