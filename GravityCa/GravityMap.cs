using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Extension;
using Merthsoft.Moose.MooseEngine.Topologies;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace GravityCa;

public class GravityMap : BaseMap
{
    public UInt128[] GravityLayer;
    public UInt128[] MassLayer;

    private int height;
    public override int Height => height;
    private int width;
    public override int Width => width;
    public override int TileWidth => 1;
    public override int TileHeight => 1;

    private UInt128[] BackBoard;

    public int Generation { get; set; } = 0;
    public bool Running { get; set; } = false;
    public double TotalMass { get; private set; } = 0;
    public double MaxMass { get; private set; } = 0;
    public double MinMass { get; private set; }
    public double TotalGravity { get; private set; } = 0;
    public double MaxGravity { get; private set; } = 0;
    public double MinGravity { get; private set; }

    private readonly static AdjacentTile<UInt128>[] AdjacentTiles = new AdjacentTile<UInt128>[9];

    public int UpdateState { get; private set; } = 0;

    public GravityMap(int width, int height, Topology topology)
    {
        RendererKey = "map";
        Topology = topology;
        this.width = width;
        this.height = height;

        GravityLayer = new UInt128[Width * Height];
        MassLayer = new UInt128[Width * Height];
        BackBoard = new UInt128[Width * Height];
        Array.Fill<UInt128>(GravityLayer, 0);
        Array.Fill<UInt128>(MassLayer, 0);
        Array.Fill<UInt128>(BackBoard, 0);

        var index = 0;
        for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
                AdjacentTiles[index++] = new() { XOffset = x, YOffset = y };
    }

    public override int IsBlockedAt(string layer, int x, int y)
        => 0;

    public void SetMass(int x, int y, UInt128 mass)
        => SafeSet(MassLayer, x, y, Width, Height, mass, Topology);

    public UInt128 GetMassAt(int x, int y, UInt128 @default)
        => SafeGet(MassLayer, x, y, Width, Height, Topology, @default);

    public UInt128 GetGravityAt(int x, int y, UInt128 @default)
            => SafeGet(GravityLayer, x, y, Width, Height, Topology, @default);

    public AdjacentTile<UInt128>[] GetGravityAdjacent(int x, int y)
    {
        FillAdjacentCells(GravityLayer, x, y, Width, Height, Topology);
        return AdjacentTiles;
    }

    public void SetGravity(int x, int y, UInt128 gravity)
        => SafeSet(GravityLayer, x, y, Width, Height, gravity, Topology);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SafeSet(UInt128[] array, int x, int y, int w, int h, UInt128 v, Topology topology)
    {
        TopologyHelper.TranslatePoint(x, y, topology, w, h, out var lX, out var lY);
        if (lX < 0 || lY < 0 || lX >= w || lY >= h)
            return;
        array[lX * w + lY] = v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt128 SafeGet(UInt128[] array, int x, int y, int w, int h, Topology topology, UInt128 @default)
    {
        TopologyHelper.TranslatePoint(x, y, topology, w, h, out var lX, out var lY);
        if (lX < 0 || lY < 0 || lX >= w || lY >= h)
            return @default;
        return array[lX * w + lY];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FillAdjacentCells(UInt128[] array, int x, int y, int w, int h, Topology topology)
    {
        AdjacentTiles[0].Value = SafeGet(array, x - 1, y - 1, w, h, topology, 0);
        AdjacentTiles[1].Value = SafeGet(array, x - 1, y, w, h, topology, 0);
        AdjacentTiles[2].Value = SafeGet(array, x - 1, y + 1, w, h, topology, 0);
        AdjacentTiles[3].Value = SafeGet(array, x, y - 1, w, h, topology, 0);
        AdjacentTiles[4].Value = SafeGet(array, x, y, w, h, topology, 0);
        AdjacentTiles[5].Value = SafeGet(array, x, y + 1, w, h, topology, 0);
        AdjacentTiles[6].Value = SafeGet(array, x + 1, y - 1, w, h, topology, 0);
        AdjacentTiles[7].Value = SafeGet(array, x + 1, y, w, h, topology, 0);
        AdjacentTiles[8].Value = SafeGet(array, x + 1, y + 1, w, h, topology, 0);
    }

    public Color GetColorAt(int i, int j)
    {
        Color? color = null;
        if (GravityGame.DrawMass && TotalMass > 0)
            color = GetColor(MassLayer, i, j, GravityGame.MassColors, (double)GravityGame.MaxMass, GravityGame.MassMinDrawValue, null, MinMass, MaxMass, LerpMode.ZeroToSystemMax);
        if (GravityGame.DrawGravity && color == null && TotalGravity > 0)
            color = GetColor(GravityLayer, i, j, GravityGame.GravityColors, (double)GravityGame.MaxGravity, null, null, MinGravity, MaxGravity, GravityGame.GravityColorLerpMode);

        return color ?? Color.Transparent;
    }
    private Color? GetColor(
                       UInt128[] tileLayer,
                       int i,
                       int j,
                       Color[] colors,
                       double overallMax,
                       UInt128? minDrawValue,
                       UInt128? maxDrawValue,
                       double systemMin,
                       double systemMax,
                       LerpMode lerpMode)
    {
        var value = (double)tileLayer[i * Width + j];
        var percentage = lerpMode switch
        {
            LerpMode.ZeroToSystemMax => value / systemMax,
            LerpMode.SystemMinToSystemMax => (value - systemMin) / (systemMax - systemMin),
            _ => value / overallMax,
        };

        if (minDrawValue.HasValue && value < (double)minDrawValue.Value
            || maxDrawValue.HasValue && value > (double)maxDrawValue.Value
            || percentage == 0)
        {
            return null;
        }
        else if (percentage >= 1)
        {
            return colors.Last();
        }

        if (percentage <= 0 || double.IsNaN(percentage) || double.IsInfinity(percentage) || !double.IsPositive(percentage))
            return colors[0];

        var colorLocation = (colors.Length - 1) * percentage;
        var colorIndex = (int)colorLocation;
        var newPercentage = colorLocation - colorIndex;
        return GraphicsExtensions.ColorGradientPercentage(colors[colorIndex], colors[colorIndex + 1], newPercentage);
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        if (!Running)
        {
            UpdateTotals();
            return;
        }

        Generation++;
        Array.Fill<UInt128>(BackBoard, 0);
        if (UpdateState == 0)
        {
            TotalGravity = 0;
            MaxGravity = 0;
            MinGravity = double.PositiveInfinity;
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    FillAdjacentCells(GravityLayer, x, y, Width, Height, Topology);
                    const int massReducer = 3;
                    var adjGrav = (AdjacentTiles[0].Value >> massReducer)
                                + (AdjacentTiles[1].Value >> massReducer)
                                + (AdjacentTiles[2].Value >> massReducer)
                                + (AdjacentTiles[3].Value >> massReducer)
                                + (AdjacentTiles[5].Value >> massReducer)
                                + (AdjacentTiles[6].Value >> massReducer)
                                + (AdjacentTiles[7].Value >> massReducer)
                                + (AdjacentTiles[8].Value >> massReducer);
                    var gravity = (MassLayer[x * Width + y] >> 4) + adjGrav;
                    if (gravity == 0 && adjGrav > 0)
                        gravity = 1;

                    if (gravity < 0)
                        gravity = 0;

                    if (gravity > GravityGame.MaxGravity)
                        gravity = GravityGame.MaxGravity;

                    BackBoard[x * Width + y] = gravity;

                    var floatingPointGravity = (double)gravity;

                    if (floatingPointGravity > MaxGravity)
                        MaxGravity = (double)gravity;

                    if (floatingPointGravity < MinGravity)
                        MinGravity = floatingPointGravity;

                    TotalGravity += floatingPointGravity;
                }

            Array.Copy(BackBoard, GravityLayer, Width * Height);
        }
        else if (UpdateState == 1)
        {
            TotalMass = 0;
            MaxMass = 0;
            MinMass = double.PositiveInfinity;
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var mass = MassLayer[x * Width + y];
                    if (mass == 0)
                    {
                        continue;
                    }

                    var floatingPointMass = (double)mass;
                    TotalMass += floatingPointMass;
                    if (floatingPointMass > MaxMass)
                        MaxMass = floatingPointMass;

                    if (floatingPointMass < MinMass)
                        MinMass = floatingPointMass;

                    FillAdjacentCells(MassLayer, x, y, Width, Height, Topology);

                    var surrounded = AdjacentTiles.All(t => t.Value > 0);
                    var set = false;

                    if (!surrounded)
                    {
                        var spotGrav = GravityLayer[x * Width + y];

                        FillAdjacentCells(GravityLayer, x, y, Width, Height, Topology);
                        var gravList = AdjacentTiles.GroupBy(x => x.Value).OrderByDescending(x => x.Key);
                        foreach (var g in gravList)
                        {
                            if (g.Key < spotGrav)
                                break;
                            foreach (var selected in g.Shuffle())
                            {
                                var (xOffset, yOffset, _) = selected;
                                var (newX, newY) = TranslatePoint(x + xOffset, y + yOffset);
                                if (newX < 0 || newX >= Width || newY < 0 || newY >= Height
                                    || MassLayer[newX * Width + newY] != 0
                                    || BackBoard[newX * Width + newY] != 0)
                                    continue;
                                BackBoard[newX * Width + newY] = mass;
                                MassLayer[x * Width + y] = 0;
                                set = true;
                                break;
                            }
                            if (set)
                                break;
                        }
                    }
                    if (!set)
                        BackBoard[x * Width + y] = mass;
                }

            Array.Copy(BackBoard, MassLayer, Width * Height);
        }
        else if (UpdateState == 2)
        {
            TotalMass = 0;
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var mass = MassLayer[x * Width + y];
                    if (mass == 0)
                    {
                        continue;
                    }
                    var floatingPointMass = (double)mass;
                    TotalMass += floatingPointMass;
                    if (floatingPointMass > MaxMass)
                        MaxMass = floatingPointMass;

                    if (floatingPointMass < MinMass)
                        MinMass = floatingPointMass;

                    FillAdjacentCells(MassLayer, x, y, Width, Height, Topology);

                    var surrounded = AdjacentTiles.Count(t => t.Value > 0) > 1;
                    var set = false;
                    var hungry = MooseGame.Random.NextDouble() < .25f;
                    if (hungry && surrounded && mass < GravityGame.MaxMass)
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
                                if (eatenMass + mass < GravityGame.MaxMass)
                                {
                                    BackBoard[x * Width + y] = mass + eatenMass;
                                    BackBoard[newX * Width + newY] = 0;
                                    MassLayer[newX * Width + newY] = 0;
                                    set = true;
                                }
                                else
                                {
                                    //var diff = GravityCellularAutomataGame.MaxMass - mass;
                                    //BackBoard[x * Width + y] = GravityCellularAutomataGame.MaxMass;
                                    //BackBoard[newX, newY] = eatenMass - diff;
                                    //MassLayer[newX, newY] = eatenMass - diff;
                                    //set = true;
                                }
                            }
                        }
                    }
                    if (!set)
                        BackBoard[x * Width + y] = mass;
                }

            Array.Copy(BackBoard, MassLayer, Width * Height);
        }

        UpdateState = (UpdateState + 1) % 4;
    }

    private void UpdateTotals()
    {
        TotalMass = 0;
        MaxMass = 0;
        MinMass = double.PositiveInfinity;
        TotalGravity = 0;
        MaxGravity = 0;
        MinGravity = double.PositiveInfinity;

        for (var x = 0; x < Width * Height; x++)
        {
            BackBoard[x] = 0;
            var gravity = GravityLayer[x];
            var mass = MassLayer[x];

            if (gravity != 0)
            {
                var floatingPointGravity = (double)gravity;

                if (floatingPointGravity > MaxGravity)
                    MaxGravity = (double)gravity;

                if (floatingPointGravity < MinGravity)
                    MinGravity = floatingPointGravity;

                TotalGravity += floatingPointGravity;
            }

            if (mass != 0)
            {
                var floatingPointMass = (double)mass;
                TotalMass += floatingPointMass;
                if (floatingPointMass > MaxMass)
                    MaxMass = floatingPointMass;

                if (floatingPointMass < MinMass)
                    MinMass = floatingPointMass;
            }
        }
    }

    internal void Reset()
    {
        Generation = 0;
        Array.Fill<UInt128>(GravityLayer, 0);
        Array.Fill<UInt128>(MassLayer, 0);
        Array.Fill<UInt128>(BackBoard, 0);
        TotalMass = 0;
        MaxMass = 0;
        MinMass = MaxMass;
        TotalGravity = 0;
        MaxGravity = 0;
        MinGravity = MaxGravity;
    }
}