namespace Merthsoft.Moose.Dungeon.Entities.Items.Scrolls;
public class SpiralScroll : Scroll
{
    public SpiralScroll(ScrollDef def, int x, int y) : base(def, x, y)
    {

    }

    protected override IEnumerable<Point> AllowedCells()
    {
        var spiralEnumerator = player.Cell.SpiralAround().GetEnumerator();
        for (var i = 0; i < 200; i++)
            yield return spiralEnumerator.MoveNextGetCurrent();
    }
}
