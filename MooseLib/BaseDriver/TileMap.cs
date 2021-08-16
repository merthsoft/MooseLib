using Merthsoft.MooseEngine.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public abstract class TileMap : BaseMap
    {
        public override int Height { get; }
        public override int Width { get; }
        public override int TileWidth { get; }
        public override int TileHeight { get; }

        private ReadOnlyCollection<ITileLayer> layersCache = null!; // Set in BuildLayers called by ctor
        public override IReadOnlyList<ILayer> Layers => layersCache;
        public override IEnumerable<int> ObjectLayerIndices => Enumerable.Empty<int>();
        public override IEnumerable<int> TileLayerIndices => Enumerable.Range(0, Layers.Count);

        private readonly List<ITileLayer> layers = new();

        public TileMap(int width, int height, int numLayers, int tileWidth, int tileHeight)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            BuildLayers(numLayers);
        }

        private void BuildLayers(int numLayers)
        {
            for (int layerIndex = 0; layerIndex < numLayers; layerIndex++)
                layers.Add(new TileLayer<int>($"Layer {layerIndex}", layerIndex, Width, Height));
            layersCache = layers.AsReadOnly();
        }
    }
}
