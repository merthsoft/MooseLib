using System.Collections;

namespace Merthsoft.Moose.Dungeon.Entities.Spells;
public class SpellContainer : Spell, IEnumerable<Spell>
{
    public List<Spell> Spells = new List<Spell>();
    private int manaCost;
    public override int ManaCost => manaCost;

    public SpellContainer(DungeonObject owner) : base(new SpellDef("container", 0, "container"), owner, Vector2.Zero)
    {
        State = Active;
    }

    public SpellContainer Add(Spell spell)
    {
        Spells.Add(spell);
        if (ManaCost < spell.Def.ManaCost)
            manaCost = spell.Def.ManaCost;
        return this;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        foreach (var spell in Spells)
            spell.Update(game, gameTime);

        if (Spells.All(s => s.State == Dead))
        {
            Remove = true;
            State = Dead;
        }
    }

    public override void Effect()
    {
        
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var spell in Spells)
            spell.Draw(game, gameTime, spriteBatch);
    }

    public IEnumerator<Spell> GetEnumerator() => ((IEnumerable<Spell>)Spells).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Spells).GetEnumerator();
}
