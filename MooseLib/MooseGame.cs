using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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

        private readonly Dictionary<string, ILayerRenderer> Renderers = new();

        public int MapHeight => MainMap.Height;

        public int MapWidth => MainMap.Width;

        public int TileWidth => MainMap.TileWidth;

        public int TileHeight => MainMap.TileHeight;

        public Size2 TileSize => new(TileWidth, TileHeight); // TODO: Cache

        public Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2); // TODO: Cache

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
        }

        protected abstract void Load();

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = new(0, 0) };

            Load();

            Defs.AsParallel().ForEach(kvp => kvp.Value.LoadContent(ContentManager));
        }

        protected void LoadMap()
        {
            foreach (var renderer in Renderers.Values)
                renderer.Load(MainMap);
        }

        public void AddRenderer(string rendererKey, ILayerRenderer renderer)
            => Renderers[rendererKey] = renderer;

        public TObject AddObject<TObject>(TObject gameObject, IMap? parentMap = null) where TObject : GameObjectBase
        {
            ObjectsToAdd.Enqueue(gameObject);
            gameObject.ParentMap = parentMap ?? MainMap;
            return gameObject;
        }

        public TDef AddDef<TDef>(TDef def) where TDef : Def
        {
            Defs[def.DefName] = def;
            return def;
        }

        public TDef GetDef<TDef>(string defName) where TDef : Def
            => ((Defs.GetValueOrDefault(defName) ?? Def.Empty) as TDef)!;

        protected virtual void PreRenderUpdate(GameTime gameTime) { return; }
        protected virtual void PreObjectsUpdate(GameTime gameTime) { return; }
        protected virtual void PostObjectsUpdate(GameTime gameTime) { return; }
        protected virtual void PreMapUpdate(GameTime gameTime) { return; }
        protected virtual void PostUpdate(GameTime gameTime) { return; }

        protected override void Update(GameTime gameTime)
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            WorldMouse = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y).GetFloor();

            PreRenderUpdate(gameTime);
            foreach (var renderer in Renderers.Values)
                renderer.Update(gameTime);

            PreObjectsUpdate(gameTime);
            foreach (var obj in Objects)
                obj.Update(gameTime);
            PostObjectsUpdate(gameTime);

            foreach (var obj in Objects)
                if (obj.RemoveFlag && obj.ParentMap != null && obj.ParentMap.Layers[obj.Layer] is IObjectLayer layer)
                {
                    layer.RemoveObject(obj);
                    obj.ParentMap = null;
                }

            Objects.RemoveWhere(obj => obj.RemoveFlag);

            foreach (var obj in ObjectsToAdd)
            {
                if (MainMap.Layers[obj.Layer] is not IObjectLayer layer)
                    throw new Exception("Cannot add object to non-object layer");
                layer.AddObject(obj);
                Objects.Add(obj);
            }

            ObjectsToAdd.Clear();

            PreMapUpdate(gameTime);
            MainMap.Update(gameTime);

            PostUpdate(gameTime);
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
                var renderer = Renderers[layer.RendererKey];

                renderer.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                hookTuple?.preHook?.Invoke(layerIndex);
                renderer.Draw(layer);
                hookTuple?.postHook?.Invoke(layerIndex);
                renderer.End();
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
