﻿using Merthsoft.MooseEngine.Interface;
using MonoGame.Extended.Tiled;

namespace Merthsoft.MooseEngine.TiledDriver
{
    public record TiledMooseTile(TiledMapTile Tile) : ITile<TiledMapTile>
    {
        public bool IsBlocking(IMap map)
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
}
