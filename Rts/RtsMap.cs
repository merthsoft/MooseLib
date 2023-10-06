using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.Noise;
using Merthsoft.Moose.MooseEngine.PathFinding.Maps;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.Moose.Rts;

internal class RtsMap : PathFinderMap
{
    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; } = 8;
    public override int TileHeight { get; } = 8;

    public TileLayer<TerrainTile> TerrainLayer { get; }
    public TileLayer<ResourceTile> ResourceLayer { get; }
    public TileLayer<ItemTile> ItemLayer { get; } = null!;
    public ObjectLayer<Unit> UnitLayer { get; }

    private Dictionary<int, int> indexMap = new()
    {
        [0b00000000] = 0,
        [0b10000000] = 1,
        [0b11100000] = 2,
        [0b00100000] = 3,
        [0b10010100] = 4,
        [0b00101001] = 5,
        [0b00000100] = 6,
        [0b00000111] = 7,
        [0b00000001] = 8,
        [0b00101111] = 9,
        [0b10010111] = 10,
        [0b11101001] = 11,
        [0b11110100] = 12,
        [0b00000011] = 7,
        [0b00001001] = 5,
        [0b01100000] = 2,
        [0b00000110] = 7,
        [0b00010100] = 4,
        [0b11010100] = 2,
        [0b10010000] = 4,
        [0b00101000] = 5,
        [0b11000000] = 2
    };

    public RtsMap(int width, int height) : base(new FlowFieldPathFinder())
    {
        Height = height;
        Width = width;
        TerrainLayer = AddLayer(new TileLayer<TerrainTile>("terrain", Width, Height, TerrainTile.Water) { RendererKey = "terrain" });
        ResourceLayer = AddLayer(new TileLayer<ResourceTile>("resource", Width, Height, ResourceTile.Empty) { RendererKey = "resource" });
        ItemLayer = AddLayer(new TileLayer<ItemTile>("item", Width, Height, ItemTile.Empty) { RendererKey = "item" });
        UnitLayer = AddLayer(new ObjectLayer<Unit>("units", width, height));
    }

    public IEnumerable<Unit> GetUnits()
        => UnitLayer.Objects;

    public override int IsBlockedAt(string layer, int x, int y)
        => layer switch
        {
            "terrain" => TerrainLayer.GetTileValue(x, y) < TerrainTile.Land ? 1 : 0,
            "resource" => ResourceLayer.GetTileValue(x, y) >= ResourceTile.Tree1_Start ? 1 : 0,
            "item" => 0,
            "units" => UnitLayer.ObjectMap[x, y].Count,
            _ => GetBlockingVector(x, y).Sum(),
        };

    public bool IsCellInBounds(int x, int y)
        => CellIsInBounds(x, y);

    private enum GeneratedType
    {
        Land,
        Water,
        WaterDecoration,
        Grass,
        Tree,
        Stump,
        Mushroom,
        Flower,
        Overgrowth
    }

    private void SetTerrainLayerSquare(int x, int y, TerrainTile? value = null, Func<int, TerrainTile>? valueGenerator = null)
    {
        TerrainLayer.SetTileValue(x * 2, y * 2, value ?? valueGenerator!(0));
        TerrainLayer.SetTileValue(x * 2 + 1, y * 2, value ?? valueGenerator!(1));
        TerrainLayer.SetTileValue(x * 2, y * 2 + 1, value ?? valueGenerator!(2));
        TerrainLayer.SetTileValue(x * 2 + 1, y * 2 + 1, value ?? valueGenerator!(3));
    }

    private void SetResourceLayerSquare(int x, int y, ResourceTile? value = null, Func<int, double, ResourceTile>? valueGenerator = null)
    {
        var coinFlip = MooseGame.Random.NextDouble();
        ResourceLayer.SetTileValue(x * 2, y * 2, value ?? valueGenerator!(0, coinFlip));
        ResourceLayer.SetTileValue(x * 2 + 1, y * 2, value ?? valueGenerator!(1, coinFlip));
        ResourceLayer.SetTileValue(x * 2, y * 2 + 1, value ?? valueGenerator!(2, coinFlip));
        ResourceLayer.SetTileValue(x * 2 + 1, y * 2 + 1, value ?? valueGenerator!(3, coinFlip));
    }

    public void RandomizeMap()
    {
        UnitLayer.Objects.ForEach(o => o.Remove = true);
        MooseGame.Instance.SetSeed((int)DateTime.Now.Ticks);
        var treeNoise = GenNoise(Width / 2, Height / 2, .1f);
        var waterNoise = GenNoise(Width / 2, Height / 2, .02f);
        var fieldNoise = GenNoise(Width / 2, Height / 2, .02f);

        for (var x = 0; x < Width/2; x += 1)
            for (var y = 0; y < Height/2; y += 1)
            {
                var coinFlip = MooseGame.Random.NextDouble();
                var water = waterNoise[x, y];
                var tree = treeNoise[x, y];
                var field = fieldNoise[x, y];

                var value = (water, tree, field, coinFlip) switch
                {
                    ( < 23, _, _, <= .69) => GeneratedType.Overgrowth,
                    ( < 23, _, _, _) => GeneratedType.Mushroom,
                    ( < 29, _, _, _) => GeneratedType.WaterDecoration,
                    ( < 55, _, _, _) => GeneratedType.Water,
                    (_, > 110 and < 150, _, >.1) => GeneratedType.Grass,
                    (_, > 110 and < 150, _, _) => GeneratedType.Flower,
                    (_, _, < 40, _) => GeneratedType.Flower,
                    (_, >= 150 and < 250, _, < .1) => GeneratedType.Stump,
                    (_, >= 150 and < 250, _, < .2) => GeneratedType.Mushroom,
                    (_, >= 150 and < 250, _, _) => GeneratedType.Tree,
                    (_, >= 250, _, _) => GeneratedType.Mushroom,
                    (_, _, _, < .1) => GeneratedType.Grass,
                    (_, _, _, < .2) => GeneratedType.Stump,
                    (_, _, _, < .3) => GeneratedType.Flower,
                    (_, _, _, < .4) => GeneratedType.Mushroom,
                    _ => GeneratedType.Land,
                };

                SetTerrainLayerSquare(x, y, TerrainTile.Land);
                SetResourceLayerSquare(x, y, ResourceTile.Empty);

                switch (value)
                {
                    case GeneratedType.Water:
                        SetTerrainLayerSquare(x, y, TerrainTile.Water);
                        break;
                    case GeneratedType.Grass:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomGrass);
                        break;
                    case GeneratedType.Tree:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomGrass);
                        SetResourceLayerSquare(x, y, valueGenerator: randomTree);
                        break;
                    case GeneratedType.Stump:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomGrass);
                        SetResourceLayerSquare(x, y, valueGenerator: (i, _) => i + ResourceTile.Stump1_Start);
                        break;
                    case GeneratedType.Flower:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomFlower);
                        break;
                    case GeneratedType.Mushroom:
                        SetTerrainLayerSquare(x, y, TerrainTile.Grass_Start);
                        SetResourceLayerSquare(x, y, valueGenerator: randomMushroom);
                        break;
                    case GeneratedType.WaterDecoration:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomWaterDecoration);
                        break;
                    case GeneratedType.Overgrowth:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomOvergrowth);
                        break;
                    case GeneratedType.Land:
                    default:
                        break;
                }
                
            }

        // Convolution on final map
        // Auto-tile the water, add some other random elements
        for (var x = 0; x < Width; x += 1)
            for (var y = 0; y < Height; y += 1)
            {
                ItemLayer.SetTileValue(x, y, ItemTile.Empty);
                var coinFlip = MooseGame.Random.NextDouble();
                var terrainTile = TerrainLayer.GetTileValue(x, y);
                if (terrainTile >= TerrainTile.Water && terrainTile <= TerrainTile.WaterDecoration_End)
                {
                    var neighbors = CountNeighbors((int)TerrainTile.Water, (int)TerrainTile.WaterDecoration_End, x, y, TerrainLayer);
                    if (neighbors != 0)
                        terrainTile = (TerrainTile)((int)TerrainTile.Water + indexMap.GetValueOrDefault(neighbors));
                    else if (coinFlip < .05)
                    {
                        terrainTile = randomWaterDecoration();
                    }
                    
                    TerrainLayer.SetTileValue(x, y, terrainTile);
                } else if (terrainTile == TerrainTile.Land && coinFlip < 0.05)
                {
                    TerrainLayer.SetTileValue(x, y, randomTerrainDecoration());
                }
            }

        IsBlockingMapDirty = true;

        ResourceTile randomTree(int index, double coinFlip)
            => (ResourceTile)(index + (int)(coinFlip switch {
                < .4 => ResourceTile.Tree1_Start,
                < .5 => ResourceTile.Tree2_Start,
                < .9 => ResourceTile.Tree3_Start,
                _ => ResourceTile.Tree4_Start,
            }));

        ResourceTile randomMushroom(int _i = 0, double _cf= 0)
            => (ResourceTile)MooseGame.Random.Next((int)ResourceTile.Mushroom_Start, (int)ResourceTile.Mushroom_End + 1);

        TerrainTile randomOvergrowth(int _ = 0)
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.Grass_Start, (int)TerrainTile.Flower_End + 1);

        TerrainTile randomGrass(int _ = 0)
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.Grass_Start, (int)TerrainTile.Grass_End + 1);

        TerrainTile randomFlower(int _ = 0)
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.Flower_Start, (int)TerrainTile.Flower_End + 1);

        TerrainTile randomTerrainDecoration(int _ = 0)
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.Grass_Start, (int)TerrainTile.Flower_End + 1);

        TerrainTile randomWaterDecoration(int _ = 0)
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.WaterDecoration_Start, (int)TerrainTile.WaterDecoration_End + 1);
    }

    private float[,] GenNoise(int width, int height, float scale)
    {
        SimplexNoise.Seed++;
        return SimplexNoise.Calc2D(width, height, scale);
    }

    protected int CountNeighbors(int tile_start, int tile_end, int x, int y, ITileLayer layer)
    {
        var total = 0;
        var north = GetNeighborValue(tile_start, tile_end, x + 0, y + -1, layer, 0b00000010);
        total += north;
        var west = GetNeighborValue(tile_start, tile_end, x + -1, y + 0, layer,  0b00001000);
        total += west;
        var east = GetNeighborValue(tile_start, tile_end, x + 1, y + 0, layer,   0b00010000);
        total += east;
        var south = GetNeighborValue(tile_start, tile_end, x + 0, y + 1, layer,  0b01000000);
        total += south;

        total += GetNeighborValue(tile_start, tile_end, x + -1, y + -1, layer,   0b00000001);
        total += GetNeighborValue(tile_start, tile_end, x + 1, y + -1, layer,    0b00000100);
        total += GetNeighborValue(tile_start, tile_end, x + -1, y + 1, layer,    0b00100000);
        total += GetNeighborValue(tile_start, tile_end, x + 1, y + 1, layer,     0b10000000);

        return total;
    }

    protected int GetNeighborValue(int tile_start, int tile_end, int x, int y, ITileLayer layer, int neighborValue)
    {
        var tile = layer.GetTileIndex(x, y);
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height || (tile >= tile_start && tile <= tile_end))
            return 0;
        else
            return neighborValue;
    }

    public bool CanHarvest(int x, int y)
        => HarvestDetails(x, y).harvestType != HarvestType.None;

    public int GetHarvestDelay(int x, int y)
        => HarvestDetails(x, y).harvestDelay;

    private static ItemTile[] Empty = [];
    private static ItemTile[] SingleWood = [ItemTile.Wood];
    private static ItemTile[] DoubleWood = [ItemTile.Wood, ItemTile.Wood];
    private static ItemTile[] SingleMushroom = [ItemTile.Mushroom];

    private (HarvestType harvestType, int harvestDelay, int harvestStartX, int harvestStartY, int harvestWidth, int harvestHeight, ItemTile[] item, ResourceTile replacementTile) HarvestDetails(int x, int y)
        => ResourceLayer.GetTileValue(x, y) switch {
            //ResourceTile.Stump1_Start => (HarvestType.RemoveStump, 50, x, y, 2, 2, SingleWood, ResourceTile.Empty),
            //ResourceTile.Stump1_Start + 1 => (HarvestType.RemoveStump, 50, x - 1, y, 2, 2, SingleWood, ResourceTile.Empty),
            //ResourceTile.Stump1_Start + 2 => (HarvestType.RemoveStump, 50, x, y - 1, 2, 2, SingleWood, ResourceTile.Empty),
            //ResourceTile.Stump1_Start + 3 => (HarvestType.RemoveStump, 50, x - 1, y - 1, 2, 2, SingleWood, ResourceTile.Empty),

            >= ResourceTile.Mushroom_Start and <= ResourceTile.Mushroom_End => (HarvestType.ForageForFood, 10, x, y, 1, 1, SingleMushroom, ResourceTile.Empty),

            ResourceTile.Tree1_Start => (HarvestType.CutTree, 20, x, y, 2, 2, SingleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree1_Start + 1 => (HarvestType.CutTree, 20, x-1, y, 2, 2, SingleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree1_Start + 2 => (HarvestType.CutTree, 20, x, y-1, 2, 2, SingleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree1_Start + 3 => (HarvestType.CutTree, 20, x-1, y-1, 2, 2, SingleWood, ResourceTile.Stump1_Start),

            ResourceTile.Tree2_Start => (HarvestType.CutTree, 25, x, y, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree2_Start + 1 => (HarvestType.CutTree, 25, x - 1, y, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree2_Start + 2 => (HarvestType.CutTree, 25, x, y - 1, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree2_Start + 3 => (HarvestType.CutTree, 25, x - 1, y - 1, 2, 2, DoubleWood, ResourceTile.Stump1_Start),

            ResourceTile.Tree3_Start => (HarvestType.CutTree, 25, x, y, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree3_Start + 1 => (HarvestType.CutTree, 25, x - 1, y, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree3_Start + 2 => (HarvestType.CutTree, 25, x, y - 1, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree3_Start + 3 => (HarvestType.CutTree, 25, x - 1, y - 1, 2, 2, DoubleWood, ResourceTile.Stump1_Start),

            ResourceTile.Tree4_Start => (HarvestType.CutTree, 25, x, y, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree4_Start + 1 => (HarvestType.CutTree, 25, x - 1, y, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree4_Start + 2 => (HarvestType.CutTree, 25, x, y - 1, 2, 2, DoubleWood, ResourceTile.Stump1_Start),
            ResourceTile.Tree4_Start + 3 => (HarvestType.CutTree, 25, x - 1, y - 1, 2, 2, DoubleWood, ResourceTile.Stump1_Start),

            var r => (HarvestType.None, 0, 0, 0, 0, 0, Empty, r),
        };

    public void HarvestTile(int x, int y, Unit harvester)
    {
        var (harvestType, _harvestDelay, harvestStartX, harvestStartY, harvestWidth, harvestHeight, items, replacementTile) = HarvestDetails(x, y);
        if (harvestType == HarvestType.None)
            return;

        for (var j = 0; j < harvestHeight; j++)
            for (var i = 0; i < harvestWidth; i++)
                ResourceLayer.SetTileValue(harvestStartX + i, harvestStartY + j, replacementTile++);

        var currentSpiral = Point.Zero;
        foreach (var item in items)
        {
            var spiral = Extensions.SpiralAround(harvester.Cell.X, harvester.Cell.Y).GetEnumerator();
            while (IsBlockedAt("resource", x + currentSpiral.X, y + currentSpiral.Y) > 0
                   || IsBlockedAt("item", x + currentSpiral.X, y + currentSpiral.Y) > 0)
            {
                spiral.MoveNext();
                currentSpiral = spiral.Current;
            };
            ItemLayer.SetTileValue(x + currentSpiral.X, y + currentSpiral.Y, item);
        }
    }
}
