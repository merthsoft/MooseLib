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

    protected override int IsBlockedAt(string layer, int x, int y)
        => layer switch
        {
            "walls" => WallLayer.GetTileValue(x, y),
            "objects" => ObjectLayer.GetObjects(x, y).Count(o => o.Blocking),
            _ => 0
        };
    
    internal void Load(RayGame game, MapFile mapFile)
    {
        height = mapFile.Height;
        width = mapFile.Width;
        
        ClearLayers();
        FloorLayer = AddLayer(new TileLayer<int>("floor", Width, Height, -1, 54));
        WallLayer = AddLayer(new TileLayer<int>("walls", Width, Height, 1));
        WallLayer.RendererKey = "walls";
        ObjectLayer = AddLayer(new ObjectLayer<RayGameObject>("objects", Width, Height));
        CeilingLayer = AddLayer(new TileLayer<int>("ceiling", Width, Height, -1, 0));

        var index = 0;

        // Walls first
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                foreach (var layer in mapFile.Layers)
                {
                    var tile = layer.Data[index] - 1;
                    if (tile <= 0)
                        continue;
                    else if (tile < doorsOffset)
                        WallLayer.SetTileValue(x, y, tile);
                }
                index++;
            }
        // Everything else after so we can analyze the walls as needed for secrets, doors, etc.
        index = 0;
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                foreach (var layer in mapFile.Layers)
                {
                    var tile = layer.Data[index] - 1;
                    if (tile < doorsOffset)
                        continue;
                    else if (tile < staticOffset) // Doors
                        game.SpawnDoor(x, y, WallLayer.GetTileValue(x - 1, y) > 0, tile);
                    else if (tile < actorOffset) // Static objects
                    {
                        tile -= staticOffset;
                        if (WallLayer.GetTileValue(x, y) > 0)
                            game.SpawnStaticOverlay(tile, x, y);
                        else
                            game.SpawnStatic(tile, x, y);
                    }
                    else if (tile < specialOffset) // Actors
                        game.SpawnActor(((tile - actorOffset) / 4) switch
                        {
                            0 => "Guard",
                            1 => "SS",
                            2 => "Officer",
                            _ => throw new Exception(),
                        }, x, y, ((tile - actorOffset) % 4) switch
                        {
                            0 => Vector3.Left,
                            1 => Vector3.Right,
                            2 => Vector3.Down,
                            _ => Vector3.Up,
                        });
                    else if (tile < animatedOffset) // Special
                    {
                        tile -= specialOffset;
                        if (tile == 0)
                        {
                            var wall = WallLayer.GetTileValue(x, y);
                            game.SpawnSecretWall(wall, x, y);
                            WallLayer.SetTileValue(x, y, -1);
                        }
                        else if (tile is >= 1 and <= 4)
                        {
                            game.SetPlayerPosition(x, y);
                            game.SetPlayerFacing(tile switch
                            {
                                1 => Vector3.Left,
                                2 => Vector3.Right,
                                3 => Vector3.Down,
                                _ => Vector3.Up,
                            });
                        }
                        else if (tile is 5 or 6)
                        {
                            WallLayer.SetTileValue(x, y, -1); // Just in case
                            game.SpawnElevator(tile == 5, x, y);
                        }
                    } else // Animated
                    {
                        tile -= animatedOffset;
                        if (tile == 0)
                            game.SpawnActor("BlinkingLight", x, y, Vector3.Zero);
                    }
                }
                index++;
            }
    }
}
