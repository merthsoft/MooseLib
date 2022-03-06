using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public class TileLayer<TTile> : ITileLayer<TTile>
{
    public int Height { get; }
    public int Width { get; }

    public string Name { get; }

    public bool IsHidden { get; set; } = false;
    public float Opacity { get; set; }
    public virtual Vector2 DrawOffset { get; set; }

    public TTile[,] Tiles { get; }

    public TileLayer(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;

        Tiles = new TTile[Width, Height];
    }

    public TileLayer(string name, int width, int height, TTile defaultTile)
        : this(name, width, height)
    { 
        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                Tiles[i, j] = defaultTile;
    }

    public TTile GetTileValue(int x, int y)
        => Tiles[x, y];

    public void SetTileValue(int x, int y, TTile value)
        => Tiles[x, y] = value;
}
