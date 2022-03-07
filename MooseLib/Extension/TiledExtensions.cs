using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.TiledDriver;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MooseEngine.Extension;

public static class TiledExtensions
{

    public static bool? GetBoolProperty(this TiledMapProperties? poperties, string name)
    {
        if (poperties == null)
            return false;

        var property = poperties.FirstOrDefault(p => p.Key == name).Value;
        if (property == null)
            return null;

        return bool.TryParse(property, out var result) && result;
    }

    public static bool IsBlocking(this TiledMapTile Tile, IMap map)
    {
        var tiledMap = (map as TiledMooseMap)?.Map;
        if (tiledMap == null)
            return true;

        var tileSet = tiledMap.GetTilesetByTileGlobalIdentifier(Tile.GlobalIdentifier);
        if (tileSet == null)
            return false;

        var firstTile = tiledMap.GetTilesetFirstGlobalIdentifier(tileSet);
        var tileSetTile = tileSet.Tiles.FirstOrDefault(t => t.LocalTileIdentifier == Tile.GlobalIdentifier - firstTile);

        return tileSetTile?.Properties.GetBoolProperty("blocking")
            ?? tileSet.Properties.GetBoolProperty("blocking")
            ?? false;
    }
}
