using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class MultiMap<TTile> : BaseMap
    {
        public enum LayerType { Tile, Object };

        public override int Height { get; }
        public override int Width { get; }
        public override int TileWidth { get; }
        public override int TileHeight { get; }

        private ReadOnlyCollection<ILayer> layers;
        public override IReadOnlyList<ILayer> Layers => layers;
        
        public override IEnumerable<int> ObjectLayerIndices { get; }
        
        public override IEnumerable<int> TileLayerIndices { get; }

        public string DefaultTileLayerRendererKey { get; set; }
        public string DefaultObjectLayerRendererKey { get; set; }

        public MultiMap(int width, int height, int tileWidth, int tileHeight, string tileLayerRenderKey, string objectLayerRenderKey, params LayerType[] layerTypes)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            DefaultTileLayerRendererKey = tileLayerRenderKey;
            DefaultObjectLayerRendererKey = objectLayerRenderKey;

            var layers = new List<ILayer>();
            var objectLayerIndices = new List<int>();
            var tileLayerIndices = new List<int>();

            TileLayer<TTile> createTileLayer(int layerIndex)
            {
                tileLayerIndices.Add(layerIndex);
                return new TileLayer<TTile>($"Layer {layerIndex}", Width, Height) { RendererKey = tileLayerRenderKey };
            }

            ObjectLayer createObjectLayer(int layerIndex)
            {
                objectLayerIndices.Add(layerIndex);
                return new ObjectLayer($"Layer {layerIndex}");
            }

            for (var layerIndex = 0; layerIndex < layerTypes.Length; layerIndex++)
                layers.Add(layerTypes[layerIndex] switch
                {
                    LayerType.Tile => createTileLayer(layerIndex),
                    LayerType.Object => createObjectLayer(layerIndex),
                    _ => throw new ArgumentOutOfRangeException($"layerTypes[{layerIndex}]", $"{layerTypes[layerIndex]} is not a valid layer type")
                });
            this.layers = layers.AsReadOnly();
            ObjectLayerIndices = objectLayerIndices.AsReadOnly();
            TileLayerIndices = tileLayerIndices.AsReadOnly();
        }

        public override void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
        {
            throw new NotImplementedException();
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
}
