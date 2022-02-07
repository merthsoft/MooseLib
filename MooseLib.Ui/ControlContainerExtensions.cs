﻿using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public static class ControlContainerExtensions
    {
        public static Line AddLine(this IControlContainer container, int x1, int y1, int x2, int y2, int thickness = 1)
             => container.AddControlPassThrough(new Line(container.ParentWindow, x1, y1, x2, y2, thickness));

        public static Line AddLine(this IControlContainer container, float x1, float y1, float x2, float y2, int thickness = 1)
             => container.AddControlPassThrough(new Line(container.ParentWindow, (int)x1, (int)y1, (int)x2, (int)y2, thickness));

        public static Rect AddRectangle(this IControlContainer container, int x, int y, int w, int h)
             => container.AddControlPassThrough(new Rect(container.ParentWindow, x, y, w, h));

        public static Label AddLabel(this IControlContainer container, int x, int y, string text, int fontIndex = 0, Color? color = null, int strokeSize = 0, Color? strokeColor = null, bool hightlightOnHover = false)
            => container.AddControlPassThrough(new Label(container.ParentWindow, x, y)
            {
                Text = text,
                FontIndex = fontIndex,
                Color = color,
                StrokeSize = strokeSize,
                StrokeColor = strokeColor ?? container.ParentWindow.Theme.TextBorderColor,
                HighlightOnHover = hightlightOnHover,
            });

        public static Label AddActionLabel(this IControlContainer container, int x, int y, string text, Action<Control, UpdateParameters>? action, int fontIndex = 0)
            => container.AddControlPassThrough(new Label(container.ParentWindow, x, y)
            {
                Text = text,
                Action = action,
                FontIndex = fontIndex,
            });

        public static TextList AddActionList(this IControlContainer container, int x, int y, Action<Control, UpdateParameters> action, IEnumerable<string> options, int fontIndex = 0)
            => container.AddControlPassThrough(new TextList(container.ParentWindow, x, y, options)
            {
                Action = action,
                SelectMode = SelectMode.None,
                FontIndex = fontIndex,
            });

        public static TextGrid AddActionGrid(this IControlContainer container, int x, int y, int gridWidth, Action<Control, UpdateParameters> action, IEnumerable<string> options, int fontIndex = 0)
            => container.AddControlPassThrough(new TextGrid(container.ParentWindow, x, y, gridWidth, options)
            {
                Action = action,
                SelectMode = SelectMode.None,
                FontIndex = fontIndex,
            });

        public static Picture AddPicture(this IControlContainer container, int x, int y, Texture2D texture, Rectangle? sourceRectangle = null)
            => container.AddControlPassThrough(new Picture(container.ParentWindow, x, y, texture) { SourceRectangle = sourceRectangle ?? new(x, y, texture.Width, texture.Height) });

        public static Picture AddPicture(this IControlContainer container, int x, int y, Texture2D texture, Vector2 scale)
            => container.AddControlPassThrough(new Picture(container.ParentWindow, x, y, texture) { Scale = scale });

        public static Picture AddPicture(this IControlContainer container, int x, int y, Texture2D texture, float scale)
            => container.AddControlPassThrough(new Picture(container.ParentWindow, x, y, texture) { Scale = new(scale, scale) });

        public static Button AddButton(this IControlContainer container, int x, int y, string text, Action<Control, UpdateParameters> action, int fontIndex = 0)
            => container.AddControlPassThrough(new Button(container.ParentWindow, x, y, text)
            {
                Action = action,
                FontIndex = fontIndex,
            });

        public static ToggleButton AddToggleButton(this IControlContainer container, int x, int y, string text, bool toggled, Action<Control, UpdateParameters> action, int fontIndex = 0)
            => container.AddControlPassThrough(new ToggleButton(container.ParentWindow, x, y, text)
            {
                Action = action,
                FontIndex = fontIndex,
            });

        public static Slider AddSlider(this IControlContainer container, int x, int y, long min, long max, long initialValue, Action<Control, UpdateParameters> action, int fontIndex = 0)
            => container.AddControlPassThrough(new Slider(container.ParentWindow, x, y, min, max)
            {
                Value = initialValue,
                Action = action,
                FontIndex = fontIndex,
            });

        public static Checkbox AddCheckbox(this IControlContainer container, int x, int y, bool isChecked, string? text, Action<Control, UpdateParameters> action, int fontIndex = 0)
            => container.AddControlPassThrough(new Checkbox(container.ParentWindow, x, y)
            {
                IsChecked = isChecked,
                Text = text,
                Action = action,
                FontIndex = fontIndex,
            });

        public static Panel AddPanel(this IControlContainer container, int x, int y, int w, int h)
            => container.AddControlPassThrough(new Panel(container.ParentWindow, x, y, w, h));

        public static StackPanel AddStackPanel(this IControlContainer container, int x, int y, int w, int h)
            => container.AddControlPassThrough(new StackPanel(container.ParentWindow, x, y, w, h));
    }
}
