using EpPathFinding.cs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
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
        protected readonly Dictionary<string, SpriteSheet> Animations = new();
        protected readonly List<Unit> SelectedUnits = new();

        protected TiledMapTileLayer BaseLayer = null!;
        protected TiledMapTileLayer UnderGroundUnitLayer = null!;
        protected TiledMapTileLayer GroundUnitLayer = null!;
        protected TiledMapTileLayer AboveGroundUnitLayer = null!;

        protected int MapHeight => MainMap.Height;
        protected int MapWidth => MainMap.Width;
        protected int TileWidth => MainMap.TileWidth;
        protected int TileHeight => MainMap.TileHeight;
        protected Size2 TileSize => new(TileWidth, TileHeight); // TODO: Cache
        protected Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2); // TODO: Cache

        protected BaseGrid WalkableGrid = null!;

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

            WalkableGrid = new DynamicGrid();

            BaseLayer = new TiledMapTileLayer("Base Layer", width, height, tileWith, tileHeight);
            UnderGroundUnitLayer = new TiledMapTileLayer("Under Ground Unit Layer", width, height, tileWith, tileHeight);
            GroundUnitLayer = new TiledMapTileLayer("Ground Unit Layer", width, height, tileWith, tileHeight);
            AboveGroundUnitLayer = new TiledMapTileLayer("Above Ground Unit Layer", width, height, tileWith, tileHeight);

            MainMap.AddLayer(BaseLayer);
            MainMap.AddLayer(UnderGroundUnitLayer);
            MainMap.AddLayer(GroundUnitLayer);
            MainMap.AddLayer(AboveGroundUnitLayer);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = new(0, 0) };
            MapRenderer = new TiledMapRenderer(GraphicsDevice);
            LoadMap();
        }

        protected void LoadMap()
            => MapRenderer.LoadMap(MainMap);

        protected override void Update(GameTime gameTime)
        {
            MapRenderer.Update(gameTime);
            Units.ForEach(unit => unit.Update(gameTime));

            BuildGrid();
        }
        
        protected void BuildGrid()
        {
            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                    WalkableGrid.SetWalkableAt(x, y, !MainMap.IsBlockedAt(x, y) && !Units.Exists(u => u.InCell(MainMap, x, y)));
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
            var unit = new Unit(Animations[animationKey], cellX, cellY, direction, state) { Speed = speed };
            Units.Add(unit);
            return unit;
        }

        protected void LoadAnimation(string animationKey) 
            => Animations[animationKey] = Content.Load<SpriteSheet>($"Animations/{animationKey}.sf", new JsonContentLoader());
    }
}
