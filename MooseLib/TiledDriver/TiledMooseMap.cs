using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using Merthsoft.MooseEngine.Interface;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Merthsoft.MooseEngine.Tiled
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

        protected virtual Grid BaseGrid
            => Grid.CreateGridWithLateralConnections(
                new GridSize(Width, Height),
                new Size(Distance.FromMeters(1), Distance.FromMeters(1)),
                Velocity.FromMetersPerSecond(1));

        public Grid BuildCollisionGrid(params Vector2[] walkableOverrides)
            => BaseGrid.DisconnectWhere((x, y) => blockingMap[x, y].Sum() > 0 && !walkableOverrides.Contains(new(x, y)));
        
        public bool CellIsInBounds(Vector2 cell)
            => cell.X > 0 && cell.X < Width
            && cell.Y > 0 && cell.Y < Height;

        public bool CellIsInBounds(int cellX, int cellY)
            => cellX > 0 && cellX < Width
            && cellY > 0 && cellY < Height;

        public virtual IEnumerable<RayCell> FindWorldRay(Vector2 startWorldPosition, Vector2 endWorldPosition, bool fillCorners = false, bool extend = false)
        {
            var (x1, y1) = (endWorldPosition.X, endWorldPosition.Y);
            var (x2, y2) = (startWorldPosition.X, startWorldPosition.Y);

            var deltaX = (int)Math.Abs(x1 - x2);
            var deltaZ = (int)Math.Abs(y1 - y2);

            if (deltaX == 0 && deltaZ == 0)
                yield break;

            var stepX = x2 < x1 ? 1 : -1;
            var stepZ = y2 < y1 ? 1 : -1;

            var err = deltaX - deltaZ;

            RayCell BuildReturnTuple(float x, float y)
                => new(new Vector2(x, y), blockingMap[(int)(x / TileWidth), (int)(y / TileHeight)]);

            while (true)
            {
                if (!WorldPositionIsInBounds(x2, y2))
                    break;

                yield return BuildReturnTuple(x2, y2);
                if (!extend && x2 == x1 && y2 == y1)
                    break;

                var e2 = 2 * err;

                if (e2 > -deltaZ)
                {
                    err -= deltaZ;
                    x2 += stepX;
                }

                if (!WorldPositionIsInBounds(x2, y2))
                    break;

                if (fillCorners)
                    yield return BuildReturnTuple(x2, y2);

                if (!extend && x2 == x1 && y2 == y1)
                    break;

                if (e2 < deltaX)
                {
                    err += deltaX;
                    y2 += stepZ;
                }
            }
        }

        public virtual IEnumerable<Vector2> FindCellPath(Vector2 startCell, Vector2 endCell, Grid? grid = null)
        {
            if (!CellIsInBounds(startCell) || !CellIsInBounds(endCell))
                return Enumerable.Empty<Vector2>();

            grid ??= BuildCollisionGrid(startCell);

            var startX = (int)startCell.X;
            var startY = (int)startCell.Y;
            var endX = (int)endCell.X;
            var endY = (int)endCell.Y;

            var path = new PathFinder()
                .FindPath(new GridPosition(startX, startY), new GridPosition(endX, endY), grid);

            if (path.Type != PathType.Complete)
                return Enumerable.Empty<Vector2>();

            return path.Edges
                .Select(e => new Vector2((int)e.End.Position.X, (int)e.End.Position.Y))
                .Distinct();
        }

        public bool WorldPositionIsInBounds(float worldX, float worldY)
            => worldX > 0 && worldX < Width * TileWidth
            && worldY > 0 && worldY < Height * TileHeight;

        public ReadOnlyCollection<int> GetBlockingVectorFromWorldPosition(Vector2 worldPosition)
            => blockingMap[(int)(worldPosition.X / TileWidth), (int)(worldPosition.Y / TileHeight)].AsReadOnly();
    }
}
