using EpPathFinding.cs;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib
{
    public static class Extensions
    {
        public static void CopyMap(this TiledMap destMap, TiledMap sourceMap, int destX, int destY)
        {

            foreach (var tileset in sourceMap.Tilesets)
                if (!destMap.Tilesets.Contains(tileset))
                    destMap.AddTileset(tileset, sourceMap.GetTilesetFirstGlobalIdentifier(tileset));

            for (int layerIndex = 0; layerIndex < sourceMap.TileLayers.Count; layerIndex++)
            {
                var sourceLayer = sourceMap.TileLayers[layerIndex];
                var destLayer = destMap.TileLayers[layerIndex];
                for (ushort x = 0; x < sourceMap.Width; x++)
                    for (ushort y = 0; y < sourceMap.Height; y++)
                        destLayer.SetTile((ushort)(x + destX), (ushort)(y + destY), (uint)sourceLayer.GetTile(x, y).GlobalIdentifier);
            }
        }

        public static Color HalveAlphaChannel(this Color c)
            => new(c, c.A / 2);

        public static bool GetBoolProperty(this TiledMapProperties poperties, string name)
        {
            var property = poperties.FirstOrDefault(p => p.Key == name).Value;
            if (property == null)
                return false;
            return bool.TryParse(property, out var result) && result;
        }

        public static bool IsBlocking(this TiledMapTile tile, TiledMap map)
        {
            var tileSet = map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
            if (tileSet == null)
                return false;

            var firstTile = map.GetTilesetFirstGlobalIdentifier(tileSet);
            var tileSetTile = tileSet.Tiles?.FirstOrDefault(t => t.LocalTileIdentifier == tile.GlobalIdentifier - firstTile);

            return tileSetTile?.Properties?.GetBoolProperty("blocking") ?? false;
        }

        public static bool IsBlockedAt(this TiledMapTileLayer layer, ushort x, ushort y, TiledMap map)
            => layer.Properties.GetBoolProperty("blocking")
            || layer.GetTile(x, y).IsBlocking(map);


        public static bool IsBlockedAt(this TiledMap map, int x, int y)
            => map.IsBlockedAt((ushort)x, (ushort)y);

        public static bool IsBlockedAt(this TiledMap map, ushort x, ushort y)
        {
            foreach (var layer in map.TileLayers)
            {
                if (layer.IsBlockedAt(x, y, map))
                    return true;
            }
            return false;
        }

        public static void Draw(this Sprite sprite, SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 scale, SpriteEffects spriteEffects)
        {
            _ = sprite ?? throw new ArgumentNullException(nameof(sprite));
            _ = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));

            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;
                spriteBatch.Draw(texture, position, sourceRectangle, sprite.Color * sprite.Alpha, rotation, sprite.Origin, scale, spriteEffects, sprite.Depth);
            }
        }

        public static bool InBounds(this Vector2 pos, int width, int height)
            => pos.X >= 0 && pos.Y >= 0 && pos.X < width && pos.Y < height;

        public static IEnumerable<Vector2> FindPath(this JumpPointParam pathFinder, Vector2 cell1, int x2, int y2)
            => pathFinder.FindPath(cell1, new Vector2(x2, y2));

        public static IEnumerable<Vector2> FindPath(this JumpPointParam pathFinder, Vector2 cell1, Vector2 cell2)
        {
            if (!cell1.InBounds(pathFinder.SearchGrid.width, pathFinder.SearchGrid.height))
                return Enumerable.Empty<Vector2>();
            if (!cell2.InBounds(pathFinder.SearchGrid.width, pathFinder.SearchGrid.height))
                return Enumerable.Empty<Vector2>();

            pathFinder.Reset(new((int)cell1.X, (int)cell1.Y), new((int)cell2.X, (int)cell2.Y));
            return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(pathFinder))
                    .Distinct().Select(gridPos => new Vector2(gridPos.x, gridPos.y));
        }

        public static void ForEach<T>(this IEnumerable<T> set, Action<T> action)
        {
            foreach (var t in set)
                action(t);
        }

        public static Vector2 GetFloor(this Vector2 vector)
            => new((int)vector.X, (int)vector.Y);
    }
}
