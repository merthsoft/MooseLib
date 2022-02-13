using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Merthsoft.Moose.MooseEngine.Ui;

public class Theme
{
    public string Name { get; private set; }

    internal Rectangle[] TextureRects = new Rectangle[16];

    private Texture2D windowTexture;
    private Point textureOffset;
    private int tileWidth;
    private int tileHeight;

    public List<SpriteFont> Fonts { get; } = new();

    public Texture2D WindowTexture
    {
        get => windowTexture;
        set
        {
            windowTexture = value;
            CalculateTextureRectangles();
        }
    }

    public Point TextureOffset
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

    public Theme(string name, Texture2D windowTexture, int tileWidth, int tileHeight, IEnumerable<SpriteFont> fonts, Point textureOffset = default, Vector2 controlDrawOffset = default)
    {
        (Name, TileWidth, TileHeight, this.windowTexture, this.textureOffset, TextureWindowControlDrawOffset)
         = (name, tileWidth, tileHeight, windowTexture, textureOffset, controlDrawOffset);
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

    public Color ResolveBackgroundColor(UpdateParameters updateParameters, bool enabled, bool highlightOnHover = true)
        => (enabled, updateParameters.MouseOver && highlightOnHover) switch
        {
            (false, _) => ControlDisabledBackgroundColor,
            (true, true) => ControlMouseOverBackgroundColor,
            (true, false) => ControlBackgroundColor
        };

    public Vector2 DrawWindow(SpriteBatch spriteBatch, Rectangle rectangle, BackgroundDrawingMode backgroundDrawingMode)
    {
        switch (backgroundDrawingMode)
        {
            case BackgroundDrawingMode.Basic:
                DrawWindowBasic(spriteBatch, rectangle);
                return BasicWindowControlDrawOffset;
            case BackgroundDrawingMode.Texture:
                DrawWindowTexture(spriteBatch, rectangle);
                return TextureWindowControlDrawOffset;
        }
        return Vector2.Zero;
    }

    protected void DrawWindowBasic(SpriteBatch spriteBatch, Rectangle rectangle)
        => spriteBatch.FillRect(rectangle, WindowBackgroundColor, WindowBorderColor);

    protected void DrawWindowTexture(SpriteBatch spriteBatch, int index, Point position, int xTileOffset, int yTileOffet)
    {
        var destRect = new Rectangle(position.X + xTileOffset * TileDrawWidth, position.Y + yTileOffet * TileDrawHeight, TileDrawWidth, TileDrawHeight);
        spriteBatch.Draw(WindowTexture, destRect, TextureRects[index], Color.White);
    }

    protected void DrawWindowTexture(SpriteBatch spriteBatch, Rectangle rectangle)
    {
        var numXTiles = rectangle.Width / TileDrawWidth;
        var numYTiles = rectangle.Height / TileDrawHeight;

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
                DrawWindowTexture(spriteBatch, index, rectangle.Location, x, y);
            }
    }

    protected void CalculateTextureRectangles()
    {
        for (var index = 0; index < 9; index++)
            TextureRects[index] = new Rectangle((index % 3) * TileWidth + TextureOffset.X, index / 3 * TileHeight + TextureOffset.Y, TileWidth, TileHeight);

        for (var index = 0; index < 3; index++)
        {
            TextureRects[index + 9] = new Rectangle(index * TileWidth + TextureOffset.X, 3 * TileHeight + TextureOffset.Y, TileWidth, TileHeight);
            TextureRects[index + 12] = new Rectangle(3 * TileWidth + TextureOffset.X, index * TileHeight + TextureOffset.Y, TileWidth, TileHeight);
        }

        TextureRects[15] = new Rectangle(3 * TileWidth + TextureOffset.X, 3 * TileHeight + TextureOffset.Y, TileWidth, TileHeight);
    }

    public Vector2 MeasureString(string s, int fontIndex)
        => Fonts[fontIndex].MeasureString(s);

    public string TruncateString(string s, int width, string truncationString = "...", int fontIndex = 0)
    {
        var totalLength = MeasureString(s, fontIndex).X;
        if (totalLength < width)
            return s;

        var length = MeasureString(truncationString, fontIndex).X.Ceiling();
        var nameBuilder = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            length += MeasureString(c.ToString(), fontIndex).X.Ceiling();
            if (length >= width)
                break;
            nameBuilder.Append(c);
        }
        return nameBuilder.Append(truncationString).ToString();
    }

}
