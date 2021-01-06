using Microsoft.Xna.Framework;

namespace MooseLib.Defs
{
    public record GameObjectDef : Def
    {
        public int DefaultLayer { get; set; }
        public Vector2 DefaultSize { get; set; } = Vector2.One;
        public Vector2 DefaultPosition { get; set; }
    }
}
