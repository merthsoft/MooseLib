﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui;

public static class ControlContainerExtensions
{
    public static TControl AddControlPassThrough<TControl>(this IControlContainer container, TControl control) where TControl : Control
    {
        container.AddControl(control);
        return control;
    }

    public static TextBox AddTextBox(this IControlContainer container, float x, float y, int width, string text = "", int fontIndex = 0)
        => container.AddControlPassThrough(new TextBox(container, x, y, width)
        {
            Text = text,
            FontIndex = fontIndex
        });

    public static Line AddLine(this IControlContainer container, int x1, int y1, int x2, int y2, int thickness = 1)
         => container.AddControlPassThrough(new Line(container, x1, y1, x2, y2, thickness));

    public static Line AddLine(this IControlContainer container, float x1, float y1, float x2, float y2, int thickness = 1)
         => container.AddControlPassThrough(new Line(container, (int)x1, (int)y1, (int)x2, (int)y2, thickness));

    public static Rect AddRectangle(this IControlContainer container, float x, float y, int w, int h, Color? fillColor = null, Color? borderColor = null)
         => container.AddControlPassThrough(new Rect(container, x, y, w, h)
         {
             BorderColor = borderColor,
             BackgroundColor = fillColor,
         });

    public static Label AddLabel(this IControlContainer container, float x, float y, string text, int fontIndex = 0, Color? color = null, int strokeSize = 0, Color? strokeColor = null, bool hightlightOnHover = false)
        =>  container.AddLabel(text, x, y, fontIndex, color, strokeSize, strokeColor, hightlightOnHover);

    public static Label AddLabel(this IControlContainer container, string text, float x = 0, float y = 0, int fontIndex = 0, Color? color = null, int strokeSize = 0, Color? strokeColor = null, bool hightlightOnHover = false)
        => container.AddControlPassThrough(new Label(container, x, y)
        {
            Text = text,
            FontIndex = fontIndex,
            TextColor = color,
            StrokeSize = strokeSize,
            StrokeColor = strokeColor ?? container.Theme.TextBorderColor,
            HighlightOnHover = hightlightOnHover,
        });

    public static Label AddActionLabel(this IControlContainer container, float x, float y, string text, Action<Control, UpdateParameters>? action, int fontIndex = 0)
        => container.AddControlPassThrough(new Label(container, x, y)
        {
            Text = text,
            Action = action,
            FontIndex = fontIndex,
            HighlightOnHover = true,
            Enabled = action != null,
        });

    public static TextGrid AddActionGrid(this IControlContainer container, float x, float y, int gridWidth, Action<Control, UpdateParameters> action, IEnumerable<string> options, int fontIndex = 0)
        => container.AddControlPassThrough(new TextGrid(container, x, y, gridWidth, options)
        {
            Action = action,
            SelectMode = SelectMode.None,
            FontIndex = fontIndex,
        });

    public static Picture AddPicture(this IControlContainer container, float x, float y, Texture2D texture, Rectangle? sourceRectangle = null, Vector2? scale = null, Color? color = null)
        => container.AddControlPassThrough(new Picture(container, x, y, texture)
        {
            SourceRectangle = sourceRectangle ?? new(0, 0, texture.Width, texture.Height),
            Scale = scale ?? Vector2.One,
            Color = color ?? Color.White,
        });

    public static Picture AddPicture(this IControlContainer container, float x, float y, Texture2D texture, Vector2 scale)
        => container.AddControlPassThrough(new Picture(container, x, y, texture) { Scale = scale });

    public static Picture AddPicture(this IControlContainer container, float x, float y, Texture2D texture, float scale)
        => container.AddControlPassThrough(new Picture(container, x, y, texture) { Scale = new(scale, scale) });

    public static Button AddButton(this IControlContainer container, float x, float y, string text, Action<Control, UpdateParameters>? action = null, int fontIndex = 0)
        => container.AddControlPassThrough(new Button(text, container, x, y)
        {
            Action = action,
            FontIndex = fontIndex,
        });

    public static Button AddButton(this IControlContainer container, string text, Action<Control, UpdateParameters>? action = null, int fontIndex = 0)
        => container.AddControlPassThrough(new Button(text, container, 0, 0)
        {
            Action = action,
            FontIndex = fontIndex,
        });

    public static Button AddTextureButton(this IControlContainer container, float x, float y, Texture2D texture,
        Action<Control, UpdateParameters> action, int fontIndex = 0, Rectangle? sourceRect = null, Vector2? scale = null, string label = "")
        => container.AddControlPassThrough(new Button("", container, x, y)
        {
            Action = action,
            FontIndex = fontIndex,
            Texture = texture,
            SourceRectangle = sourceRect,
            BackgroundDrawingMode = BackgroundDrawingMode.None,
            TextureScale = scale ?? Vector2.One,
            Text = label,
        });

    public static Button AddToggleButton(this IControlContainer container, float x, float y, string text, bool toggled, Action<Control, UpdateParameters> action, int fontIndex = 0)
        => container.AddControlPassThrough(new Button(text, container, x, y)
        {
            Action = action,
            FontIndex = fontIndex,
            Toggleable = true,
            Toggled = toggled,
        });

    public static Slider AddSlider(this IControlContainer container, float x, float y, int min, int max, int initialValue, Action<Control, UpdateParameters> action, int fontIndex = 0)
        => container.AddControlPassThrough(new Slider(container, x, y, min, max)
        {
            Value = initialValue,
            Action = action,
            FontIndex = fontIndex,
        });

    public static Panel AddPanel(this IControlContainer container, float x, float y, float w, float h, BackgroundDrawingMode backgroundDrawingMode = BackgroundDrawingMode.Texture)
        => container.AddControlPassThrough(new Panel(container, x, y, w, h)
        {
            BackgroundDrawingMode = backgroundDrawingMode,
        });

    public static StackPanel AddStackPanel(this IControlContainer container, float x, float y, float w, float h, BackgroundDrawingMode backgroundDrawingMode = BackgroundDrawingMode.Texture, StackDirection stackDirection = StackDirection.Vertical, int padding = 0)
        => container.AddControlPassThrough(new StackPanel(container, x, y, w, h)
        {
            BackgroundDrawingMode = backgroundDrawingMode,
            Direction = stackDirection,
            Padding = padding,
        });

    public static SortedStackPanel<TControl, TKey> AddSortedStackPanel<TControl, TKey>(this IControlContainer container, Func<TControl, TKey> orderBy, float x, float y, float w, float h, BackgroundDrawingMode backgroundDrawingMode = BackgroundDrawingMode.Texture, StackDirection stackDirection = StackDirection.Vertical, int padding = 0)
        where TControl : Control
        => container.AddControlPassThrough(new SortedStackPanel<TControl, TKey>(orderBy, container, x, y, w, h)
        {
            BackgroundDrawingMode = backgroundDrawingMode,
            Direction = stackDirection,
            Padding = padding,
        });

    public static GrowPanel AddGrowPanel(this IControlContainer container, float x, float y, BackgroundDrawingMode backgroundDrawingMode = BackgroundDrawingMode.Texture, StackDirection stackDirection = StackDirection.Vertical, int padding = 0)
        => container.AddControlPassThrough(new GrowPanel(container, x, y)
        {
            BackgroundDrawingMode = backgroundDrawingMode,
            Direction = stackDirection,
            Padding = padding,
        });
}
