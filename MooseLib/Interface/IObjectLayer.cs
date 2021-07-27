using MooseLib.GameObjects;
using System.Collections.Generic;

namespace MooseLib.Interface
{
    public interface IObjectLayer : ILayer
    {
        IEnumerable<GameObjectBase> ObjectsAt(int x, int y, IMap map);
    }
}
