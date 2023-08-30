using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Extension;
using Merthsoft.Moose.MooseEngine.Topologies;

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
    public double TotalMass { get; private set; } = 0;

    private static AdjacentTile<UInt128>[] AdjacentTiles = new AdjacentTile<UInt128>[9];

    public int UpdateState { get; private set; } = 0;

    public GravityMap(int width, int height, Topology topology)
    {
        Topology = topology;
        this.width = width;
        this.height = height;

        GravityLayer = AddLayer(new TileLayer<UInt128>("gravity", Width, Height, 0) { RendererKey = "gravity", Topology = topology });
        MassLayer = AddLayer(new TileLayer<UInt128>("mass", Width, Height, 0) { RendererKey = "mass", Topology = topology });

        BackBoard = new UInt128[Width, Height];

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                BackBoard[x, y] = 0;

        var index = 0;
        for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
                AdjacentTiles[index++] = new() { XOffset = x, YOffset = y };
    }

    public override int IsBlockedAt(string layer, int x, int y) 
        => 0;

    public void SetMass(int x, int y, UInt128 mass)
        => MassLayer[x, y] = mass;

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
                    var adjGrav = (GravityLayer[x - 1, y - 1] >> 3)
                                + (GravityLayer[x - 1, y] >> 3)
                                + (GravityLayer[x - 1, y + 1] >> 3)
                                + (GravityLayer[x, y - 1] >> 3)
                                + (GravityLayer[x, y + 1] >> 3)
                                + (GravityLayer[x + 1, y - 1] >> 3)
                                + (GravityLayer[x + 1, y] >> 3)
                                + (GravityLayer[x + 1, y + 1] >> 3);
                    var gravity = (MassLayer[x, y] >> 4) + adjGrav;
                    if (gravity == 0 && adjGrav > 0)
                        gravity = 1;

                    if (gravity < 0)
                        gravity = 0;

                    if (gravity > GravityCellularAutomataGame.MaxGravity)
                        gravity = GravityCellularAutomataGame.MaxGravity;

                    BackBoard[x, y] = gravity;
                }

            GravityLayer.CopyTiles(BackBoard);
        }
        else if (UpdateState == 1)
        {
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                    BackBoard[x, y] = 0;

            TotalMass = 0;
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var mass = MassLayer[x, y];
                    if (mass == 0)
                    {
                        continue;
                    }
                    TotalMass += (double)mass;

                    AdjacentTiles[0].Value = MassLayer[x - 1, y - 1];
                    AdjacentTiles[1].Value = MassLayer[x - 1, y];
                    AdjacentTiles[2].Value = MassLayer[x - 1, y + 1];
                    AdjacentTiles[3].Value = MassLayer[x, y - 1];
                    AdjacentTiles[4].Value = mass;
                    AdjacentTiles[5].Value = MassLayer[x, y + 1];
                    AdjacentTiles[6].Value = MassLayer[x + 1, y - 1];
                    AdjacentTiles[7].Value = MassLayer[x + 1, y];
                    AdjacentTiles[8].Value = MassLayer[x + 1, y + 1];

                    var surrounded = AdjacentTiles.All(t => t.Value > 0);
                    var set = false;

                    if (!surrounded)
                    {
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

                        var gravList = AdjacentTiles.GroupBy(x => x.Value).OrderByDescending(x => x.Key);
                        foreach (var g in gravList)
                        {
                            if (g.Key < spotGrav)
                                break;
                            foreach (var selected in g.Shuffle())
                            {
                                var (xOffset, yOffset, _) = selected;
                                var (newX, newY) = TranslatePoint(x + xOffset, y + yOffset);
                                if (newX < 0 || newX > Width || newY < 0 || newY > Height
                                    || MassLayer[newX, newY] != 0
                                    || BackBoard[newX, newY] != 0)
                                    continue;
                                BackBoard[newX, newY] = mass;
                                MassLayer[x, y] = 0;
                                set = true;
                                break;
                            }
                            if (set)
                                break;
                        }
                    }
                    if (!set)
                        BackBoard[x, y] = mass;
                }

            MassLayer.CopyTiles(BackBoard);
        }
        else if (UpdateState == 2)
        {
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                    BackBoard[x, y] = 0;

            TotalMass = 0;
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var mass = MassLayer[x, y];
                    if (mass == 0)
                    {
                        continue;
                    }
                    TotalMass += (double)mass;

                    AdjacentTiles[0].Value = MassLayer[x - 1, y - 1];
                    AdjacentTiles[1].Value = MassLayer[x - 1, y];
                    AdjacentTiles[2].Value = MassLayer[x - 1, y + 1];
                    AdjacentTiles[3].Value = MassLayer[x, y - 1];
                    AdjacentTiles[4].Value = mass;
                    AdjacentTiles[5].Value = MassLayer[x, y + 1];
                    AdjacentTiles[6].Value = MassLayer[x + 1, y - 1];
                    AdjacentTiles[7].Value = MassLayer[x + 1, y];
                    AdjacentTiles[8].Value = MassLayer[x + 1, y + 1];

                    var surrounded = AdjacentTiles.Count(t => t.Value > 0) > 1;
                    var set = false;
                    var hungry = MooseGame.Instance.Random.NextDouble() < .25f;
                    if (hungry && surrounded && mass < GravityCellularAutomataGame.MaxMass)
                    {
                        var smallestGroup = AdjacentTiles.Where((x, i) => i != 4 && x.Value > 0 && x.Value <= mass)
                                                            .GroupBy(x => x.Value).OrderBy(x => x.Key).FirstOrDefault();
                        var eaten = smallestGroup?.Shuffle().First();
                        if (eaten != null)
                        {
                            var (xOffset, yOffset, eatenMass) = eaten.Value;
                            var (newX, newY) = TranslatePoint(x + xOffset, y + yOffset);
                            if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
                            {
                                if (eatenMass + mass < GravityCellularAutomataGame.MaxMass)
                                {
                                    BackBoard[x, y] = mass + eatenMass;
                                    BackBoard[newX, newY] = 0;
                                    MassLayer[newX, newY] = 0;
                                    set = true;
                                }
                                else
                                {
                                    //var diff = GravityCellularAutomataGame.MaxMass - mass;
                                    //BackBoard[x, y] = GravityCellularAutomataGame.MaxMass;
                                    //BackBoard[newX, newY] = eatenMass - diff;
                                    //MassLayer[newX, newY] = eatenMass - diff;
                                    //set = true;
                                }
                            }
                        }
                    }
                    if (!set)
                        BackBoard[x, y] = mass;
                }

            MassLayer.CopyTiles(BackBoard);
        }

        UpdateState = (UpdateState + 1) % 3;
    }
}