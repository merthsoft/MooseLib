
namespace Merthsoft.Moose.Dungeon.Entities.Monster;

public class Marshall : DungeonMonster
{
    public Marshall(MonsterDef def, Vector2? position) : base(def, position)
    {

    }

    protected override void MonsterUpdate(GameTime gameTime)
    {
        if (SeenCount == 0)
            return;
        var myCell = GetCell();
        var path = ParentMap.FindCellPath(myCell, game.Player.GetCell());
        if (!path.Any())
            return;
        var cell = path.FirstOrDefault();
        var nextMove = DirectionFrom(myCell - cell);
        if (nextMove == null)
            return;
        ProcessMove(nextMove);
    }

    private void ProcessMove(string move)
    {
        var moveDelta = Vector2.Zero;
        var newDirection = "";
        switch (move)
        {
            case Left:
                moveDelta = new(-1, 0);
                newDirection = Left;
                break;
            case Right:
                moveDelta = new(1, 0);
                newDirection = Right;
                break;
            case Down:
                moveDelta = new(0, 1);
                newDirection = Down;
                break;
            case Up:
                moveDelta = new(0, -1);
                newDirection = Up;
                break;
        }

        Direction = newDirection;

        this.AddTween(p => p.AnimationRotation, Direction == Left ? -1 : 1, .075f,
            onEnd: _ => this.AddTween(p => p.AnimationRotation, 0, .075f));

        if (moveDelta != Vector2.Zero)
        {
            var newcell = Position / 16 + moveDelta;
            var tile = game.GetDungeonTile((int)newcell.X, (int)newcell.Y);
            var playerCell = game.Player.GetCell();
            if (!tile.IsBlocking() && playerCell != newcell.ToPoint())
            {
                AnimationPosition = Vector2.Zero;
                moveDelta *= 16;
                this.AddTween(p => p.AnimationPosition, moveDelta, .2f,
                    onEnd: _ =>
                    {
                        Position += moveDelta;
                        AnimationPosition = Vector2.Zero;
                    });
            }
        }
    }
}