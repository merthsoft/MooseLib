using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Spells;

namespace Merthsoft.Moose.Dungeon;
public class SpellBook
{
    public List<SpellDef> AllSpells { get; } = [];
    public Dictionary<string, SpellDef> SpellDictionary { get; } = [];

    public Dictionary<string, Func<SpellDef, DungeonObject, Vector2, Spell>> Factory { get; } = [];
    
    public void AddSpell(SpellDef spellDef, Func<SpellDef, DungeonObject, Vector2, Spell> generator)
    {
        Factory[spellDef.DefName] = generator;
        AllSpells.Add(spellDef);
        SpellDictionary[spellDef.DefName] = spellDef;
    }

    public Spell Cast(SpellDef spell, DungeonObject owner, Vector2 position) 
        => Factory[spell.DefName](spell, owner, position);
}
