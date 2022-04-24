using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.GameObjects;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MooseEngine.TiledDriver;

public class TiledMooseObjectLayer : ObjectLayer<GameObjectBase>
{
    public TiledMapObjectLayer Layer { get; private set; }

    public TiledMooseObjectLayer(TiledMapObjectLayer layer)
        : base(layer.Name)
        => Layer = layer;
}
