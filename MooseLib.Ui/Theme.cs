using System.Text;

namespace Merthsoft.Moose.MooseEngine.Ui;

public class Theme
{
    internal Rectangle[] TextureRects = new Rectangle[16];

    private Texture2D windowTexture;
    private Vector2 textureOffset;
    private int tileWidth;
    private int tileHeight;

    public List<SpriteFont> Fonts { get; } = [];

    public Texture2D? Cursor { get; set; }

    public Texture2D WindowTexture
    {
        get => windowTexture;
        set
        {
            windowTexture = value;
            CalculateTextureRectangles();
        }
    }

    public Vector2 TextureOffset
    {
        get => textureOffset;
        set
        {
            textureOffset = value;
            CalculateTextureRectangles();
        }
    }

    public Vector2 TileScale { get; set; } = new(1, 1);

    public Vector2 TextureWindowControlDrawOffset { get; set; } = new(2, 2);
    public Vector2 BasicWindowControlDrawOffset { get; set; } = new(2, 2);

    public int TileWidth
    {
        get => tileWidth;
        set
        {
            tileWidth = value;
            CalculateTextureRectangles();
        }
    }

    public int TileHeight
    {
        get => tileHeight;
        set
        {
            tileHeight = value;
            CalculateTextureRectangles();
        }
    }

    public int TileDrawWidth => (int)(TileWidth * TileScale.X);
    public int TileDrawHeight => (int)(TileHeight * TileScale.Y);

    public Color TextColor { get; set; } = Color.Black;
    public Color TextMouseOverColor { get; set; } = Color.White;
    public Color TextBorderColor { get; set; } = Color.Gray;
    public Color TextDisabledColor { get; set; } = Color.Gray;
    public Color SelectedColor { get; set; } = Color.Blue;
    public Color SelectedMouseOverColor { get; set; } = Color.DarkBlue;
    public Color ControlPointerColor { get; set; } = Color.Red;
    public Color ControlBackgroundColor { get; set; } = Color.Gray;
    public Color ControlDisabledBackgroundColor { get; set; } = Color.DarkGray;
    public Color ControlMouseOverBackgroundColor { get; set; } = Color.LightGray;
    public Color ControlBorderColor { get; set; } = Color.Black;
    public Color WindowBackgroundColor { get; set; } = new(113, 65, 59);
    public Color WindowBorderColor { get; set; } = new(219, 164, 99);

    public Theme(Texture2D windowTexture, int tileWidth, int tileHeight, IEnumerable<SpriteFont> fonts, Vector2 textureOffset = default, Vector2 controlDrawOffset = default)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        this.windowTexture = windowTexture;
        this.textureOffset = textureOffset;
        TextureWindowControlDrawOffset = controlDrawOffset;
        Fonts.AddRange(fonts);
    }

    public Color ResolveTextColor(UpdateParameters updateParameters, bool enabled, bool selected, bool highlightOnHover = true)
        => (enabled, selected, updateParameters.MouseOver && highlightOnHover) switch
        {
            (false, _, _) => TextDisabledColor,
            (true, true, true) => SelectedMouseOverColor,
            (true, true, false) => SelectedColor,
            (true, false, true) => TextMouseOverColor,
            (true, false, false) => TextColor
        };

    public Color ResolveMainColorShift(UpdateParameters updateParameters, bool enabled, bool selected, bool highlightOnHover = true)
        => (enabled, selected, updateParameters.MouseOver && highlightOnHover) switch
        {
            (false, _, _) => ControlDisabledBackgroundColor,
            (true, true, true) => SelectedMouseOverColor,
            (true, true, false) => SelectedColor,
            (true, false, true) => TextMouseOverColor,
            (true, false, false) => Color.White
        };

    public Color ResolveBackgroundColor(UpdateParameters updateParameters, bool enabled, bool highlightOnHover = true)
        => (enabled, updateParameters.MouseOver && highlightOnHover) switch
        {
            (false, _) => ControlDisabledBackgroundColor,
            (true, true) => ControlMouseOverBackgroundColor,
            (true, false) => ControlBackgroundColor
        };

    public Color ResolvePointerColor(bool selected)
        => selected ? SelectedMouseOverColor : ControlPointerColor;

    public Vector2 DrawWindow(SpriteBatch spriteBatch, Vector2 position, Vector2 size, BackgroundDrawingMode backgroundDrawingMode, Color? colorShift = null)
        => DrawWindow(spriteBatch, new(position, size), backgroundDrawingMode, colorShift);

    public Vector2 DrawWindow(SpriteBatch spriteBatch, RectangleF rectangle, BackgroundDrawingMode backgroundDrawingMode, Color? colorShift = null)
    {
        _ = backgroundDrawingMode switch
        {
            BackgroundDrawingMode.Basic => DrawWindowBasic(spriteBatch, rectangle),
            BackgroundDrawingMode.Texture => DrawWindowTexture(spriteBatch, rectangle, colorShift),
            _ => default,
        };
        return GetDrawOffset(backgroundDrawingMode);
    }

    public Vector2 GetDrawOffset(BackgroundDrawingMode backgroundDrawingMode)
        => backgroundDrawingMode switch
        {
            BackgroundDrawingMode.Basic => BasicWindowControlDrawOffset,
            BackgroundDrawingMode.Texture => TextureWindowControlDrawOffset,
            _ => Vector2.Zero,
        };

    protected RectangleF DrawWindowBasic(SpriteBatch spriteBatch, RectangleF rectangle)
    {
        spriteBatch.FillRect(rectangle, WindowBackgroundColor, WindowBorderColor);
        return rectangle;
    }

    protected void DrawWindowTexture(SpriteBatch spriteBatch, int index, Vector2 position, int xTileOffset, int yTileOffet, Color? colorShift = null)
    {
        var destRect = new RectangleF(position.X + xTileOffset * TileDrawWidth, position.Y + yTileOffet * TileDrawHeight, TileDrawWidth, TileDrawHeight);
        spriteBatch.Draw(WindowTexture, destRect.ToRectangle(), TextureRects[index], colorShift ?? Color.White);
    }

    public Vector2 CalculateNewSize(Vector2 size)
    {
        var numXTiles = (size.X / TileDrawWidth).Ceiling();
        var numYTiles = (size.Y / TileDrawHeight).Ceiling();
        return new(numXTiles * TileWidth, numYTiles * TileHeight);
    }

    protected RectangleF DrawWindowTexture(SpriteBatch spriteBatch, RectangleF rectangle, Color? colorShift = null)
    {
        var numXTiles = (rectangle.Width / TileDrawWidth).Ceiling();
        var numYTiles = (rectangle.Height / TileDrawHeight).Ceiling();

        var upperLeftIndex = 0;
        var upperIndex = 1;
        var upperRightIndex = 2;
        var leftIndex = 3;
        var middleIndex = 4;
        var rightIndex = 5;
        var bottomLeftIndex = 6;
        var bottomIndex = 7;
        var bottomRightIndex = 8;

        var heightOneLeftIndex = 9;
        var heightOneMiddleIndex = 10;
        var heightOneRightIndex = 11;

        if (numYTiles == 1)
        {
            upperLeftIndex = heightOneLeftIndex;
            upperIndex = heightOneMiddleIndex;
            upperRightIndex = heightOneRightIndex;
            leftIndex = heightOneLeftIndex;
            middleIndex = heightOneMiddleIndex;
            rightIndex = heightOneRightIndex;
            bottomLeftIndex = heightOneLeftIndex;
            bottomIndex = heightOneMiddleIndex;
            bottomRightIndex = heightOneRightIndex;
        }

        for (var x = 0; x < numXTiles; x++)
            for (var y = 0; y < numYTiles; y++)
            {
                var index = middleIndex;
                if ((x, y) == (0, 0))
                    index = upperLeftIndex;
                else if ((x, y) == (0, numYTiles - 1))
                    index = bottomLeftIndex;
                else if (x == 0)
                    index = leftIndex;
                else if ((x, y) == (numXTiles - 1, 0))
                    index = upperRightIndex;
                else if (y == 0)
                    index = upperIndex;
                else if ((x, y) == (numXTiles - 1, numYTiles - 1))
                    index = bottomRightIndex;
                else if (y == numYTiles - 1)
                    index = bottomIndex;
                else if (x == numXTiles - 1)
                    index = rightIndex;
                DrawWindowTexture(spriteBatch, index, rectangle.Position, x, y, colorShift);
            }

        return new(rectangle.X, rectangle.Y, numXTiles * TileWidth, numYTiles * TileHeight);
    }

    protected void CalculateTextureRectangles()
    {
        for (var index = 0; index < 9; index++)
            TextureRects[index] = new RectangleF((index % 3) * TileWidth + TextureOffset.X, index / 3 * TileHeight + TextureOffset.Y, TileWidth, TileHeight).ToRectangle();

        for (var index = 0; index < 3; index++)
        {
            TextureRects[index + 9] = new RectangleF(index * TileWidth + TextureOffset.X, 3 * TileHeight + TextureOffset.Y, TileWidth, TileHeight).ToRectangle();
            TextureRects[index + 12] = new RectangleF(3 * TileWidth + TextureOffset.X, index * TileHeight + TextureOffset.Y, TileWidth, TileHeight).ToRectangle();
        }

        TextureRects[15] = new RectangleF(3 * TileWidth + TextureOffset.X, 3 * TileHeight + TextureOffset.Y, TileWidth, TileHeight).ToRectangle();
    }

    public Vector2 MeasureString(string s, int fontIndex)
        => Fonts[fontIndex].MeasureString(s);

    public string TruncateString(string s, int width, string truncationString = "...", int fontIndex = 0)
    {
        var totalLength = MeasureString(s, fontIndex).X;
        if (totalLength < width)
            return s;

        var length = MeasureString(truncationString, fontIndex).X;
        var nameBuilder = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            length += MeasureString(c.ToString(), fontIndex).X;
            if (length >= width)
                break;
            nameBuilder.Append(c);
        }
        return nameBuilder.Append(truncationString).ToString();
    }

}
