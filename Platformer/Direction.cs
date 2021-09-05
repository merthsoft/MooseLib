using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.Platformer
{
    class Direction
    {
        public const string Left = "left";
        public const string Right = "right";

        public static Vector2 North = new(0, -1);
        public static Vector2 East = new(1, 0);
        public static Vector2 South = new(0, 1);
        public static Vector2 West = new(-1, 0);
    }
}
