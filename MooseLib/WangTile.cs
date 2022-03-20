namespace Merthsoft.Moose.MooseEngine;

public record WangTile(int TileId)
{
    public List<int> North = new();
    public List<int> NorthEast = new();
    public List<int> East = new();
    public List<int> SouthEast = new();
    public List<int> South = new();
    public List<int> SouthWest = new();
    public List<int> West = new();
    public List<int> NorthWest = new();
        
    public string WangId => $"{North.JoinString("|")},{NorthEast.JoinString("|")},{East.JoinString("|")},{SouthEast.JoinString("|")},{South.JoinString("|")},{SouthWest.JoinString("|")},{West.JoinString("|")},{NorthWest.JoinString("|")}";
    
    public WangTile(int tileId, string wangId) : this(tileId)
    {
        var split = wangId.Split(",");
        North = split[0].Split('|').Select(int.Parse).ToList();
        NorthEast = split[1].Split('|').Select(int.Parse).ToList();
        East = split[2].Split('|').Select(int.Parse).ToList();
        SouthEast = split[3].Split('|').Select(int.Parse).ToList();
        South = split[4].Split('|').Select(int.Parse).ToList();
        SouthWest = split[5].Split('|').Select(int.Parse).ToList();
        West = split[6].Split('|').Select(int.Parse).ToList();
        NorthWest = split[7].Split('|').Select(int.Parse).ToList();
    }

    public virtual bool Equals(WangTile? wangTile)
        => WangId == wangTile?.WangId;

    public override int GetHashCode()
        => WangId.GetHashCode();

    public int Compare(WangTile other)
    {
        var match = 0;

        if (North[0] != 0 && !matchValue(North, other.North[0]))
            return int.MinValue;

        if (NorthEast[0] != 0 && !matchValue(NorthEast, other.NorthEast[0]))
            return int.MinValue;

        if (East[0] != 0 && !matchValue(East, other.East[0]))
            return int.MinValue;

        if (SouthEast[0] != 0 && !matchValue(SouthEast, other.SouthEast[0]))
            return int.MinValue;

        if (South[0] != 0 && !matchValue(South, other.South[0]))
            return int.MinValue;

        if (SouthWest[0] != 0 && !matchValue(SouthWest, other.SouthWest[0]))
            return int.MinValue;

        if (West[0] != 0 && !matchValue(West, other.West[0]))
            return int.MinValue;

        if (NorthWest[0] != 0 && !matchValue(NorthWest, other.NorthWest[0]))
            return int.MinValue;

        
        return match;
        
        bool matchValue(List<int> direction, int parsedIndex)
        {
            var index = direction.Reverse<int>().IndexOf(i => i == parsedIndex);
            match += index;
            return index > -1;
        }
    }

    public override string ToString() => $"{TileId}: {WangId}";
}
