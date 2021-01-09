namespace MooseLib.Defs
{
    public record Def
    {
        public static readonly Def Empty = new("EMPTY_DEF");

        public string DefName { get; }

        public virtual void LoadContent(MooseContentManager contentManager) { }

        protected Def(string defName) 
            => DefName = defName;
    }
}
