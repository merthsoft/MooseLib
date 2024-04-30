using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GravityCa;
internal class GravityMapRenderer(SpriteBatch spriteBatch, Point scaledSize) : SpriteBatchMapRenderer(spriteBatch)
{
    public static string RenderKey => "Standard";

    public Point ScreenSize { get; set; } = scaledSize;


    private Texture2D BackingTexture = null!; // LoadContent
    private readonly Color[] ColorArray = new Color[GravityGame.MapSize * GravityGame.MapSize];

    private GravityMap GravityMap = null!; // LoadContent

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        GravityMap = GravityGame.Map;
        BackingTexture = new Texture2D(contentManager.GraphicsDevice, GravityMap.Width, GravityMap.Height);
    }

    public override void Draw(MooseGame game, GameTime gameTime, IMap map)
    {
        if (!GravityGame.DrawMass && !GravityGame.DrawGravity) 
            return;

        if (GravityMap.UpdateState != 3 && GravityMap.Running) // Always re-render when it's paused in case things change
        {
            SpriteBatch.Draw(BackingTexture, new Rectangle(DrawOffset.ToPoint(), ScreenSize), Color.White);
            return;
        }

        for (int i = 0; i < GravityMap.Width; i++)
            for (int j = 0; j < GravityMap.Height; j++)
                ColorArray[j * GravityMap.Height + i] = GravityMap.GetColorAt(i, j);

        BackingTexture.SetData(ColorArray); 
        SpriteBatch.Draw(BackingTexture, new Rectangle(DrawOffset.ToPoint(), ScreenSize), Color.White);
    }

    
}
