using Microsoft.Xna.Framework;

namespace Merthsoft.MooseEngine
{
    public record RayCell(Vector2 WorldPosition, IList<int> BlockedVector);
}
