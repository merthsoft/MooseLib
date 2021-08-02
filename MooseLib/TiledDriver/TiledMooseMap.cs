using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MooseLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib.Tiled
{
    public record TiledMooseMap : IMap
    {
        public TiledMap Map { get; protected set; }

        public int Height => Map.Height;
        public int Width => Map.Width;
        public int TileWidth => Map.TileWidth;
        public int TileHeight => Map.TileHeight;

        protected readonly List<ILayer> layerCache = new();
        public IReadOnlyList<ILayer> Layers => layerCache.AsReadOnly();
        
        private List<int>[,] blockingMap = new List<int>[0, 0];

        protected readonly HashSet<int> objectLayerIndices = new();
        public IEnumerable<int> ObjectLayerIndices => objectLayerIndices;

        protected readonly HashSet<int> tileLayerIndices = new();
        public IEnumerable<int> TileLayerIndices => tileLayerIndices;

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
            objectLayerIndices.Clear();
            tileLayerIndices.Clear();

            layerCache.AddRange(Map.Layers.Select((l, _) => l switch
            {
                TiledMapTileLayer tileLayer => new TiledMooseTileLayer(tileLayer) as ILayer,
                TiledMapObjectLayer objectLayer => new TiledMooseObjectLayer(objectLayer) as ILayer,
                _ => throw new Exception("Can't handle this type of layer"),
            }));
            
            for (var layerIndex = 0; layerIndex < Map.Layers.Count; layerIndex++)
            {
                var layer = Map.Layers[layerIndex];
                switch (layer)
                {
                    case TiledMapTileLayer tileLayer:
                        layerCache.Add(new TiledMooseTileLayer(tileLayer));
                        tileLayerIndices.Add(layerIndex);
                        break;

                    case TiledMapObjectLayer objectLayer:
                        layerCache.Add(new TiledMooseObjectLayer(objectLayer));
                        objectLayerIndices.Add(layerIndex);
                        break;
                }
            }

            BuildFullBlockingMap();
        }

        public virtual void CopyFromMap(IMap fromMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
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

        protected virtual void BuildFullBlockingMap()
        {
            blockingMap = new List<int>[Width, Height];

            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    blockingMap[x, y] = new List<int>();
                    for (var layerIndex = 0; layerIndex < layerCache.Count; layerIndex++)
                    {
                        var value = 0;
                        var layer = layerCache[layerIndex];
                        switch (layer)
                        {
                            case IObjectLayer objectLayer:
                                value = objectLayer.Objects.Any(o => o.InCell(layerIndex, x, y, this)) ? 1 : 0;
                                break;
                            case ITileLayer tileLayer:
                                value = tileLayer.IsBlockedAt(x, y, this) ? 1 : 0;
                                break;
                        }
                        blockingMap[x, y].Add(value);
                    }
                }
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateBlockingMap();
        }

        public virtual IEnumerable<int> GetBlockingMap(int x, int y)
            => blockingMap[x, y].AsEnumerable();


        protected virtual void UpdateBlockingMap()
        {
            foreach (var layerIndex in ObjectLayerIndices)
                for (var x = 0; x < Width; x++)
                    for (var y = 0; y < Height; y++)
                        blockingMap[x, y][layerIndex] = 
                            (layerCache[layerIndex] as IObjectLayer)!.Objects.Count(o => o.InCell(x, y, this));
        }
    }
}
