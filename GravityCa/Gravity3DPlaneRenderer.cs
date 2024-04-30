using GravityCa;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.GravityCa;
internal class Gravity3DPlaneRenderer : GraphicsDevice3DPointListMapRenderer
{
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
            new(GravityMap.Width/2, 0, GravityMap.Height/2), 
            Vector3.Forward);

        var vector = new Vector3();
        for (int i = 0; i < GravityMap.Width; i++)
            for (int j = 0; j < GravityMap.Height; j++)
            {
                var gravity = GravityMap.GetGravityAt(i, j, 0);
                var signedGravity = (double)gravity - (double)UInt128.MaxValue / 2;

                vector.X = i;
                vector.Z = j;
                vector.Y = (float)signedGravity;
                var color = GravityMap.GetColorAt(i, j);
                SetVertexAndIncrementBufferIndex(vector, color);
            }
    }
}
