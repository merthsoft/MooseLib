using Merthsoft.Moose.GravityCa;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
using Merthsoft.Moose.MooseEngine.Extension;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GravityCa;
enum LerpMode
{
    ZeroToGlobalMax,
    ZeroToSystemMax,
    SystemMinToSystemMax
}
internal class GravityMapRenderer(SpriteBatch spriteBatch, Point scaledSize) : SpriteBatchMapRenderer(spriteBatch)
{
    public int Width { get; set; } = GravityCellularAutomataGame.MapSize;
    public int Height { get; set; } = GravityCellularAutomataGame.MapSize;

    public Point ScreenSize { get; set; } = scaledSize;

    public bool DrawGravity { get; set; } = true;
    public bool DrawMass { get; set; } = true;

    public Color[] GravityColors { get; set; } = Palettes.AllPalettes[1];
    public LerpMode GravityLerpMode { get; set; } = LerpMode.SystemMinToSystemMax;
    public Color[] MassColors { get; set; } = Palettes.AllPalettes[0];
    public UInt128? MassMinDrawValue { get; set; }

    private Texture2D BackingTexture = null!; // LoadContent
    private Color[] ColorArray = new Color[GravityCellularAutomataGame.MapSize * GravityCellularAutomataGame.MapSize];

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        BackingTexture = new Texture2D(contentManager.GraphicsDevice, Width, Height);
    }

    public override void Draw(MooseGame game, GameTime gameTime, IMap map)
    {
        if (!DrawMass && !DrawGravity) 
            return;

        var gravityMap = map as GravityMap ?? throw new NotSupportedException();
        if (gravityMap.UpdateState != 3 && gravityMap.Running) // Always re-render when it's paused in case things change
        {
            SpriteBatch.Draw(BackingTexture, new Rectangle(DrawOffset.ToPoint(), ScreenSize), Color.White);
            return;
        }

        var gravityLayer = gravityMap.GravityLayer;
        var massLayer = gravityMap.MassLayer;
        
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
            {
                Color? color = null;
                if (DrawMass && gravityMap.TotalMass > 0)
                    color = GetColor(massLayer, i, j, MassColors, (double)GravityCellularAutomataGame.MaxMass, MassMinDrawValue, null, gravityMap.MinMass, gravityMap.MaxMass, LerpMode.ZeroToSystemMax);
                if (DrawGravity && color == null && gravityMap.TotalGravity > 0)
                    color = GetColor(gravityLayer, i, j, GravityColors, (double)GravityCellularAutomataGame.MaxGravity, null, null, gravityMap.MinGravity, gravityMap.MaxGravity, GravityLerpMode);

                ColorArray[j * Height + i] = color ?? Color.Transparent;
            }

        BackingTexture.SetData(ColorArray); 
        SpriteBatch.Draw(BackingTexture, new Rectangle(DrawOffset.ToPoint(), ScreenSize), Color.White);
    }

    private Color? GetColor(
                           UInt128[] tileLayer,
                           int i,
                           int j,
                           Color[] colors,
                           double overallMax,
                           UInt128? minDrawValue,
                           UInt128? maxDrawValue,
                           double systemMin,
                           double systemMax,
                           LerpMode lerpMode)
    {
        var value = (double)tileLayer[i * Width + j];
        var percentage = lerpMode switch
        {
            LerpMode.ZeroToSystemMax => value / systemMax,
            LerpMode.SystemMinToSystemMax => (value - systemMin) / (systemMax - systemMin),
            _ => value / overallMax,
        };

        if (minDrawValue.HasValue && value < (double)minDrawValue.Value
            || maxDrawValue.HasValue && value > (double)maxDrawValue.Value
            || percentage == 0)
        {
            return null;
        }
        else if (percentage >= 1)
        {
            return colors.Last();
        }

        var colorLocation = (colors.Length - 1) * percentage;
        var colorIndex = (int)colorLocation;
        var newPercentage = colorLocation - colorIndex;
        return GraphicsExtensions.ColorGradientPercentage(colors[colorIndex], colors[colorIndex + 1], newPercentage);
    }
}
