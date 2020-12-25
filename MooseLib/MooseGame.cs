﻿using Microsoft.Xna.Framework;
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
            Objects.ForEach(unit => unit.Update(gameTime));

            BuildGrid();
        }

        protected void BuildGrid()
        {
            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                    BlockingMap[x, y] = MainMap.IsBlockedAt(x, y) || Objects.Any(u => u.InCell(x, y)) ? 100 : 1;
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
                        Objects
                            .SkipWhile(unit => unit.Layer > layerIndex)
                            .TakeWhile(unit => unit.Layer <= layerIndex)
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

        public IEnumerator<(Vector2 worldPosition, bool hit)> FindWorldRay(Vector2 startWorldPosition, Vector2 endWorldPosition)
        {
            yield break;
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

        public GameObject? UnitAtWorldLocation(Vector2 worldLocation)
            => Objects.FirstOrDefault(unit => unit.AtWorldLocation(worldLocation));
    }
}
