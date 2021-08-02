using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Merthsoft.MooseEngine
{
    public record RayCell(Vector2 WorldPosition, IList<int> BlockedVector);
}
