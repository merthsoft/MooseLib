namespace Merthsoft.Moose.Dungeon.Entities.Items.Scrolls;
public class SpiralScroll : Scroll
{
    public SpiralScroll(ScrollDef def, Vector2 position) : base(def, position)
    {

    }

    protected override IEnumerable<Point> AllowedCells()
    {
        var spiralEnumerator = player.GetCell().SpiralAround().GetEnumerator();
        for (var i = 0; i < 200; i++)
            yield return spiralEnumerator.MoveNextGetCurrent();
    }
}
