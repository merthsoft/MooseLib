using Merthsoft.Moose.MageQuest.Spells;

namespace Merthsoft.Moose.MageQuest;
public class SpellBook
{
    public List<SpellDef> AllSpells { get; } = new();

    public Dictionary<string, Func<SpellDef, Vector2, Vector2, Spell>> Factory { get; } = new();

    public void AddSpell(SpellDef spellDef, Func<SpellDef, Vector2, Vector2, Spell> generator)
    {
        Factory[spellDef.DefName] = generator;
        AllSpells.Add(spellDef);
    }

    public Spell Cast(SpellDef spell, Vector2 start, Vector2 position)
        => Factory[spell.DefName](spell, start, position);
}
