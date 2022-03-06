﻿using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.TiledDriver;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using Roy_T.AStar.Grids;

namespace Merthsoft.Moose.MooseEngine;

public static class Extensions
{
    public static void MoveToTop<T>(this List<T> list, int index)
    {
        T item = list[index];
        for (int i = index; i > 0; i--)
            list[i] = list[i - 1];
        list[0] = item;
    }

    public static void MoveToBottom<T>(this List<T> list, int index)
    {
        T item = list[index];
        for (int i = index; i < list.Count - 1; i++)
            list[i] = list[i + 1];
        list[^1] = item;
    }

    public static void Swap<T>(this IList<T> list, int index1, int index2)
    {
        var track1 = list[index1];
        var track2 = list[index2];
        list[index1] = track2;
        list[index2] = track1;
    }

    public static T AddItem<T>(this IList<T> list, T item)
    {
        list.Add(item);
        return item;
    }

    public static bool IsPrintableAscii(this char c)
        => char.IsLetterOrDigit(c)
        || char.IsPunctuation(c)
        || char.IsSymbol(c)
        || c == ' ';

    public static SpriteBatch DrawRect(this SpriteBatch s, RectangleF r, Color c)
    {
        s.DrawRectangle(r, c);
        return s;
    }

    public static SpriteBatch FillRect(this SpriteBatch s, RectangleF r, Color fillColor, Color? borderColor = null)
    {
        s.FillRectangle(r, fillColor);
        if (borderColor != null)
            s.DrawRectangle(r, borderColor.Value);
        return s;
    }

    public static SpriteBatch FillRect(this SpriteBatch s, Vector2 position, int width, int height, Color fillColor, Color? borderColor = null)
        => s.FillRect(new Rectangle((int)position.X, (int)position.Y, width, height), fillColor, borderColor);

    public static void DrawEllipse(this SpriteBatch spriteBatch, RectangleF rect, int sides, Color color, float thickness = 1f, float layerDepth = 0)
        => spriteBatch.DrawEllipse(rect.Center, rect.Size, sides, color, thickness, layerDepth);

    public static Color Shade(this Color c, float delta)
        => new(Round(c.R * delta), Round(c.G * delta), Round(c.B * delta));

    public static int IndexOf<T>(this IEnumerable<T> set, Func<T, bool> func)
    {
        var index = 0;
        foreach (var item in set)
        {
            if (func(item))
                return index;
        }
        return -1;
    }

    public static RectangleF Move(this RectangleF rect, Vector2 delta)
        => new(rect.X + delta.X, rect.Y + delta.Y, rect.Width, rect.Height);

    public static bool Intersects(this RectangleF rect, float x, float y)
        => x >= rect.X
            && x <= rect.X + rect.Width
        && y >= rect.Y
            && y <= rect.Y + rect.Height;

    public static bool Intersects(this Rectangle rect, int x, int y)
        => x >= rect.X
            && x <= rect.X + rect.Width
        && y >= rect.Y
            && y <= rect.Y + rect.Height;

    public static bool Intersects(this RectangleF rect, Vector2 point)
        => point.X >= rect.X
            && point.X <= rect.X + rect.Width
        && point.Y >= rect.Y
            && point.Y <= rect.Y + rect.Height;

    public static bool Intersects(this Rectangle rect, Point point)
        => point.X >= rect.X
            && point.X <= rect.X + rect.Width
        && point.Y >= rect.Y
            && point.Y <= rect.Y + rect.Height;

    public static IEnumerable<T> Select<T>(this Range range, Func<int, T> func)
    {
        for (int index = range.Start.Value; index < range.End.Value; index++)
            yield return func(index);
    }

    public static Vector2 Round(this Vector2 vector, int digits)
        => new(vector.X.Round(digits), vector.Y.Round(digits));

    public static Vector2 ToCell(this Vector2 worldPosition, MooseGame game)
        => new Vector2(worldPosition.X / game.TileWidth, worldPosition.Y / game.TileHeight).GetFloor();

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

    public static bool IsBlocking(this TiledMapTile Tile, IMap map)
    {
        var tiledMap = (map as TiledMooseMap)?.Map;
        if (tiledMap == null)
            return true;

        var tileSet = tiledMap.GetTilesetByTileGlobalIdentifier(Tile.GlobalIdentifier);
        if (tileSet == null)
            return false;

        var firstTile = tiledMap.GetTilesetFirstGlobalIdentifier(tileSet);
        var tileSetTile = tileSet.Tiles.FirstOrDefault(t => t.LocalTileIdentifier == Tile.GlobalIdentifier - firstTile);

        return tileSetTile?.Properties.GetBoolProperty("blocking")
            ?? tileSet.Properties.GetBoolProperty("blocking")
            ?? false;
    }

    public static void Draw(this Sprite sprite!!, SpriteBatch spriteBatch!!, Vector2 position, Transform2 transform, SpriteEffects spriteEffects)
    {
        if (sprite.IsVisible)
        {
            var texture = sprite.TextureRegion.Texture;
            var sourceRectangle = sprite.TextureRegion.Bounds;
            spriteBatch.Draw(texture, position + transform.WorldPosition, sourceRectangle, sprite.Color * sprite.Alpha, transform.WorldRotation, sprite.Origin, transform.WorldScale, spriteEffects, sprite.Depth);
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

    public static int Floor(this float f)
        => (int)MathF.Floor(f);

    public static int Ceiling(this float f)
        => (int)MathF.Ceiling(f);

    public static int Ceiling(this decimal f)
        => (int)Math.Ceiling(f);

    public static float Round(this float f, int digits)
        => MathF.Round(f, digits);

    public static int Round(this float f)
        => (int)MathF.Round(f, 0);

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

    public static string UpperFirst(this string s)
        => $"{char.ToUpper(s[0])}{new string(s.Skip(1).ToArray())}";

    public static Grid DisconnectWhere(this Grid grid, Func<int, int, bool> func)
    {
        for (var x = 0; x < grid.Columns; x++)
            for (var y = 0; y < grid.Rows; y++)
                if (func(x, y))
                    grid.DisconnectNode(new(x, y));

        return grid;
    }

    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey? key)
    {
        if (key == null)
            return default;
        if (dictionary.ContainsKey(key))
            return dictionary[key];
        return default;
    }
}
