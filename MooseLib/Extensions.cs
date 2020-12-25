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

            for (int layerIndex = 0; layerIndex < sourceMap.Layers.Count; layerIndex++)
            {
                var layer = sourceMap.Layers[layerIndex];

                switch (layer)
                {
                    case TiledMapObjectLayer objectLayer:
                        // TODO: Convert tiled objects to game objects
                        destMap.AddLayer(new TiledMapObjectLayer(objectLayer.Name, objectLayer.Objects.Clone() as TiledMapObject[]));
                        break;
                    case TiledMapTileLayer sourceLayer:
                        if (layerIndex >= destMap.TileLayers.Count)
                            destMap.AddLayer(new TiledMapTileLayer(sourceLayer.Name, destMap.Width, destMap.Height, destMap.TileWidth, destMap.TileHeight));
                        var destLayer = (destMap.Layers[layerIndex] as TiledMapTileLayer)!;
                        for (ushort x = 0; x < sourceMap.Width; x++)
                            for (ushort y = 0; y < sourceMap.Height; y++)
                                destLayer.SetTile((ushort)(x + destX), (ushort)(y + destY), (uint)sourceLayer.GetTile(x, y).GlobalIdentifier);
                        break;
                }
            }
        }

        public static Color HalveAlphaChannel(this Color c)
            => new(c, c.A / 2);

        public static bool? GetBoolProperty(this TiledMapProperties? poperties, string name)
        {
            if (poperties == null)
                return false;
            
            var property = poperties.FirstOrDefault(p => p.Key == name).Value;
            if (property == null)
                return null;
            
            return bool.TryParse(property, out var result) && result;
        }

        public static bool IsBlocking(this TiledMapTile tile, TiledMap map)
        {
            var tileSet = map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
            if (tileSet == null)
                return false;

            var firstTile = map.GetTilesetFirstGlobalIdentifier(tileSet);
            var tileSetTile = tileSet.Tiles.FirstOrDefault(t => t.LocalTileIdentifier == tile.GlobalIdentifier - firstTile);

            var ret = tileSetTile?.Properties.GetBoolProperty("blocking");
            ret ??= tileSet.Properties.GetBoolProperty("blocking");
            ret ??= false;
            return ret.Value;
        }

        public static bool IsBlockedAt(this TiledMapTileLayer layer, int x, int y, TiledMap map)
            => layer.GetTile((ushort)x, (ushort)y).IsBlocking(map);


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

        public static void ForEach<T>(this IEnumerable<T> set, Action<T> action)
        {
            foreach (var t in set)
                action(t);
        }

        public static Vector2 GetFloor(this Vector2 vector)
            => new((int)vector.X, (int)vector.Y);

        public static void InvokeAtIndex(this Action<int>?[] actions, int index)
            => actions.ElementAtOrDefault(index)?.Invoke(index);

        public static (float, float) Floor(this (float X, float Y) vec)
            => new(MathF.Floor(vec.X), MathF.Floor(vec.Y));

        public static float Floor(this float f)
            => MathF.Floor(f);

        public static float Round(this float f, int digits)
            => MathF.Round(f, digits);

        public static long Sum(this IEnumerable<byte> set)
            => set.Sum(b => (long)b);

        public static Vector2 RotateAround(this Vector2 point, Vector2 center, float angleInDegrees)
        {
            var rad = angleInDegrees * (MathF.PI / 180);
            var s = MathF.Sin(rad);
            var c = MathF.Cos(rad);

            // translate point back to origin:
            var oldX = point.X - center.X;
            var oldY = point.Y - center.Y;

            // rotate point
            var newX = oldX * c - oldY * s;
            var newY = oldX * s + oldY * c;

            // translate point back:
            return new(newX + center.X, newY + center.Y);
        }
    }
}
