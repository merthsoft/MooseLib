using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Merthsoft.Moose.MooseEngine.Renderers;

public class SpriteBatchPaletteRenderer : SpriteBatchRenderer
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Point ScaledSize => new(Width * (int)DrawScale.X, Height * (int)DrawScale.Y);

    public List<Color> Colors { get; set; } = new();
    public bool BlendColors { get; set; } = true;
    public UInt128 MaxValue { get; private set; }

    public bool UseTransparentForZero { get; set; } = false;
    public bool IsActive { get; set; } = true;

    private Texture2D BackingTexture = null!; // LoadContent
    private Color[] ColorArray;

    public SpriteBatchPaletteRenderer(SpriteBatch spriteBatch, int width, int height, UInt128 maxValue, params Color[] colors) : base(spriteBatch)
    {
        Width = width;
        Height = height;
        MaxValue = maxValue;
        Colors.AddRange(colors);
        ColorArray = new Color[width * height];
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        BackingTexture = new Texture2D(contentManager.GraphicsDevice, Width, Height);
    }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer, Vector2 drawOffset)
    {
        if (!IsActive)
            return;

        if (layer is not TileLayer<UInt128> tileLayer)
            throw new Exception("TileLayer<UInt128> layer expected");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
            {
                var value = tileLayer.Tiles[i, j];
                var percentage = (double)value / (double)MaxValue;
                var color = Color.Transparent;

                if (percentage >= 1)
                {
                    color = Colors.Last();
                }
                else if (percentage > 0 || !UseTransparentForZero)
                {
                    var colorLocation = (Colors.Count - 1) * percentage;
                    var colorIndex = (int)colorLocation;
                    color = Colors[colorIndex];
                    if (BlendColors)
                    {
                        var newPercentage = colorLocation - colorIndex;
                        color = GraphicsExtensions.ColorGradientPercentage(Colors[colorIndex], Colors[colorIndex + 1], newPercentage);
                    }
                }

                ColorArray[j * Height + i] = color;
            }

        BackingTexture.SetData(ColorArray);
        SpriteBatch.Draw(BackingTexture, new Rectangle((drawOffset + DrawOffset).ToPoint(), ScaledSize), Color.White);
    }
}