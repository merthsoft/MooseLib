using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon.Spells;

public abstract class Spell : AnimatedGameObject
{
    public new SpellDef Def => (SpellDef)base.Def;

    public static string Cast = "cast";
    public static string Active = "active";
    public static string Hit = "hit";
    public static string Dead = "dead";

    public GameObjectBase Owner { get; }

    public Spell(SpellDef def, GameObjectBase owner, Vector2 position) : base(def, position, "spells", state: Cast)
    {
        Owner = owner;
    }
}
