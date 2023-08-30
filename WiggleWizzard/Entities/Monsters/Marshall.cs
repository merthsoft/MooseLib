
namespace Merthsoft.Moose.Dungeon.Entities.Monsters;

public class Marshall : DungeonMonster
{
    public Marshall(MonsterDef def, Vector2? position) : base(def, position)
    {

    }

    protected override string MonsterUpdate(GameTime gameTime)
    {
        if (SeenCount == 0)
            return "";

        if (NextMove != "")
            ProcessMove(NextMove);
        var myCell = Cell;
        var path = ParentMap.FindCellPath(myCell, game.Player.Cell);
        if (!path.Any())
            return "";
        var cell = path.FirstOrDefault();
        return DirectionFrom(myCell - cell)!;
    }
}