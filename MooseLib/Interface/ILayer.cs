using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.MooseEngine.Interface
{
    public interface ILayer
    {
        string Name { get; }
        bool IsHidden { get; set;}
        Vector2 DrawOffset { get; }
    }
}
