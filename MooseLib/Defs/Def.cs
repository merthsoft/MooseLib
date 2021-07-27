namespace MooseLib.Defs
{
    public record Def(string DefName)
    {
        public static readonly Def Empty = new("EMPTY_DEF");

        public virtual void LoadContent(MooseContentManager contentManager) { }
    }
}
