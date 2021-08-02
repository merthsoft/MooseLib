using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Merthsoft.MooseEngine.Ui
{
    public static class Extensions
    {
        public static T AddPassThrough<T>(this ICollection<T> list, T item)
        {
            list.Add(item);
            return item;
        }

        public static bool JustPressed(this ButtonState buttonState, ButtonState previousButtonState)
               => buttonState == ButtonState.Pressed && previousButtonState == ButtonState.Released;

        public static bool JustReleased(this ButtonState buttonState, ButtonState previousButtonState)
            => buttonState == ButtonState.Released && previousButtonState == ButtonState.Pressed;
    }
}
