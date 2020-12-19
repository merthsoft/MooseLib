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

        protected readonly List<Unit> Units = new();
        protected readonly Queue<Unit> SpawnQueue = new();
        protected readonly Dictionary<string, SpriteSheet> Animations = new();
        protected readonly List<Unit> SelectedUnits = new();

        protected TiledMapTileLayer BaseLayer = null!;
        protected TiledMapTileLayer UnderGroundUnitLayer = null!;
        protected TiledMapTileLayer GroundUnitLayer = null!;
        protected TiledMapTileLayer AboveGroundUnitLayer = null!;

        public int MapHeight => MainMap.Height;
        public int MapWidth => MainMap.Width;
        public int TileWidth => MainMap.TileWidth;
        public int TileHeight => MainMap.TileHeight;
        public Size2 TileSize => new(TileWidth, TileHeight); // TODO: Cache
        public Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2); // TODO: Cache

        protected byte[,] BlockingMap = null!;

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
            
            BaseLayer = new TiledMapTileLayer("Base Layer", width, height, tileWith, tileHeight);
            UnderGroundUnitLayer = new TiledMapTileLayer("Under Ground Unit Layer", width, height, tileWith, tileHeight);
            GroundUnitLayer = new TiledMapTileLayer("Ground Unit Layer", width, height, tileWith, tileHeight);
            AboveGroundUnitLayer = new TiledMapTileLayer("Above Ground Unit Layer", width, height, tileWith, tileHeight);

            MainMap.AddLayer(BaseLayer);
            MainMap.AddLayer(UnderGroundUnitLayer);
            MainMap.AddLayer(GroundUnitLayer);
            MainMap.AddLayer(AboveGroundUnitLayer);

            BlockingMap = new byte[width, height];
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
            if (SpawnQueue.Count > 0)
                Units.Add(SpawnQueue.Dequeue());
            Units.ForEach(unit => unit.Update(gameTime));

            BuildGrid();
        }

        protected void BuildGrid()
        {
            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                    BlockingMap[x, y] = MainMap.IsBlockedAt(x, y) || Units.Exists(u => u.InCell(x, y)) ? 100 : 1;
        }

        protected override void Draw(GameTime gameTime)
            => Draw();

        protected void Draw(Action<Unit>? selectedUnitPreDraw = null, Action<Unit>? selectedUnitPostDraw = null)
        {
            GraphicsDevice.Clear(Color.Black);

            var transformMatrix = MainCamera.GetViewMatrix();

            MapRenderer.Draw(BaseLayer, transformMatrix);
            MapRenderer.Draw(UnderGroundUnitLayer, transformMatrix);

            SpriteBatch.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            
            if (selectedUnitPreDraw != null)
                foreach (var selectedUnit in SelectedUnits)
                    selectedUnitPreDraw(selectedUnit);
            
            SpriteBatch.End();

            MapRenderer.Draw(GroundUnitLayer, transformMatrix);

            SpriteBatch.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            Units.ForEach(unit => unit.Draw(SpriteBatch));

            if (selectedUnitPostDraw != null)
                foreach (var selectedUnit in SelectedUnits)
                    selectedUnitPostDraw(selectedUnit);

            SpriteBatch.End();

            MapRenderer.Draw(AboveGroundUnitLayer, transformMatrix);
        }

        protected Unit AddUnit(string animationKey, int cellX, int cellY, Direction direction = Direction.None, State state = State.Idle, int speed = 0)
        {
            if (!Animations.ContainsKey(animationKey))
                LoadAnimation(animationKey);
            var unit = new Unit(this, Animations[animationKey], cellX, cellY, direction, state) { Speed = speed };
            Units.Add(unit);
            return unit;
        }

        protected Unit AddUnitToSpawnQueue(string animationKey, int cellX, int cellY, Direction direction = Direction.None, State state = State.Idle, int speed = 0)
        {
            if (!Animations.ContainsKey(animationKey))
                LoadAnimation(animationKey);
            var unit = new Unit(this, Animations[animationKey], cellX, cellY, direction, state) { Speed = speed };
            SpawnQueue.Enqueue(unit);
            return unit;
        }

        protected void LoadAnimation(string animationKey) 
            => Animations[animationKey] = Content.Load<SpriteSheet>($"Animations/{animationKey}.sf", new JsonContentLoader());

        public bool CellIsInBounds(Vector2 cell)
            => cell.X > 0 && cell.X < MapWidth
            && cell.Y > 0 && cell.Y < MapHeight;

        public IEnumerable<Vector2> FindPath(Vector2 startCell, Vector2 endCell)
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
                    if (BlockingMap[x, y] > 1 && !(x == startX && y == startY))
                        grid.DisconnectNode(new(x, y));

            var path = new PathFinder()
                .FindPath(new GridPosition(startX, startY), new GridPosition(endX, endY), grid);

            if (path.Type != PathType.Complete)
                return Enumerable.Empty<Vector2>();
            
            return path.Edges
                .Select(e => new Vector2((int)e.End.Position.X, (int)e.End.Position.Y))
                .Distinct();
        }

        public Unit? UnitAtWorldLocation(Vector2 worldLocation)
            => Units.FirstOrDefault(unit => unit.AtWorldLocation(worldLocation));
    }
}
