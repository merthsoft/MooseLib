using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class ToggleButton : Button
{
    public bool Toggled { get; set; } = false;

    public ToggleButton(string text, IControlContainer container, float x, float y) : base(text, container, x, y) { }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        => DrawBasicButton(spriteBatch, parentOffset,
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
