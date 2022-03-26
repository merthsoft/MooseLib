namespace Merthsoft.Moose.Dungeon.Entities.Spells;
public record FlameDef : SpellDef { 
    public FlameDef() : base("Flame", 1, targetMode: TargetMode.FourWay) 
    {
        BlocksPlayer = false;
        TurnBased = true;
    } 
}
public class Flame : Spell
{
    public Flame(SpellDef def, DungeonObject owner, Vector2 position) : base(def, owner, position)
    {
        Position = new Vector2((int)position.X / 16 * 16, (int)position.Y / 16 * 16);
        State = Cast;
        StateCompleteAction = () =>
        {
            State = Active;
            StateCompleteAction = null;
            CurrentlyBlockingInput = false;
        };
    }

    public override void Update(MooseGame mooseGame, GameTime gameTime)
    {
        if (TurnsAlive < 0)
            State = Dead; 
        else if (!hasHit)
        {
            var (x, y) = GetCell();
            var monster = game.GetMonster(x, y);
            if (monster != null)
                State = Hit;
        }
        base.Update(mooseGame, gameTime);
        
        if (game.Player.HasInputThisFrame)
        {
            TurnsAlive--;
            hasHit = false;
        }

    }

    public override void Effect()
    {
        var (x, y) = GetCell();
        var target = game.GetMonster(x, y);
        target?.TakeDamage(2);
    }
}
