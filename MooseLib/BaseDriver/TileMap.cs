using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class TileMap<TTile> : BaseMap
    {
        public override int Height { get; }
        public override int Width { get; }
        public override int TileWidth { get; }
        public override int TileHeight { get; }

        private ReadOnlyCollection<TileLayer<TTile>> layersCache;
        public override IReadOnlyList<ILayer> Layers => layersCache;

        public override IEnumerable<int> ObjectLayerIndices => Enumerable.Empty<int>();
        public override IEnumerable<int> TileLayerIndices => Enumerable.Range(0, Layers.Count);

        private readonly List<TileLayer<TTile>> layers = new();

        public string DefaultTileMapRendererKey { get; set; }

        public TileMap(int width, int height, int numLayers, int tileWidth, int tileHeight, string defaultTileMapRenderKey)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            DefaultTileMapRendererKey = defaultTileMapRenderKey;

            for (int layerIndex = 0; layerIndex < numLayers; layerIndex++)
                layers.Add(new TileLayer<TTile>($"Layer {layerIndex}", Width, Height) { RendererKey = DefaultTileMapRendererKey });
            layersCache = layers.AsReadOnly();
        }

        protected override int IsBlockedAt(int layer, int x, int y) 
            => 0;

        public override void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
        {
            width = width ?? Width;
            height = height ?? Height;

            if (sourceX < 0 || sourceY < 0 || destX < 0 || destY < 0
                || width <= 0 || height <= 0
                || sourceX > sourceMap.Width || sourceY > sourceMap.Height
                || destX > sourceMap.Height || destY > sourceMap.Height
                || sourceX + width > sourceMap.Width || sourceY + height > sourceMap.Height
                || destX + width > sourceMap.Height || destY + height > sourceMap.Height)
                throw new ArgumentOutOfRangeException();

            if (sourceMap is not TileMap<TTile> sourceTileMap)
                throw new ArgumentOutOfRangeException(nameof(sourceMap), "sourceMap must be or derive from TileMap");

            var sourceLayers = sourceTileMap.Layers;

            for (var layer = 0; layer < Layers.Count; layer++)
            {
                if (layer >= sourceLayers.Count)
                    break;

                for (var x = 0; x < width; x++)
                    for (var y = 0; y < height; y++)
                        layers[layer].SetTile(x + destX, y + destY,
                            (sourceLayers[layer] as TileLayer<TTile>)!.GetTileValue(x + sourceX, y + sourceY));
            }
        }

        public void SetTile(int layer, int x, int y, TTile value)
            => layers[layer].SetTile(x, y, value);

        public override void Update(GameTime gameTime)
            => base.Update(gameTime);
    }
}
