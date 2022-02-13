using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class ToggleButton : Button
{
    public bool Toggled { get; set; } = false;

    public ToggleButton(Window window, int x, int y, string text) : base(window, x, y, text) { }

    public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        => DrawButton(spriteBatch, drawOffset,
            BackgroundColor ?? Theme.ResolveBackgroundColor(UpdateParameters, Enabled),
            BorderColor ?? Theme.ControlBorderColor,
            Theme.ResolveTextColor(UpdateParameters, Enabled, Toggled)
           );

    public override void Update(UpdateParameters updateParameters)
    {
        if (updateParameters.MouseOver && (updateParameters.LeftMouseClick || updateParameters.RightMouseClick))
        {
            Toggled = !Toggled;
            Action?.Invoke(this, updateParameters);
        }
    }
}
