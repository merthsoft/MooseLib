using Merthsoft.Moose.MooseEngine.BaseDriver;

namespace GravityCa;
public class GravityMap : BaseMap
{
    public readonly TileLayer<UInt128> GravityLayer;
    public readonly TileLayer<UInt128> MassLayer;

    private int height;
    public override int Height => height;
    private int width;
    public override int Width => width;
    public override int TileWidth => 1;
    public override int TileHeight => 1;

    private static UInt128[,] BackBoard = null!;

    public int Generation { get; private set; } = 0;

    private static AdjacentTile<UInt128>[] AdjacentTiles = new AdjacentTile<UInt128>[9];

    private int UpdateState = 0;

    public GravityMap(int width, int height)
    {
        this.width = width;
        this.height = height;

        GravityLayer = AddLayer(new TileLayer<UInt128>("gravity", Width, Height, 0) { RendererKey = "gravity" });
        MassLayer = AddLayer(new TileLayer<UInt128>("mass", Width, Height, 0) { RendererKey = "mass" });

        BackBoard = new UInt128[Width, Height];

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                BackBoard[x, y] = 0;

        var index = 0;
        for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
                AdjacentTiles[index++] = new() { XOffset = x, YOffset = y };
    }

    protected override int IsBlockedAt(string layer, int x, int y) 
        => 0;

    public void SetMass(int x, int y, UInt128 mass)
        => MassLayer[x, y] = mass;

    public void AddMass(int x, int y, UInt128 mass)
    {
        MassLayer[x, y] += mass;
        if (MassLayer[x, y] > GravityCellularAutomataGame.MaxValue)
            MassLayer[x, y] = GravityCellularAutomataGame.MaxValue;
    }

    public void SubtractMass(int x, int y, UInt128 mass)
    {
        MassLayer[x, y] -= mass;
        if (MassLayer[x, y] > GravityCellularAutomataGame.MaxValue)
            MassLayer[x, y] = GravityCellularAutomataGame.MaxValue;
    }

    public void SetGravity(int x, int y, UInt128 gravity)
        => GravityLayer[x, y] = gravity;

    public void Update()
    {
        Generation++;

        if (UpdateState == 0)
        {
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                    BackBoard[x, y] = 0;

            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var adjGrav = GravityLayer[x - 1, y - 1]
                                + GravityLayer[x - 1, y]
                                + GravityLayer[x - 1, y + 1]
                                + GravityLayer[x, y - 1]
                                + GravityLayer[x, y + 1]
                                + GravityLayer[x + 1, y - 1]
                                + GravityLayer[x + 1, y]
                                + GravityLayer[x + 1, y + 1];
                    var gravity = (MassLayer[x, y] >> 3) + (adjGrav >> 3);
                    if (gravity == 0 && adjGrav > 0)
                        gravity = 1;

                    if (gravity < 0)
                        gravity = 0;

                    if (gravity > GravityCellularAutomataGame.MaxValue)
                        gravity = GravityCellularAutomataGame.MaxValue;

                    BackBoard[x, y] = gravity;
                }

            GravityLayer.CopyTiles(BackBoard);
        }
        else
        {
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                    BackBoard[x, y] = 0;

            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var mass = MassLayer[x, y];
                    if (mass == 0)
                    {
                        continue;
                    }
                    var spotGrav = GravityLayer[x, y];

                    AdjacentTiles[0].Value = GravityLayer[x - 1, y - 1];
                    AdjacentTiles[1].Value = GravityLayer[x - 1, y];
                    AdjacentTiles[2].Value = GravityLayer[x - 1, y + 1];
                    AdjacentTiles[3].Value = GravityLayer[x, y - 1];
                    AdjacentTiles[4].Value = spotGrav;
                    AdjacentTiles[5].Value = GravityLayer[x, y + 1];
                    AdjacentTiles[6].Value = GravityLayer[x + 1, y - 1];
                    AdjacentTiles[7].Value = GravityLayer[x + 1, y];
                    AdjacentTiles[8].Value = GravityLayer[x + 1, y + 1];

                    var gravList = AdjacentTiles.OrderByDescending(x => x.Value).GroupBy(x => x.Value);
                    var set = false;
                    foreach (var g in gravList)
                    {
                        if (g.Key < spotGrav)
                            break;
                        foreach (var selected in g)
                        {
                            var (xOffset, yOffset, _) = selected;
                            if (xOffset < -1 || xOffset > 1 || yOffset < -1 || yOffset > 1
                                || MassLayer[x + xOffset, y + yOffset] != 0)
                                continue;
                            BackBoard[x + xOffset, y + yOffset] = mass;
                            MassLayer[x, y] = 0;
                            set = true;
                            break;
                        }
                        if (set)
                            break;
                    }

                    if (!set)
                        BackBoard[x, y] = mass;
                }

            MassLayer.CopyTiles(BackBoard);
        }

        UpdateState = (UpdateState + 1) % 2;
    }
}