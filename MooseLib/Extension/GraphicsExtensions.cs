using MonoGame.Extended.Sprites;

namespace Merthsoft.Moose.MooseEngine.Extension;
public static class GraphicsExtensions
{
    public const uint AlphaMask = 0xFF000000;
    public const uint ColorMask = 0x00FFFFFF;
    public const uint RedMask = 0x00FF0000;
    public const uint GreenMask = 0x0000FF00;
    public const uint BlueMask = 0x000000FF;
    public const uint WhiteMask = 0x00FFFFFF;
    public const uint BlackMask = 0x00000000;

    public const uint White = AlphaMask | WhiteMask;
    public const uint Black = AlphaMask | BlackMask;

    public static bool IsWhiteTransparent(this uint argb)
        => (argb >> 24) < 0xFF && (argb & ColorMask) == WhiteMask;

    public static uint Blend(this uint color1, uint color2)
    {
        var a1 = (color1 & AlphaMask) >> 24;
        var a2 = (color2 & AlphaMask) >> 24;
        var outA = a1 + a2 * (1 - a1);
        var r1 = (color1 & RedMask) >> 24;
        var r2 = (color2 & RedMask) >> 24;
        var g1 = (color1 & GreenMask) >> 24;
        var g2 = (color2 & GreenMask) >> 24;
        var b1 = (color1 & BlueMask) >> 24;
        var b2 = (color2 & BlueMask) >> 24;

        uint blend(uint channel1, uint channel2)
            => BlendChannel(a1, a2, outA, channel1, channel2);

        return (outA << 24) | (blend(r1, r2) << 16) | (blend(g1, g2) << 8) | blend(b1, b2);
    }

    private static uint BlendChannel(uint a1, uint a2, uint outA, uint channel1, uint channel2)
        => (uint)((channel1 * a1 + channel2 * a2 * (1 - a1)) / (float)outA).Round(0);
    
    public static Color Shade(this Color c, float delta)
        => new((c.R * delta).Round(), (c.G * delta).Round(), (c.B * delta).Round());

    public static Rectangle GetSourceRectangle(this Texture2D texture, int spriteIndex, int tileWidth, int tileHeight, int tilePadding = 0, int textureMargin = 0)
    {
        var columns = (texture.Width - textureMargin)/ (tileWidth + tilePadding);

        var sourceX = (spriteIndex % columns) * (tileWidth + tilePadding) + textureMargin;
        var sourceY = (spriteIndex / columns) * (tileHeight + tilePadding) + textureMargin;

        return new(sourceX, sourceY, tileWidth, tileHeight);
    }

    public static SpriteBatch DrawString(this SpriteBatch sb, SpriteFont font, string text, float x, float y, Color color)
    {
        sb.DrawString(font, text, new Vector2(x, y), color);
        return sb;
    }

    public static SpriteBatch DrawStringShadow(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color foreColor, Color? shadowColor = null, Vector2? shadowOffset = null)
    {
        sb.DrawString(font, text, position, shadowColor ?? Color.Black);
        sb.DrawString(font, text, position + (shadowOffset ?? Vector2.One), foreColor);
        return sb;
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

    public static void Draw(this Texture2D destinationTexture, Texture2D sourceTexture, Rectangle destinationRect)
    {
        var count = sourceTexture.Width * sourceTexture.Height;
        Color[] data = new Color[count];
        sourceTexture.GetData(0, sourceTexture.Bounds, data, 0, count);
        destinationTexture.SetData(0, destinationRect, data, 0, count);
    }

    public static IEnumerable<Color> ColorGradient(Color start, Color end, int steps)
    {
        int stepA = ((end.A - start.A) / (steps - 1));
        int stepR = ((end.R - start.R) / (steps - 1));
        int stepG = ((end.G - start.G) / (steps - 1));
        int stepB = ((end.B - start.B) / (steps - 1));

        for (int i = 0; i < steps; i++)
        {
            yield return Color.FromNonPremultiplied(start.R + (stepR * i),
                                                    start.G + (stepG * i),
                                                    start.B + (stepB * i),
                                                    start.A + (stepA * i));
        }
    }

    public static Color ColorGradientPercentage(Color start, Color end, double percent)
    {
        var startHsl = end.ToHsl();
        var endHsl = start.ToHsl();
        var alt = 1.0f - percent;

        var h = percent * startHsl.H + alt * endHsl.H;
        var s = percent * startHsl.S + alt * endHsl.S;
        var l = percent * startHsl.L + alt * endHsl.L;

        return FromHsl(h, s, l);
    }

    public static Color FromHsl(double h, double s, double l)
    {
        byte r = 0;
        byte g = 0;
        byte b = 0;

        if (s == 0)
        {
            r = g = b = (byte)(l * 255);
        }
        else
        {
            double v1, v2;
            double hue = h / 360.0;

            v2 = (l < 0.5) ? (l * (1 + s)) : ((l + s) - (l * s));
            v1 = 2 * l - v2;

            r = (byte)(255 * HueToRGB(v1, v2, hue + (1.0 / 3)));
            g = (byte)(255 * HueToRGB(v1, v2, hue));
            b = (byte)(255 * HueToRGB(v1, v2, hue - (1.0 / 3)));
        }

        return new(r, g, b);
    }

    private static double HueToRGB(double v1, double v2, double vH)
    {
        if (vH < 0)
            vH += 1;

        if (vH > 1)
            vH -= 1;

        if ((6 * vH) < 1)
            return (v1 + (v2 - v1) * 6 * vH);

        if ((2 * vH) < 1)
            return v2;

        if ((3 * vH) < 2)
            return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

        return v1;
    }
}
