namespace Merthsoft.Moose.Dungeon.Entities.Items.Scrolls;
public class RookScroll : Scroll
{
    public RookScroll(ScrollDef def, int x, int y) : base(def, x, y)
    {

    }

    protected override IEnumerable<Point> AllowedCells()
    {
        var (playerX, playerY) = player.GetCell();
        var deltaX = -1;
        while (!game.GetDungeonTile(playerX + deltaX, playerY).BlocksSight())
        {
            yield return new(playerX + deltaX, playerY);
            deltaX -= 1;
        }

        deltaX = 1;
        while (!game.GetDungeonTile(playerX + deltaX, playerY).BlocksSight())
        {
            if (!game.IsCellOccupied(playerX + deltaX, playerY))
                yield return new(playerX + deltaX, playerY);
            deltaX += 1;
        }

        var deltaY = -1;
        while (!game.GetDungeonTile(playerX, playerY + deltaY).BlocksSight())
        {
            yield return new(playerX, playerY + deltaY);
            deltaY -= 1;
        }

        deltaY = 1;
        while (!game.GetDungeonTile(playerX, playerY + deltaY).BlocksSight())
        {
            yield return new(playerX, playerY + deltaY);
            deltaY += 1;
        }
    }
}
