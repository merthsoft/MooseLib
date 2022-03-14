using System.Diagnostics.CodeAnalysis;

namespace Merthsoft.Moose.MooseEngine.Defs;

public record Def(string DefName) : IEquatable<Def>, IEqualityComparer<Def>
{
    public static readonly Def Empty = new("EMPTY_DEF");

    public string DefName { get; set; } = DefName;

    public virtual bool Equals(Def? other) => DefName == other?.DefName;
    public override int GetHashCode() => DefName.GetHashCode();
    public bool Equals(Def? x, Def? y) => x?.DefName == y?.DefName;
    public int GetHashCode([DisallowNull] Def obj) => obj.DefName.GetHashCode();

    public virtual void LoadContent(MooseContentManager contentManager) { }
}
