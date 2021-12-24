using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.Moose.Miner
{
    public class MinerGame : MooseGame
    {
        public const int BaseMapWidth = 80;
        public const int BaseMapHeight = 80;
        public const int BaseTileWidth = 16;
        public const int BaseTileHeight = 24;

        public MineMap Map => (ActiveMaps[0] as MineMap)!;
        public MineLayerRenderer MineLayerRenderer => GetRenderer<MineLayerRenderer>("mine")!;

        MinerPlayerDef PlayerDef { get; } = new();
        MinerPlayer Player { get; }

        Dictionary<Tile, TreasureDef> TreasureDefs { get; } = new();

        public MinerGame()
        {
            Player = new MinerPlayer(PlayerDef);
        }

        protected override void Load()
        {
            DefaultBackgroundColor = new(170, 85, 0);
            var tiles = Content.Load<Texture2D>("sprites");

            AddDefaultRenderer<MineLayer>("mine", new MineLayerRenderer(SpriteBatch, BaseTileWidth, BaseTileHeight, tiles));
            AddDefaultRenderer<ObjectLayer>("player", new SpriteBatchObjectRenderer(SpriteBatch));

            foreach (var tile in Enum.GetValues<Tile>().SkipWhile(t => t < Tile.Silver).TakeWhile(t => t < Tile.Sky))
            {
                TreasureDefs[tile] = new TreasureDef(tile, tiles);
                AddDef(TreasureDefs[tile]);
            }

            ActiveMaps.Add(new MineMap());
            AddDef(PlayerDef);

            ZoomIn(1);
            SetScreenSize(1280, 960);
        }

        protected override void PostLoad()
        {
            AddObject(Player);

            Reset();
        }

        protected override void PostObjectsUpdate(GameTime gameTime)
        {

            if (WasKeyJustPressed(Keys.R))
            {
                Reset();
                return;
            }

            HandleMovement();
        }

        private void Reset()
        {
            Map.MineLayer.Reset();
            MineLayerRenderer.GenerateDirtTexture(GraphicsDevice, MapWidth, MapHeight, TileWidth, TileHeight);
            Player.WorldPosition = new(0 * TileWidth, 5 * TileHeight);
            Player.Direction = MinerPlayer.Right;
            MarkAllObjectsForRemoval();
            Player.RemoveFlag = false;
        }

        private void HandleMovement()
        {
            var moveDelta = Vector2.Zero;
            if (WasKeyJustPressed(Keys.Left))
            {
                moveDelta = new(-1, 0);
                Player.Direction = MinerPlayer.Left;
            }
            else if (WasKeyJustPressed(Keys.Right))
            {
                moveDelta = new(1, 0);
                Player.Direction = MinerPlayer.Right;
            }
            else if (WasKeyJustPressed(Keys.Down))
                moveDelta = new(0, 1);
            else if (WasKeyJustPressed(Keys.Up))
                moveDelta = new(0, -1);


            if (moveDelta == Vector2.Zero)
                return;

            var playerCell = Player.GetCell();
            var uncovered = Map.Mine(playerCell + moveDelta);

            if (uncovered < Tile.Border)
            {
                Player.WorldPosition += new Vector2(moveDelta.X * BaseTileWidth, moveDelta.Y * BaseTileHeight);
                Map.SearchCell(Player.GetCell(), Player.HasTorch, Player.HasLantern);

                if (TreasureDefs.ContainsKey(uncovered))
                    SpawnTreasure(uncovered, Player.WorldPosition);
            }
        }

        private Treasure SpawnTreasure(Tile tile, Vector2 position)
        {
            var ret = new Treasure(TreasureDefs[tile]!, (int)position.X, (int)position.Y);
            AddObject(ret);
            return ret;
        }
    }
}
