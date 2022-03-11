using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Map;
public class MiniMapLayer : ITileLayer<int>
{
    public int Width { get; }
    public int Height { get; }
    public string Name { get; }
    public bool IsHidden { get; set; }
    public Vector2 DrawOffset { get; set; }

    public string? RendererKey { get; set; }

    public MiniMapLayer(string name, int width, int height)
    {
        Width = width;
        Height = height;
        Name = name;
    }

    int ITileLayer<int>.GetTileValue(int x, int y) => (int)DungeonPlayer.Instance.GetMiniMapTile(x, y);
}
