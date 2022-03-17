namespace Merthsoft.Moose.MooseEngine;

internal record WangTile(int TileId)
{
    public int North = 0;
    public int NorthEast = 0;
    public int East = 0;
    public int SouthEast = 0;
    public int South = 0;
    public int SouthWest = 0;
    public int West = 0;
    public int NorthWest = 0;
        
    public string WangId => $"{North},{NorthEast},{East},{SouthEast},{South},{SouthWest},{West},{NorthWest}";
    public int Index => 0;

    public WangTile(int tileId, string wangId) : this(tileId)
    {
        var split = wangId.Split(",");
        North = int.Parse(split[0]);
        NorthEast = int.Parse(split[1]);
        East = int.Parse(split[2]);
        SouthEast = int.Parse(split[3]);
        South = int.Parse(split[4]);
        SouthWest = int.Parse(split[5]);
        West = int.Parse(split[6]);
        NorthWest = int.Parse(split[7]);
    }
}
