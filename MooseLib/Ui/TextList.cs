using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib.Ui
{
    public class TextList : Control
    {
        public List<TextListOption> Options = new();
        public Color Color { get; set; } = Color.Black;
        public Color MouseOverColor { get; set; } = Color.White;
        public Color SelectedColor { get; set; } = Color.Blue;
        public SelectMode SelectMode { get; set; } = SelectMode.None;

        public int MouseOverIndex { get; protected set; } = -1;

        public TextList(Window window) : base(window)
        {
        }

        public TextList(Window window, params TextListOption[] options) : this(window)
            => Options.AddRange(options);

        public TextList(Window window, IEnumerable <string> options) : this(window)
            => Options.AddRange(options.Select(o => new TextListOption { Text = o }));

        public override Vector2 CalculateSize()
            => Options.Aggregate(Vector2.Zero, (acc, o) =>
            {
                var textSize = Window.WindowManager.Font.MeasureString(o.Text);
                return new Vector2(Math.Max(acc.X, textSize.X), acc.Y + Window.WindowManager.TileHeight);
            });
                

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Options.Count == 0 || Window.WindowManager.Font == null)
                return;

            for (var index = 0; index < Options.Count; index++)
                spriteBatch.DrawString(
                    Window.WindowManager.Font, 
                    Options[index].Text, 
                    GlobalPosition + new Vector2(0, index * Window.WindowManager.TileHeight), 
                    index == MouseOverIndex ? MouseOverColor : Color
                );
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver)
            {
                MouseOverIndex = (int)updateParameters.LocalMousePosition.Y / Window.WindowManager.TileHeight;
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
