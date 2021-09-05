using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.GameObjects
{
    public record DrawParameters(
        Texture2D Texture, 
        Rectangle DestinationRectangle,
        Rectangle? SourceRectangle = null, 
        Color? Color = null, 
        float Rotation = 0f, 
        Vector2? Origin = null, 
        SpriteEffects Effects = SpriteEffects.None, 
        float LayerDepth = 1f
    );
}