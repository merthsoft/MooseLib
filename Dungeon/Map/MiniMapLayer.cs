using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Map;
public class MiniMapLayer : ITileLayer<MiniMapTile>
{
    public int Width { get; }
    public int Height { get; }
    public string Name { get; }
    public bool IsHidden { get; set; }
    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawSize { get; set; }
    public Color DrawColor { get; set; } = Color.White;

    public string? RendererKey { get; set; }

    public MiniMapLayer(string name, int width, int height)
    {
        Width = width;
        Height = height;
        Name = name;
    }

    public MiniMapTile GetTileValue(int x, int y) 
        => DungeonPlayer.Instance.GetMiniMapTile(x, y);
}
