using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MooseLib.GameObjects;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace MooseLib
{
    public abstract class MooseGame : Game
    {
        public GraphicsDeviceManager Graphics { get; set; }

        public TiledMap MainMap = null!;
        public TiledMapRenderer MapRenderer = null!;
        public OrthographicCamera MainCamera = null!;
        public SpriteBatch SpriteBatch = null!;

        public MouseState PreviousMouseState { get; private set; }

        public MouseState CurrentMouseState { get; private set; }

        public Vector2 WorldMouse { get; private set; }

        public readonly SortedSet<AnimatedGameObject> Objects = new();
        public readonly Queue<AnimatedGameObject> ObjectsToAdd = new();
        public readonly Dictionary<string, SpriteSheet> AnimationSpriteSheets = new();

        public int MapHeight => MainMap.Height;

        public int MapWidth => MainMap.Width;

        public int TileWidth => MainMap.TileWidth;

        public int TileHeight => MainMap.TileHeight;

        public Size2 TileSize => new(TileWidth, TileHeight); // TODO: Cache

        public Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2); // TODO: Cache


        protected readonly HashSet<int> objectLayerIndices = new();

        public IEnumerable<int> ObjectLayerIndices => objectLayerIndices;

        protected readonly HashSet<int> tileLayerIndices = new();

        public IEnumerable<int> TileLayerIndices => tileLayerIndices;

        public List<int>[,] BlockingMap { get; private set; } = new List<int>[0, 0];

        public MooseGame()
        {
            Content.RootDirectory = nameof(Content);
            IsMouseVisible = true;
            Graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            base.Initialize();
        }

        protected void InitializeMap(int width, int height, int tileWith, int tileHeight)
        {
            MainMap = new TiledMap("map", width, height, tileWith, tileHeight, TiledMapTileDrawOrder.RightDown, TiledMapOrientation.Orthogonal);
            BlockingMap = new List<int>[width, height];
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = new(0, 0) };
            MapRenderer = new TiledMapRenderer(GraphicsDevice);
        }

        protected void LoadMap()
        {
            MapRenderer.LoadMap(MainMap);
            BuildFullBlockingMap();
        }

        protected virtual void PreMapUpdate(GameTime gameTime) { return; }
        protected virtual void PreObjectsUpdate(GameTime gameTime) { return; }
        protected virtual void PreUpdateBlockingMap(GameTime gameTime) { return; }
        protected virtual void PostUpdate(GameTime gameTime) { return; }


        protected override void Update(GameTime gameTime)
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            WorldMouse = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y).GetFloor();

            PreMapUpdate(gameTime);
            MapRenderer.Update(gameTime);

            PreObjectsUpdate(gameTime);
            Objects.ForEach(obj => obj.Update(gameTime));
            Objects.RemoveWhere(obj => obj.RemoveFlag);

            ObjectsToAdd.ForEach(obj => Objects.Add(obj));
            ObjectsToAdd.Clear();

            PreUpdateBlockingMap(gameTime);
            UpdateBlockingMap();

            PostUpdate(gameTime);
        }

        public IEnumerable<AnimatedGameObject> ObjectsAtLayer(int layerIndex)
            => Objects
                .SkipWhile(o => o.Layer < layerIndex)
                .TakeWhile(o => o.Layer == layerIndex);

        protected virtual void UpdateBlockingMap()
        {
            var layerGroups = new Dictionary<int, HashSet<AnimatedGameObject>>();
            foreach (var obj in Objects)
            {
                layerGroups.TryAdd(obj.Layer, new());
                layerGroups[obj.Layer].Add(obj);
            }

            foreach (var layerIndex in ObjectLayerIndices)
            {
                var layer = MainMap.Layers[layerIndex];
                switch (layer)
                {
                    case TiledMapObjectLayer:
                        for (var x = 0; x < MapWidth; x++)
                            for (var y = 0; y < MapHeight; y++)
                                BlockingMap[x, y][layerIndex] 
                                    = layerGroups.TryGetValue(layerIndex, out var set)
                                        ? set.Count(o => o.InCell(x, y))
                                        : 0;
                        break;
                }
            }
        }

        protected virtual void BuildFullBlockingMap()
        {
            objectLayerIndices.Clear();
            tileLayerIndices.Clear();
            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                {
                    BlockingMap[x, y] = new List<int>();
                    for (var layerIndex = 0; layerIndex < MainMap.Layers.Count; layerIndex++)
                    {
                        byte value = 0;
                        var layer = MainMap.Layers[layerIndex];
                        switch (layer)
                        {
                            case TiledMapObjectLayer:
                                value = ObjectsAtLayer(layerIndex).Any(o => o.InCell(x, y)) ? 1 : 0;
                                objectLayerIndices.Add(layerIndex);
                                break;
                            case TiledMapTileLayer tileLayer:
                                value = tileLayer.IsBlockedAt(x, y, MainMap) ? 1 : 0;
                                tileLayerIndices.Add(layerIndex);
                                break;
                        }
                        BlockingMap[x, y].Add(value);
                    }
                }
        }

        protected override void Draw(GameTime gameTime)
            => Draw();

        protected void Draw(params (Action<int>? preHook, Action<int>? postHook)?[] renderHooks)
        {
            GraphicsDevice.Clear(Color.Black);

            var transformMatrix = MainCamera.GetViewMatrix();

            for (var layerIndex = 0; layerIndex < MainMap.Layers.Count; layerIndex++)
            {
                var hookTuple = renderHooks?.ElementAtOrDefault(layerIndex);
                var layer = MainMap.Layers[layerIndex];
                switch (layer)
                {
                    case TiledMapTileLayer:
                        hookTuple?.preHook?.Invoke(layerIndex);
                        MapRenderer.Draw(layer, transformMatrix);
                        hookTuple?.postHook?.Invoke(layerIndex);
                        break;
                    case TiledMapObjectLayer:
                        SpriteBatch.Begin(transformMatrix: MainCamera.GetViewMatrix(), blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                        hookTuple?.preHook?.Invoke(layerIndex);
                        ObjectsAtLayer(layerIndex)
                            .ForEach(unit => unit.Draw(SpriteBatch));
                        hookTuple?.postHook?.Invoke(layerIndex);
                        SpriteBatch.End();
                        break;
                }
            }
        }

        public SpriteSheet LoadAnimatedSpriteSheet(string animationKey, bool replace = false)
        {
            if (replace || !AnimationSpriteSheets.ContainsKey(animationKey))
                AnimationSpriteSheets[animationKey] = Content.Load<SpriteSheet>($"Animations/{animationKey}.sf", new JsonContentLoader());

            return AnimationSpriteSheets[animationKey];
        }

        public bool CellIsInBounds(Vector2 cell)
            => cell.X > 0 && cell.X < MapWidth
            && cell.Y > 0 && cell.Y < MapHeight;

        public bool CellIsInBounds(int cellX, int cellY)
            => cellX > 0 && cellX < MapWidth
            && cellY > 0 && cellY < MapHeight;

        public virtual IEnumerable<(Vector2 worldPosition, IList<int> blockedVector)> FindWorldRay(Vector2 startWorldPosition, Vector2 endWorldPosition, bool fillCorners = false, bool extend = false)
        {
            var (x1, y1) = (endWorldPosition.X, endWorldPosition.Y);
            var (x2, y2) = (startWorldPosition.X, startWorldPosition.Y);

            var deltaX = (int)Math.Abs(x1 - x2);
            var deltaZ = (int)Math.Abs(y1 - y2);
            var stepX = x2 < x1 ? 1 : -1;
            var stepZ = y2 < y1 ? 1 : -1;

            var err = deltaX - deltaZ;

            (Vector2, List<int> blocked) BuildReturnTuple(float x, float y) 
                => (new Vector2(x, y), BlockingMap[(int)(x / TileWidth), (int)(y / TileHeight)]);

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

        public virtual Grid BaseGrid 
            => Grid.CreateGridWithLateralConnections(
                new GridSize(MapWidth, MapHeight), 
                new Roy_T.AStar.Primitives.Size(Distance.FromMeters(1), Distance.FromMeters(1)),
                Velocity.FromMetersPerSecond(1));

        protected virtual Grid BuildCollisionGrid(params Vector2[] walkableOverrides)
            => BaseGrid.DisconnectWhere((x, y) => BlockingMap[x, y].Sum() > 0 && !walkableOverrides.Contains(new(x, y)));

        public GameObjectBase? UnitAtWorldLocation(Vector2 worldLocation)
            => Objects.FirstOrDefault(unit => unit.AtWorldPosition(worldLocation));

        public bool WorldPositionIsInBounds(float worldX, float worldY)
            => worldX > 0 && worldX < MapWidth * TileWidth
            && worldY > 0 && worldY < MapHeight * TileHeight;
    }
}
