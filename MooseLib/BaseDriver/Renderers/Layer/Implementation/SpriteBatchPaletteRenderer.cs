using Merthsoft.Moose.MooseEngine.Interface;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;

public class SpriteBatchPaletteRenderer : SpriteLayerBatchRenderer
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Point ScaledSize => new((int)(Width * DrawScale.X), (int)(Height * DrawScale.Y));

    public Color[] Colors { get; set; } = [];
    public bool BlendColors { get; set; } = true;
    public UInt128 MaxValue { get; private set; }

    public UInt128? MinDrawValue { get; set; }
    public UInt128? MaxDrawValue { get; set; }

    public bool UseTransparentForZero { get; set; } = false;
    public bool IsActive { get; set; } = true;

    private Texture2D BackingTexture = null!; // LoadContent
    private Color[] ColorArray;

    public SpriteBatchPaletteRenderer(SpriteBatch spriteBatch, int width, int height, UInt128 maxValue, params Color[] colors) : base(spriteBatch)
    {
        Width = width;
        Height = height;
        MaxValue = maxValue;
        Colors = colors;
        ColorArray = new Color[width * height];
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        BackingTexture = new Texture2D(contentManager.GraphicsDevice, Width, Height);
    }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer)
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
                Color color;

                if (MinDrawValue.HasValue && value < MinDrawValue.Value
                    || MaxDrawValue.HasValue && value > MaxDrawValue.Value 
                    || percentage == 0 && UseTransparentForZero)
                {
                    color = Color.Transparent;
                }
                else if (percentage >= 1)
                {
                    color = Colors.Last();
                }
                else
                {
                    var colorLocation = (Colors.Length - 1) * percentage;
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
        SpriteBatch.Draw(BackingTexture, new Rectangle(DrawOffset.ToPoint(), ScaledSize), Color.White);
    }
}