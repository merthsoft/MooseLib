using Merthsoft.Moose.MooseEngine.Interface;
using System.Collections.ObjectModel;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public class MultiMap<TTile> : BaseMap
{
    public enum LayerType { Tile, Object };

    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; }
    public override int TileHeight { get; }

    private readonly ReadOnlyCollection<ILayer> layers;
    public override IReadOnlyList<ILayer> Layers => layers;

    public MultiMap(int width, int height, int tileWidth, int tileHeight, params ILayer[] layers)
    {
        Width = width;
        Height = height;
        TileWidth = tileWidth;
        TileHeight = tileHeight;

        this.layers = new(layers);
    }

    public MultiMap(int width, int height, int tileWidth, int tileHeight, params MultiMap<TTile>.LayerType[] layerTypes)
    {
        Width = width;
        Height = height;
        TileWidth = tileWidth;
        TileHeight = tileHeight;

        var layers = new List<ILayer>();
        var objectLayerIndices = new List<int>();
        var tileLayerIndices = new List<int>();

        TileLayer<TTile> createTileLayer(int layerIndex)
        {
            tileLayerIndices.Add(layerIndex);
            return new TileLayer<TTile>($"Layer {layerIndex}", Width, Height);
        }

        ObjectLayer createObjectLayer(int layerIndex)
        {
            objectLayerIndices.Add(layerIndex);
            return new ObjectLayer($"Layer {layerIndex}");
        }

        for (var layerIndex = 0; layerIndex < layerTypes.Length; layerIndex++)
            layers.Add(layerTypes[layerIndex] switch
            {
                MultiMap<TTile>.LayerType.Tile => createTileLayer(layerIndex),
                MultiMap<TTile>.LayerType.Object => createObjectLayer(layerIndex),
                _ => throw new ArgumentOutOfRangeException($"layerTypes[{layerIndex}]", layerTypes[layerIndex], $"{layerTypes[layerIndex]} is not a valid layer type")
            });
        this.layers = layers.AsReadOnly();
    }

    public void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
    {
        width ??= Width;
        height ??= Height;

        if (sourceX < 0 || sourceY < 0 || destX < 0 || destY < 0
            || width <= 0 || height <= 0
            || sourceX > sourceMap.Width || sourceY > sourceMap.Height
            || destX > sourceMap.Height || destY > sourceMap.Height
            || sourceX + width > sourceMap.Width || sourceY + height > sourceMap.Height
            || destX + width > sourceMap.Height || destY + height > sourceMap.Height)
            throw new ArgumentOutOfRangeException("Not in bounds of one of the maps.", innerException: null);

        if (sourceMap is not MultiMap<TTile> sourceTileMap)
            throw new ArgumentOutOfRangeException(nameof(sourceMap), "sourceMap must be MultiMap");

        var sourceLayers = sourceTileMap.Layers;

        for (var layerIndex = 0; layerIndex < Layers.Count; layerIndex++)
        {
            if (layerIndex >= sourceLayers.Count)
                break;

            var layer = layers[layerIndex];

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    if (layer is TileLayer<TTile> tileLayer)
                        tileLayer.SetTile(x + destX, y + destY,
                            (sourceLayers[layerIndex] as TileLayer<TTile>)!.GetTileValue(x + sourceX, y + sourceY));
        }
    }

    protected override int IsBlockedAt(int layer, int x, int y)
        => 0;

    public void SetTile(int layer, int x, int y, TTile value)
    {
        if (layers[layer] is not TileLayer<TTile> tileLayer)
            return;
        tileLayer.SetTile(x, y, value);
    }

    public override void Update(GameTime gameTime)
        => base.Update(gameTime);
}
