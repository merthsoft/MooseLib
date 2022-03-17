namespace Merthsoft.Moose.Dungeon.Entities.Monsters;
public class Slime : DungeonMonster
{
    public Slime(MonsterDef def, Vector2? position) : base(def, position)
    {
    }

    protected override string MonsterUpdate(GameTime gameTime) 
    {
        if (NextMove != "")
            ProcessMove(NextMove);

        return game.Random.Next(4) switch
        {
            0 => Left,
            1 => Right,
            2 => Up,
            _ => Down,
        };
    }
}
