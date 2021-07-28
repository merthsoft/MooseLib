using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MooseLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib.Tiled
{
    public record TiledMooseMap(TiledMap Map) : IMap
    {
        public int Height => Map.Height;
        public int Width => Map.Width;
        public int TileWidth => Map.TileWidth;
        public int TileHeight => Map.TileHeight;

        private readonly List<ILayer> layerCache = new();
        public IReadOnlyList<ILayer> Layers => layerCache.AsReadOnly();

        public TiledMooseMap(string name, int width, int height, int tileWidth, int tileHeight, 
            TiledMapTileDrawOrder renderOrder = TiledMapTileDrawOrder.RightDown, 
            TiledMapOrientation orientation = TiledMapOrientation.Orthogonal, 
            Color? backgroundColor = null)
            : this(new TiledMap(name, width, height, tileWidth, tileHeight, renderOrder, orientation, backgroundColor)) 
        {
            BuildLayerCache();
        }

        private void BuildLayerCache()
        {
            layerCache.Clear();
            layerCache.AddRange(Map.Layers.Select((l, _) => l switch
            {
                TiledMapTileLayer tileLayer => new TiledMooseTileLayer(tileLayer) as ILayer,
                TiledMapObjectLayer objectLayer => new TiledMooseObjectLayer(objectLayer) as ILayer,
                _ => throw new Exception("Can't handle this type of layer"),
            }));
        }

        public void CopyFromMap(IMap fromMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
        {
            var sourceMap = (fromMap as TiledMooseMap)?.Map;
            if (sourceMap == null)
                throw new Exception("Can only copy another TiledMap");

            foreach (var tileset in sourceMap.Tilesets)
                if (!Map.Tilesets.Contains(tileset))
                    Map.AddTileset(tileset, sourceMap.GetTilesetFirstGlobalIdentifier(tileset));

            for (var layerIndex = 0; layerIndex < sourceMap.Layers.Count; layerIndex++)
            {
                var layer = sourceMap.Layers[layerIndex];

                switch (layer)
                {
                    case TiledMapObjectLayer objectLayer:
                        // TODO: Convert tiled objects to game objects
                        Map.AddLayer(new TiledMapObjectLayer(objectLayer.Name, objectLayer.Objects.Clone() as TiledMapObject[]));
                        break;
                    case TiledMapTileLayer sourceLayer:
                        if (layerIndex >= Map.TileLayers.Count)
                            Map.AddLayer(new TiledMapTileLayer(sourceLayer.Name, Map.Width, Map.Height, Map.TileWidth, Map.TileHeight));
                        var destLayer = (Map.Layers[layerIndex] as TiledMapTileLayer)!;
                        for (ushort x = 0; x < (width ?? sourceMap.Width); x++)
                            for (ushort y = 0; y < (height ?? sourceMap.Height); y++)
                                destLayer.SetTile
                                    ((ushort)(x + destX), (ushort)(y + destY), 
                                    (uint)sourceLayer.GetTile((ushort)(x + sourceX), (ushort)(y + sourceY)).GlobalIdentifier);
                        break;
                }
            }

            BuildLayerCache();
        }
    }
}
