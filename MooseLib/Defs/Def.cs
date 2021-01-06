namespace MooseLib.Defs
{
    public abstract record Def
    {
        public string DefName { get; init; } = "";

        public virtual void LoadContent(MooseGame parentGame) { }
    }
}
