using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MooseLib
{
    public record RayCell(Vector2 WorldPosition, IList<int> BlockedVector);
}
