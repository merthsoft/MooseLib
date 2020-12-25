using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib
{
    public class MooseGame : Game
    {
        protected GraphicsDeviceManager Graphics { get; set; }
        
        protected TiledMap MainMap = null!;
        protected TiledMapRenderer MapRenderer = null!;
        protected OrthographicCamera MainCamera = null!;
        protected SpriteBatch SpriteBatch = null!;

        protected readonly SortedSet<GameObject> Objects = new();
        protected readonly Dictionary<string, SpriteSheet> Animations = new();

        public int MapHeight => MainMap.Height;
        public int MapWidth => MainMap.Width;
        public int TileWidth => MainMap.TileWidth;
        public int TileHeight => MainMap.TileHeight;
        public Size2 TileSize => new(TileWidth, TileHeight); // TODO: Cache
        public Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2); // TODO: Cache

        protected List<byte>[,] BlockingMap = null!;

        public MooseGame()
        {
            Content.RootDirectory = "Content";
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
            
            BlockingMap = new List<byte>[width, height];
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = new(0, 0) };
            MapRenderer = new TiledMapRenderer(GraphicsDevice);
            LoadMap();
        }

        protected void LoadMap()
        {
            MapRenderer.LoadMap(MainMap);
            BuildGrid();
        }

        protected override void Update(GameTime gameTime)
        {
            MapRenderer.Update(gameTime);
            Objects.ForEach(unit => unit.Update(gameTime));

            BuildGrid();
        }

        private IEnumerable<GameObject> ObjectsAtLayer(int layerIndex)
            => Objects
                .SkipWhile(o => o.Layer < layerIndex)
                .TakeWhile(o => o.Layer == layerIndex);

        protected void BuildGrid()
        {
            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                {
                    BlockingMap[x, y] = new List<byte>();
                    for (var layerIndex = 0; layerIndex < MainMap.Layers.Count; layerIndex++)
                    {
                        byte value = 0;
                        var layer = MainMap.Layers[layerIndex];
                        switch (layer)
                        {
                            case TiledMapObjectLayer:
                                value = ObjectsAtLayer(layerIndex).Any(o => o.InCell(x, y)) ? 1 : 0;
                                break;
                            case TiledMapTileLayer tileLayer:
                                value = tileLayer.IsBlockedAt(x, y, MainMap) ? 1 : 0;
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

        protected GameObject AddObject(string animationKey, int cellX, int cellY, string direction = Direction.None, string state = State.Idle, int objectLayerIndex = 0)
        {
            if (!Animations.ContainsKey(animationKey))
                LoadAnimation(animationKey);
            var unit = new GameObject(this, Animations[animationKey], cellX, cellY, direction, state, objectLayerIndex);
            Objects.Add(unit);
            return unit;
        }

        protected void LoadAnimation(string animationKey) 
            => Animations[animationKey] = Content.Load<SpriteSheet>($"Animations/{animationKey}.sf", new JsonContentLoader());

        public bool CellIsInBounds(Vector2 cell)
            => cell.X > 0 && cell.X < MapWidth
            && cell.Y > 0 && cell.Y < MapHeight;

        public bool CellIsInBounds(int cellX, int cellY)
            => cellX > 0 && cellX < MapWidth
            && cellY > 0 && cellY < MapHeight;

        public IEnumerable<(Vector2 worldPosition, IList<byte> blockedVector)> FindWorldRay(Vector2 startWorldPosition, Vector2 endWorldPosition, bool fillCorners = false)
        {
            var (x1, y1) = endWorldPosition;
            var (x2, y2) = startWorldPosition;

            var deltaX = (int)Math.Abs(x1 - x2);
            var deltaZ = (int)Math.Abs(y1 - y2);
            var stepX = x2 < x1 ? 1 : -1;
            var stepZ = y2 < y1 ? 1 : -1;
            
            var err = deltaX - deltaZ;

            (Vector2, List<byte> blocked) BuildReturnTuple(float x, float y)
            {
                var cellX = (int)(x / TileWidth);
                var cellY = (int)(y / TileHeight);
                var blocked = BlockingMap[cellX, cellY];
                var ret = (new Vector2(x, y), blocked);
                return ret;
            }

            while (true)
            {
                if (!WorldPositionIsInBounds(x2, y2))
                    break;

                yield return BuildReturnTuple(x2, y2);
                if (x2 == x1 && y2 == y1) 
                    break;

                int e2 = 2 * err;

                if (e2 > -deltaZ)
                {
                    err -= deltaZ;
                    x2 += stepX;
                }

                if (!WorldPositionIsInBounds(x2, y2))
                    break;

                if (fillCorners)
                    yield return BuildReturnTuple(x2, y2);
                
                if (x2 == x1 && y2 == y1) 
                    break;

                if (e2 < deltaX)
                {
                    err += deltaX;
                    y2 += stepZ;
                }
            }
        }

        public IEnumerable<Vector2> FindCellPath(Vector2 startCell, Vector2 endCell)
        {
            if (!CellIsInBounds(startCell) || !CellIsInBounds(endCell))
                return Enumerable.Empty<Vector2>();

            var startX = (int)startCell.X;
            var startY = (int)startCell.Y;
            var endX = (int)endCell.X;
            var endY = (int)endCell.Y;

            var grid = Grid.CreateGridWithLateralConnections(
                    new GridSize(MapWidth, MapHeight),
                    new Roy_T.AStar.Primitives.Size(Distance.FromMeters(1), Distance.FromMeters(1)),
                    Velocity.FromMetersPerSecond(1));

            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                    if (BlockingMap[x, y].Sum() > 0 && !(x == startX && y == startY))
                        grid.DisconnectNode(new(x, y));

            var path = new PathFinder()
                .FindPath(new GridPosition(startX, startY), new GridPosition(endX, endY), grid);

            if (path.Type != PathType.Complete)
                return Enumerable.Empty<Vector2>();
            
            return path.Edges
                .Select(e => new Vector2((int)e.End.Position.X, (int)e.End.Position.Y))
                .Distinct();
        }

        public GameObject? UnitAtWorldLocation(Vector2 worldLocation)
            => Objects.FirstOrDefault(unit => unit.AtWorldLocation(worldLocation));

        public bool WorldPositionIsInBounds(float worldX, float worldY)
            => worldX > 0 && worldX < MapWidth * TileWidth
            && worldY > 0 && worldY < MapHeight * TileHeight;
    }
}
