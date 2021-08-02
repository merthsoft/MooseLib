using Microsoft.Xna.Framework;

namespace Merthsoft.MooseEngine.Defs
{
    public record GameObjectDef : Def
    {
        public int DefaultLayer { get; set; }
        public Vector2 DefaultSize { get; set; } = Vector2.One;
        public Vector2 DefaultPosition { get; set; }
        public int WorldSizeRound { get; set; } = 2;

        public GameObjectDef(string defName) : base(defName) { }
    }
}
