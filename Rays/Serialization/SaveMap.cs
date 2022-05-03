namespace Merthsoft.Moose.Rays.Serialization;
public record SaveMap()
{
    public string Name { get; set; } = "New Map";
    public int Width { get; set; }
    public int Height { get; set; }

    public int[][] Walls { get; set; } = new int[0][];
    public List<SaveItem> Items { get; set; } = new();

    public SaveMap(int width, int height) : this()
    {
        Width = width;
        Height = height;
        Walls = new int[width][];
        for (var x = 0; x < width; x++)
            Walls[x] = new int[height];
    }

    public SaveMap DeepCopy()
    {
        var ret = new SaveMap(Width, Height);
        foreach (var item in Items)
            ret.Items.Add(item with { });

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                ret.Walls[x][y] = Walls[x][y];

        return ret;
    }
}

public enum ItemType { Object, Door, Actor, Special };
public record SaveItem(ItemType ItypeType, string Name, int X, int Y);