using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MooseLib.Defs;
using MooseLib.GameObjects;
using MooseLib.Interface;
using MooseLib.Tiled;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib
{
    public abstract class MooseGame : Game
    {
        public MooseContentManager ContentManager { get; set; }

        public IMapRenderer MapRenderer = null!;
        public OrthographicCamera MainCamera = null!;
        public SpriteBatch SpriteBatch = null!;

        public GraphicsDeviceManager Graphics { get; set; }

        public IMap MainMap = null!;

        public IDictionary<string, Def> Defs { get; } = new Dictionary<string, Def>();

        public MouseState PreviousMouseState { get; private set; }

        public MouseState CurrentMouseState { get; private set; }

        public KeyboardState PreviousKeyState { get; private set; }

        public KeyboardState CurrentKeyState { get; private set; }

        public Vector2 WorldMouse { get; private set; }

        private readonly SortedSet<GameObjectBase> Objects = new();
        public IReadOnlyList<GameObjectBase> ReadObjects => Objects.ToList().AsReadOnly();
        private readonly Queue<GameObjectBase> ObjectsToAdd = new();

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
            ContentManager = new MooseContentManager(Content);
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
            MainMap = new TiledMooseMap("map", width, height, tileWith, tileHeight);
            BlockingMap = new List<int>[width, height];
        }

        protected abstract void Load();

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = new(0, 0) };
            MapRenderer = new TiledMooseMapRenderer(GraphicsDevice);

            Load();

            Defs.AsParallel().ForEach(kvp => kvp.Value.LoadContent(ContentManager));
        }

        protected void LoadMap()
        {
            MapRenderer.LoadMap(MainMap);
            BuildFullBlockingMap();
        }

        public void AddObject(GameObjectBase gameObject)
            => ObjectsToAdd.Enqueue(gameObject);

        public void AddDef<TDef>(TDef def) where TDef : Def
            => Defs[def.DefName] = def;

        public TDef GetDef<TDef>(string defName) where TDef : Def
            => ((Defs.GetValueOrDefault(defName) ?? Def.Empty) as TDef)!;

        protected virtual void PreMapUpdate(GameTime gameTime) { return; }
        protected virtual void PreObjectsUpdate(GameTime gameTime) { return; }
        protected virtual void PreUpdateBlockingMap(GameTime gameTime) { return; }
        protected virtual void PostUpdate(GameTime gameTime) { return; }

        protected override void Update(GameTime gameTime)
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            WorldMouse = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y).GetFloor();

            PreMapUpdate(gameTime);
            MapRenderer.Update(gameTime);

            PreObjectsUpdate(gameTime);
            Objects.ForEach(obj => obj.Update(gameTime));
            Objects.RemoveWhere(obj => obj.RemoveFlag);

            ObjectsToAdd.ForEach(obj =>Objects.Add(obj));

            ObjectsToAdd.Clear();

            PreUpdateBlockingMap(gameTime);
            UpdateBlockingMap();

            PostUpdate(gameTime);
        }

        public IEnumerable<GameObjectBase> ObjectsAtLayer(int layerIndex)
            => Objects
                .SkipWhile(o => o.Layer < layerIndex)
                .TakeWhile(o => o.Layer == layerIndex);

        protected virtual void UpdateBlockingMap()
        {
            var layerGroups = BuildObjectLayerGroups();

            foreach (var layerIndex in ObjectLayerIndices)
                for (var x = 0; x < MapWidth; x++)
                    for (var y = 0; y < MapHeight; y++)
                        BlockingMap[x, y][layerIndex] =
                            layerGroups.TryGetValue(layerIndex, out var set) ? set.Count(o => o.InCell(x, y, MainMap)) : 0;
        }

        private Dictionary<int, HashSet<GameObjectBase>> BuildObjectLayerGroups()
        {
            var layerGroups = new Dictionary<int, HashSet<GameObjectBase>>();
            foreach (var obj in Objects)
            {
                layerGroups.TryAdd(obj.Layer, new());
                layerGroups[obj.Layer].Add(obj);
            }

            return layerGroups;
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
                        var value = 0;
                        var layer = MainMap.Layers[layerIndex];
                        switch (layer)
                        {
                            case IObjectLayer:
                                value = Objects.Any(o => o.InCell(layerIndex, x, y, MainMap)) ? 1 : 0;
                                objectLayerIndices.Add(layerIndex);
                                break;
                            case ITileLayer tileLayer:
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

            var objectGroups = BuildObjectLayerGroups();

            for (var layerIndex = 0; layerIndex < MainMap.Layers.Count; layerIndex++)
            {
                var hookTuple = renderHooks?.ElementAtOrDefault(layerIndex);
                var layer = MainMap.Layers[layerIndex];
                switch (layer)
                {
                    case ITileLayer tileLayer:
                        hookTuple?.preHook?.Invoke(layerIndex);
                        MapRenderer.Draw(tileLayer, transformMatrix);
                        hookTuple?.postHook?.Invoke(layerIndex);
                        break;
                    case IObjectLayer:
                        SpriteBatch.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                        hookTuple?.preHook?.Invoke(layerIndex);
                        objectGroups[layerIndex]?.ForEach(unit => unit.Draw(SpriteBatch));
                        hookTuple?.postHook?.Invoke(layerIndex);
                        SpriteBatch.End();
                        break;
                }
            }
        }

        public bool CellIsInBounds(Vector2 cell)
            => cell.X > 0 && cell.X < MapWidth
            && cell.Y > 0 && cell.Y < MapHeight;

        public bool CellIsInBounds(int cellX, int cellY)
            => cellX > 0 && cellX < MapWidth
            && cellY > 0 && cellY < MapHeight;

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
                => new(new Vector2(x, y), BlockingMap[(int)(x / TileWidth), (int)(y / TileHeight)]);

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

        public bool WorldPositionIsInBounds(float worldX, float worldY)
            => worldX > 0 && worldX < MapWidth * TileWidth
            && worldY > 0 && worldY < MapHeight * TileHeight;

        public List<int> GetBlockingVectorFromWorldPosition(Vector2 worldPosition)
            => BlockingMap[(int)(worldPosition.X / TileWidth), (int)(worldPosition.Y / TileHeight)];
    }
}
