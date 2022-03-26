using MonoGame.Extended.Sprites;

namespace Merthsoft.Moose.MooseEngine.Extension;
public static class GraphicsExtensions
{
    public static Rectangle GetSourceRectangle(this Texture2D texture, int spriteIndex, int tileWidth, int tileHeight, int tilePadding = 0, int textureMargin = 0)
    {
        var columns = (texture.Width - textureMargin)/ (tileWidth + tilePadding);

        var sourceX = (spriteIndex % columns) * (tileWidth + tilePadding) + textureMargin;
        var sourceY = (spriteIndex / columns) * (tileHeight + tilePadding) + textureMargin;

        return new(sourceX, sourceY, tileWidth, tileHeight);
    }

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
        => new((c.R * delta).Round(), (c.G * delta).Round(), (c.B * delta).Round());


    public static Color HalveAlphaChannel(this Color c)
        => c with { A = (byte)(c.A / 2) };

    public static void Draw(this Sprite sprite, SpriteBatch spriteBatch, Vector2 position, Transform2 transform, SpriteEffects spriteEffects)
    {
        if (sprite.IsVisible)
        {
            var texture = sprite.TextureRegion.Texture;
            var sourceRectangle = sprite.TextureRegion.Bounds;
            spriteBatch.Draw(texture, position + transform.WorldPosition, sourceRectangle, sprite.Color * sprite.Alpha, transform.WorldRotation, sprite.Origin, transform.WorldScale, spriteEffects, sprite.Depth);
        }
    }
}
