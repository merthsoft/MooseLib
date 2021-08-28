using Merthsoft.MooseEngine;
using Merthsoft.MooseEngine.BaseDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Merthsoft.Islands
{
    public class IslandGame : MooseGame
    {
        private const string TileRenderKey = "island_tile_renderer";

        private const int TileDimmensions = 8;
        private const float DefaultZoom = 1f;
        private const int MapSize = 80;
        private const int WindowSize = MapSize * TileDimmensions * (int)(2 * DefaultZoom);

        GameOfLifeMap map = new(MapSize, MapSize, TileDimmensions, TileDimmensions, TileRenderKey);

        public IslandGame()
        {
            
        }

        protected override StartupParameters Startup()
            => new()
            {
                ScreenWidth = WindowSize,
                ScreenHeight = WindowSize
            };

        protected override void Load()
        {
            map.Randomize();
            MainMap = map;

            var tilesheet = Content.Load<Texture2D>("Images/tileset");
            AddRenderer(TileRenderKey, new SpriteBatchIndexedTextureTileRenderer(SpriteBatch, TileDimmensions, TileDimmensions, tilesheet));

            ZoomIn(1);
        }

        private void Randomize()
        {
            for (var i = 0; i < MapSize; i++)
                for (var j = 0; j < MapSize; j++)
                    map[i, j] = Random.NextDouble() < .35 ? 1 : 0;
        }

        private void Clear()
        {
            for (var i = 0; i < MapSize; i++)
                for (var j = 0; j < MapSize; j++)
                    map[i, j] = 0;
        }

        protected override void PreMapUpdate(GameTime gameTime)
        {
            if (WasKeyJustPressed(Keys.R))
                map.Randomize();

            if (WasKeyJustPressed(Keys.C))
                map.Clear();

            if (WasKeyJustPressed(Keys.Space))
                map.DoUpdate = !map.DoUpdate;

            if (IsLeftMouseDown())
            {
                var cellX = (CurrentMouseState.Y / 2) / TileDimmensions;
                var cellY = (CurrentMouseState.X / 2) / TileDimmensions;

                Stamp(cellX, cellY, 1, 2);
            }

            if (IsRightMouseDown())
            {
                var cellX = (CurrentMouseState.Y / 2) / TileDimmensions;
                var cellY = (CurrentMouseState.X / 2) / TileDimmensions;

                Stamp(cellX, cellY, 0, 2);
            }
        }

        private void Stamp(int cellX, int cellY, int v, int size = 1)
        {
            for (var i = -size; i <= size; i++)
                for (var j = -size; j <= size; j++)
                    map[cellX + i, cellY + j] = v;
        }
    }
}
