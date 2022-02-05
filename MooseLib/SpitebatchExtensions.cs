using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merthsoft.Moose.MooseEngine
{
    public static class SpiteBatchExtensions
    {
        public static SpriteBatch DrawRect(this SpriteBatch s, Rectangle r, Color c)
        {
            s.DrawRectangle(r, c);
            return s;
        }

        public static SpriteBatch FillRect(this SpriteBatch s, Rectangle r, Color fillColor, Color? borderColor = null)
        {
            s.FillRectangle(r, fillColor);
            if (borderColor != null)
                s.DrawRectangle(r, borderColor.Value);
            return s;
        }

        public static SpriteBatch FillRect(this SpriteBatch s, Vector2 position, int width, int height, Color fillColor, Color? borderColor = null)
        {
            var r = new Rectangle((int)position.X, (int)position.Y, width, height);
            s.FillRectangle(r, fillColor);
            if (borderColor != null)
                s.DrawRectangle(r, borderColor.Value);
            return s;
        }
    }
}
