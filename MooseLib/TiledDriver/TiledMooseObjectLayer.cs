using Merthsoft.MooseEngine.BaseDriver;
using MonoGame.Extended.Tiled;

namespace Merthsoft.MooseEngine.TiledDriver
{
    public class TiledMooseObjectLayer : ObjectLayer
    {
        public TiledMapObjectLayer Layer { get; private set; }
        
        public TiledMooseObjectLayer(TiledMapObjectLayer layer) 
            : base(layer.Name)
            => Layer = layer;
    }
}
