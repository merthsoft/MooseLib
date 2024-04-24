using Merthsoft.Moose.GravityCa;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;
using Merthsoft.Moose.MooseEngine.Extension;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GravityCa;
internal class GravityMapRenderer(SpriteBatch spriteBatch, Point scaledSize) : SpriteBatchMapRenderer(spriteBatch)
{
    public int Width { get; set; } = GravityCellularAutomataGame.MapSize;
    public int Height { get; set; } = GravityCellularAutomataGame.MapSize;

    public Point ScreenSize { get; set; } = scaledSize;

    public bool DrawGravity { get; set; } = true;
    public bool DrawMass { get; set; } = true;

    public Color[] GravityColors { get; set; } = Palettes.AllPalettes[1];
    public bool GravityBlendColors { get; set; } = true;
    public bool GravityRelativeSpectrum { get; set; } = true;
    public Color[] MassColors { get; set; } = Palettes.AllPalettes[1];
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
        var gravityMap = map as GravityMap ?? throw new NotSupportedException();

        var gravityLayer = gravityMap.GravityLayer;
        var massLayer = gravityMap.MassLayer;
        var massMax = Math.Max(1, gravityMap.MaxMass);
        var gravityMax = Math.Max(1, gravityMap.MaxGravity);

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
            {
                Color? color = null;
                if (DrawMass)
                    color = GetColor(massLayer, i, j, MassColors, massMax, MassMinDrawValue, null, true);
                if (DrawGravity && color == null)
                    color = GetColor(gravityLayer, i, j, GravityColors, GravityRelativeSpectrum ? gravityMax :(double)GravityCellularAutomataGame.MaxGravity, null, null, GravityBlendColors);

                ColorArray[j * Height + i] = color ?? Color.Transparent;
            }

        BackingTexture.SetData(ColorArray); 
        SpriteBatch.Draw(BackingTexture, new Rectangle(DrawOffset.ToPoint(), ScreenSize), Color.White);
    }

    private Color? GetColor(TileLayer<UInt128> tileLayer,
                           int i,
                           int j,
                           Color[] colors,
                           double maxValue,
                           UInt128? minDrawValue,
                           UInt128? maxDrawValue,
                           bool blendColors)
    {
        var value = tileLayer.Tiles[i, j];
        var percentage = (double)value / (double)maxValue;
        Color color;

        if (minDrawValue.HasValue && value < minDrawValue.Value
            || maxDrawValue.HasValue && value > maxDrawValue.Value
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
        color = colors[colorIndex];
        if (blendColors)
        {
            var newPercentage = colorLocation - colorIndex;
            color = GraphicsExtensions.ColorGradientPercentage(colors[colorIndex], colors[colorIndex + 1], newPercentage);
        }
        return color;
    }
}
