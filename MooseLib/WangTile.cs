﻿namespace Merthsoft.Moose.MooseEngine;

public record WangTile(int TileId)
{
    public List<int> North = [];
    public List<int> NorthEast = [];
    public List<int> East = [];
    public List<int> SouthEast = [];
    public List<int> South = [];
    public List<int> SouthWest = [];
    public List<int> West = [];
    public List<int> NorthWest = [];

    public int? AppliesTo = null;
        
    public string WangId 
        => $"{North.JoinString("|")},{NorthEast.JoinString("|")},{East.JoinString("|")},{SouthEast.JoinString("|")},{South.JoinString("|")},{SouthWest.JoinString("|")},{West.JoinString("|")},{NorthWest.JoinString("|")}";
    
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

        if (!matchValue(North, other.North[0]))
            return int.MinValue;

        if (!matchValue(NorthEast, other.NorthEast[0]))
            return int.MinValue;

        if (!matchValue(East, other.East[0]))
            return int.MinValue;

        if (!matchValue(SouthEast, other.SouthEast[0]))
            return int.MinValue;

        if (!matchValue(South, other.South[0]))
            return int.MinValue;

        if (!matchValue(SouthWest, other.SouthWest[0]))
            return int.MinValue;

        if (!matchValue(West, other.West[0]))
            return int.MinValue;

        if (!matchValue(NorthWest, other.NorthWest[0]))
            return int.MinValue;

        return match;
        
        bool matchValue(List<int> direction, int parsedIndex)
        {
            var index = -1;
            for (var i = direction.Count - 1; i >= 0; i--)
                if (direction[i] == 0 || direction[i] == parsedIndex)
                {
                    index = i;
                    break;
                }
            
            match += index + int.MaxValue / 10;
            return index > -1;
        }
    }

    public override string ToString() => $"{TileId}: {WangId}";
}
