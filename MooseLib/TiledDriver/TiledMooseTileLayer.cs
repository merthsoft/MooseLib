using Merthsoft.Moose.MooseEngine.Interface;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MooseEngine.TiledDriver;

public record TiledMooseTileLayer(TiledMapTileLayer Layer) : ITileLayer<TiledMapTile>
{
    public string? RendererKey { get; set; }

    public string Name => Layer.Name;
    public bool IsHidden
    {
        get => !Layer.IsVisible;
        set => Layer.IsVisible = !value;
    }

    public float Opacity
    {
        get => Layer.Opacity;
        set => Layer.Opacity = value;
    }
    public int Width => Layer.Width;
    public int Height => Layer.Height;

    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawSize { get; set; }
    public Color DrawColor { get; set; } = Color.White;

    public TiledMapTile GetTileValue(int x, int y)
        => Layer.GetTile((ushort)x, (ushort)y);

    public bool IsBlockedAt(int x, int y, IMap map)
        => GetTileValue(x, y).IsBlocking(map);
}
