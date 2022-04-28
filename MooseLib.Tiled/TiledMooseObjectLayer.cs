using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.GameObjects;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MooseEngine.Tiled;

public class TiledMooseObjectLayer : ObjectLayer<GameObjectBase>
{
    public TiledMapObjectLayer Layer { get; private set; }

    public TiledMooseObjectLayer(TiledMapObjectLayer layer, int width, int height)
        : base(layer.Name, width, height)
        => Layer = layer;
}
