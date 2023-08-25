using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public class TileLayer<TTile> : ITileLayer<TTile>
{
    public string? RendererKey { get; set; }

    public int Height { get; }
    public int Width { get; }

    public string Name { get; }

    public bool IsHidden { get; set; } = false;
    public float Opacity { get; set; }
    public virtual Vector2 DrawOffset { get; set; }
    public virtual Vector2 DrawSize { get; set; }

    public TTile[,] Tiles { get; protected set; }

    public TTile this[int i, int j]
    {
        get => GetTileValue(i, j);
        set => SetTileValue(i, j, value);
    }

    public TTile EdgeTile { get; set; }
    public Color DrawColor { get; set; } = Color.White;

    public TileLayer(string name, int width, int height, TTile edgeTile)
    {
        Name = name;
        Width = width;
        Height = height;
        EdgeTile = edgeTile;

        Tiles = new TTile[Width, Height];
    }

    public TileLayer(string name, int width, int height, TTile edgeTile, TTile defaultTile)
        : this(name, width, height, edgeTile)
    { 
        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                Tiles[i, j] = defaultTile;
    }

    public TTile GetTileValue(int x, int y)
        => x < 0 || x >= Width || y < 0 || y >= Height
        ? EdgeTile 
        : Tiles[x, y];

    public void SetTileValue(int x, int y, TTile value)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;
        Tiles[x, y] = value;
    }

    public void SetTileValue(int x, int y, TTile value, int thickness)
    {
        if (thickness == 0)
        {
            SetTileValue(x, y, value);
            return;
        }
        for (var deltaX = -thickness; deltaX <= thickness; deltaX++)
            for (var deltaY = -thickness; deltaY <= thickness; deltaY++)
                SetTileValue(x + deltaX, y + deltaY, value);
    }

    public void CopyTiles(TTile[,] tiles)
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Tiles[x, y] = tiles[x, y];
    }

    public virtual void Update(GameTime gameTime) { }
}
