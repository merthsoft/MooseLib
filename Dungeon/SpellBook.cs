using Merthsoft.Moose.Dungeon.Spells;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon;
public class SpellBook
{
    public List<SpellDef> AllSpells { get; } = new();

    public Dictionary<string, Func<SpellDef, GameObjectBase, Vector2, Spell>> Factory { get; } = new();
    
    public void AddSpell(SpellDef spellDef, Func<SpellDef, GameObjectBase, Vector2, Spell> generator)
    {
        Factory[spellDef.DefName] = generator;
        AllSpells.Add(spellDef);
    }

    public Spell Cast(SpellDef spell, GameObjectBase owner, Vector2 position) 
        => Factory[spell.DefName](spell, owner, position);
}
