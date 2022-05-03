namespace RayMapEditor;

public static class FloodFill
{
    public static IEnumerable<IntVec2> Fill(IntVec2 s, IntVec2 t, SaveMap saveMap)
    {
        var ret = new HashSet<IntVec2>();

        var cells = new Queue<IntVec2>();
        cells.Enqueue(t);

        while (cells.Count() > 0 && ret.Count() < 15000)
        {
            var cell = cells.Dequeue();
            if (cell.x < 0 || cell.y < 0 || cell.x >= saveMap.Width || cell.y >= saveMap.Height)
                continue;
            if (ret.Contains(cell))
                continue;
            
            var addFlag = false;
            var neighborsFlag = false;

            if (addFlag)
                ret.Add(cell);
            if (neighborsFlag)
            {
                cells.Enqueue(cell + IntVec2.North);
                cells.Enqueue(cell + IntVec2.East);
                cells.Enqueue(cell + IntVec2.South);
                cells.Enqueue(cell + IntVec2.West);
            }
        }

        return ret;
    }
}