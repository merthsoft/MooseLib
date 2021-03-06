﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MooseLib.Ui.Controls
{
    public class TextList : Control
    {
        public List<TextListOption> Options = new();

        public SelectMode SelectMode { get; set; } = SelectMode.None;

        public int MouseOverIndex { get; protected set; } = -1;

        public TextList(Window window, int x, int y) : base(window, x, y)
        {
        }

        public TextList(Window window, int x, int y, params TextListOption[] options) : this(window, x, y)
            => Options.AddRange(options);

        public TextList(Window window, int x, int y, IEnumerable<string> options) : this(window, x, y)
            => Options.AddRange(options.Select(o => new TextListOption { Text = o }));

        public override Vector2 CalculateSize()
            => Options.Aggregate(Vector2.Zero, (acc, o) =>
        {
            var textSize = Window.Theme.Fonts[FontIndex].MeasureString(o.Text);
            return new Vector2(Math.Max(acc.X, textSize.X), acc.Y + Window.Theme.TileHeight);
        });


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Options.Count == 0 || Window.Theme.Fonts == null)
                return;

            for (var index = 0; index < Options.Count; index++)
                spriteBatch.DrawString(
                    Window.Theme.Fonts[FontIndex], 
                    Options[index].Text, 
                    GlobalPosition + new Vector2(0, index * Window.Theme.TileHeight), 
                    index == MouseOverIndex ? Theme.TextMouseOverColor : Theme.TextColor);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver)
            {
                MouseOverIndex = (int)updateParameters.LocalMousePosition.Y / Window.Theme.TileHeight;
                if (updateParameters.MouseOver && updateParameters.LeftMouse)
                {
                    Options[MouseOverIndex].Selected = SelectMode switch
                    {
                        SelectMode.Multiple => !Options[MouseOverIndex].Selected,
                        SelectMode.Single => true,
                        SelectMode.None => false,
                        _ => false,
                    };
                    Action?.Invoke(this, updateParameters);
                }
            }
            else
                MouseOverIndex = -1;
            base.Update(updateParameters);
        }
    }
}
