using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.Rays.Serialization;

namespace Merthsoft.Moose.Rays;
public class RayMap : BaseMap
{
    private int height;
    public override int Height => height;

    private int width;
    public override int Width => width;

    public override int TileWidth { get; } = 16;
    public override int TileHeight { get; } = 16;

    public TileLayer<int> FloorLayer { get; set; }   = null!;
    public TileLayer<int> WallLayer { get; set; }    = null!;
    public ObjectLayer<RayGameObject> ObjectLayer { get; set; } = null!;
    public TileLayer<int> CeilingLayer { get; set; } = null!;

    public RayMap()
    {
    }

    public void InitializeWalls(List<List<int>> wallMap)
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                WallLayer.SetTileValue(x, y, wallMap[y][x]);
    }

    public override int IsBlockedAt(string layer, int x, int y)
        => layer switch
        {
            "walls" => WallLayer.GetTileValue(x, y),
            "objects" => ObjectLayer.GetObjects(x, y).Count(o => o.Blocking),
            _ => 0
        };
    
    internal void Load(RayGame game, Definitions definitions, SaveMap mapFile)
    {
        height = mapFile.Height;
        width = mapFile.Width;
        
        ClearLayers();
        FloorLayer = AddLayer(new TileLayer<int>("floor", Width, Height, -1, 54));
        WallLayer = AddLayer(new TileLayer<int>("walls", Width, Height, 1));
        WallLayer.RendererKey = "walls";
        ObjectLayer = AddLayer(new ObjectLayer<RayGameObject>("objects", Width, Height));
        CeilingLayer = AddLayer(new TileLayer<int>("ceiling", Width, Height, -1, 0));

        // Walls first
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var tile = mapFile.Walls[x][y];
                if (tile <= 0)
                    continue;
                else 
                    WallLayer.SetTileValue(x, y, tile);
            }

        foreach (var item in mapFile.Items)
        {
            switch (item.ItypeType)
            {
                case ItemType.Door:
                    RayGame.Instance.SpawnDoor(item.Name, item.X, item.Y);
                    break;
                case ItemType.Special:
                    if (item.Name == "Pushwall")
                        RayGame.Instance.SpawnSecretWall(item.X, item.Y);
                    else if (item.Name.StartsWith("Start"))
                    {
                        RayGame.Instance.SetPlayerPosition(item.X, item.Y);
                        RayGame.Instance.SetPlayerFacing(GetDir(item));
                    }
                    break;
                case ItemType.Object:
                    RayGame.Instance.SpawnStatic(item.Name, item.X, item.Y);
                    break;
            }
        }

        static Vector3 GetDir(SaveItem item) => item.Name.Split(' ')[1][1] switch
        {
            'W' => Vector3.Left,
            'E' => Vector3.Right,
            'N' => Vector3.Down,
            _ => Vector3.Up
        };
    }
}
