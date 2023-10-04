using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.Noise;
using Merthsoft.Moose.MooseEngine.PathFinding.Maps;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.Moose.Rts;
internal enum TerrainTile
{
    Water = 0,
    WaterEdge_Start,
    WaterEdge_End = WaterEdge_Start + 11,
    WaterDecoration_Start,
    WaterDecoration_End,
    Land,
    Grass_Start,
    Grass_End = Grass_Start + 6,
    Flower_Start,
    Flower_End = Flower_Start + 11
}

internal class RtsMap : PathFinderMap
{
    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; } = 8;
    public override int TileHeight { get; } = 8;

    public TileLayer<TerrainTile> TerrainLayer { get; }
    public TileLayer ResourceLayer { get; }
    public TileLayer ItemLayer { get; } = null!;
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
        ResourceLayer = AddLayer(new TileLayer("resource", Width, Height, -1) { RendererKey = "resource" });
        //ItemLayer = AddLayer(new TileLayer<int>("item", Width, Height, -1));
        UnitLayer = AddLayer(new ObjectLayer<Unit>("units", width, height));

        RandomizeMap();
    }

    public IEnumerable<Unit> GetUnits()
        => UnitLayer.Objects;

    public override int IsBlockedAt(string layer, int x, int y)
        => layer switch
        {
            "terrain" => TerrainLayer.GetTileValue(x, y) < TerrainTile.Land ? 1 : 0,
            "resource" => ResourceLayer.GetTileValue(x, y) >= 5 ? 1 : 0,
            "item" => ItemLayer.GetTileValue(x, y),
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
        Flower
    }

    private static void SetLayerSquare(TileLayer tileLayer, int x, int y, int? value = null, Func<int, int>? valueGenerator = null)
    {
        tileLayer.SetTileValue(x * 2, y * 2, value ?? valueGenerator!(0));
        tileLayer.SetTileValue(x * 2 + 1, y * 2, value ?? valueGenerator!(1));
        tileLayer.SetTileValue(x * 2, y * 2 + 1, value ?? valueGenerator!(2));
        tileLayer.SetTileValue(x * 2 + 1, y * 2 + 1, value ?? valueGenerator!(3));
    }

    private void SetTerrainLayerSquare(int x, int y, TerrainTile? value = null, Func<int, TerrainTile>? valueGenerator = null)
    {
        TerrainLayer.SetTileValue(x * 2, y * 2, value ?? valueGenerator!(0));
        TerrainLayer.SetTileValue(x * 2 + 1, y * 2, value ?? valueGenerator!(1));
        TerrainLayer.SetTileValue(x * 2, y * 2 + 1, value ?? valueGenerator!(2));
        TerrainLayer.SetTileValue(x * 2 + 1, y * 2 + 1, value ?? valueGenerator!(3));
    }


    public void RandomizeMap()
    {
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
                    ( < 20, _, _, _) => GeneratedType.WaterDecoration,
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
                SetLayerSquare(ResourceLayer, x, y, 0);

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
                        SetLayerSquare(ResourceLayer, x, y, valueGenerator: (i) => i + 5);
                        break;
                    case GeneratedType.Stump:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomGrass);
                        SetLayerSquare(ResourceLayer, x, y, valueGenerator: (i) => i + 9);
                        break;
                    case GeneratedType.Flower:
                        SetTerrainLayerSquare(x, y, valueGenerator: randomFlower);
                        break;
                    case GeneratedType.Mushroom:
                        SetTerrainLayerSquare(x, y, TerrainTile.Grass_Start);
                        SetLayerSquare(ResourceLayer, x, y, valueGenerator: (_) => MooseGame.Random.Next(1, 4));
                        break;
                    case GeneratedType.Land:
                    default:
                        break;
                }
                
            }

        // Convolusion on final map
        // Auto-tile the water, add some other random elements
        for (var x = 0; x < Width; x += 1)
            for (var y = 0; y < Height; y += 1)
            {
                var coinFlip = MooseGame.Random.NextDouble();
                var terrainTile = TerrainLayer.GetTileValue(x, y);
                if (terrainTile == TerrainTile.Water)
                {
                    var neighbors = CountNeighbors((int)TerrainTile.Water, (int)TerrainTile.WaterDecoration_End, x, y, TerrainLayer);
                    if (neighbors != 0)
                        terrainTile = (TerrainTile)((int)terrainTile + indexMap.GetValueOrDefault(neighbors));
                    else if (coinFlip < .05)
                    {
                        terrainTile = randomWaterDecoration();
                    }
                    
                    if (terrainTile != TerrainTile.Water)
                        TerrainLayer.SetTileValue(x, y, terrainTile);
                } else if (terrainTile == TerrainTile.Land && coinFlip < 0.05)
                {
                    TerrainLayer.SetTileValue(x, y, randomTerrainDecoration());
                }
            }

        IsBlockingMapDirty = true;

        TerrainTile randomGrass(int _)
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.Grass_Start, (int)TerrainTile.Grass_End + 1);

        TerrainTile randomFlower(int _)
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.Flower_Start, (int)TerrainTile.Flower_End + 1);

        TerrainTile randomTerrainDecoration()
            => (TerrainTile)MooseGame.Random.Next((int)TerrainTile.Grass_Start, (int)TerrainTile.Flower_End + 1);

        TerrainTile randomWaterDecoration()
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
        var west = GetNeighborValue(tile_start, tile_end, x + -1, y + 0, layer, 0b00001000);
        total += west;
        var east = GetNeighborValue(tile_start, tile_end, x + 1, y + 0, layer, 0b00010000);
        total += east;
        var south = GetNeighborValue(tile_start, tile_end, x + 0, y + 1, layer, 0b01000000);
        total += south;

        total += GetNeighborValue(tile_start, tile_end, x + -1, y + -1, layer, 0b00000001);
        total += GetNeighborValue(tile_start, tile_end, x + 1, y + -1, layer, 0b00000100);
        total += GetNeighborValue(tile_start, tile_end, x + -1, y + 1, layer, 0b00100000);
        total += GetNeighborValue(tile_start, tile_end, x + 1, y + 1, layer, 0b10000000);

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
}
