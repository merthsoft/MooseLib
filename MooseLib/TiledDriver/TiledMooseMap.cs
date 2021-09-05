﻿using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MooseEngine.TiledDriver
{
    public class TiledMooseMap : BaseMap
    {
        public TiledMap Map { get; protected set; }

        public override int Height => Map.Height;
        public override int Width => Map.Width;
        public override int TileWidth => Map.TileWidth;
        public override int TileHeight => Map.TileHeight;

        protected readonly List<ILayer> layerCache = new();
        public override IReadOnlyList<ILayer> Layers => layerCache.AsReadOnly();

        public TiledMooseMap(TiledMap map)
        {
            Map = map;
            BuildLayerCache();
        }

        public TiledMooseMap(string name, int width, int height, int tileWidth, int tileHeight, 
            TiledMapTileDrawOrder renderOrder = TiledMapTileDrawOrder.RightDown, 
            TiledMapOrientation orientation = TiledMapOrientation.Orthogonal, 
            Color? backgroundColor = null)
            : this(new TiledMap(name, width, height, tileWidth, tileHeight, renderOrder, orientation, backgroundColor)) 
        {
            BuildLayerCache();
        }

        protected virtual void BuildLayerCache()
        {
            layerCache.Clear();

            for (var layerIndex = 0; layerIndex < Map.Layers.Count; layerIndex++)
            {
                var mooseLayer = Map.Layers[layerIndex] switch
                {
                    TiledMapTileLayer tileLayer => new TiledMooseTileLayer(tileLayer),
                    TiledMapObjectLayer objectLayer => new TiledMooseObjectLayer(objectLayer),
                    _ => null as ILayer
                };
                if (mooseLayer != null)
                    layerCache.Add(mooseLayer);
            }

            BuildFullBlockingMap();
        } 

        public override void CopyFromMap(IMap fromMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
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

        protected override int IsBlockedAt(int layer, int x, int y)
            => layerCache[layer] switch
            {
                TiledMooseObjectLayer objectLayer => objectLayer.Objects.Any(o => o.InCell(layer, x, y, this)) ? 1 : 0,
                TiledMooseTileLayer tileLayer => tileLayer.IsBlockedAt(x, y, this) ? 1 : 0,
                _ => 0,
            };
    }
}
