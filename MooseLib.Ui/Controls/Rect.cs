﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Rect : Control
{
    public Vector2 Size { get; set; }

    public int Thickness { get; set; } = 1;

    public Color? BorderColor { get; set; }
    public Color? FillColor { get; set; }

    public bool DrawBorder { get; set; } = true;
    public bool DrawFill { get; set; } = true;

    public override Vector2 CalculateSize() => Size;

    public Rect(Theme theme, float x, float y, float w, float h, int thickness = 1) : base(theme, x, y)
    {
        Size = new(w, h);
        Thickness = thickness;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
    {
        if (DrawFill)
            spriteBatch.FillRectangle(Position + parentOffset, Size, FillColor ?? Theme.ControlBackgroundColor);
        if (DrawBorder)
            spriteBatch.DrawRectangle(Position + parentOffset, Size, BorderColor ?? Theme.ControlBorderColor, Thickness);
    }
}