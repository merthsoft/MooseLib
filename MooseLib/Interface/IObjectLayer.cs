using Merthsoft.MooseEngine.GameObjects;
using System.Collections.Generic;

namespace Merthsoft.MooseEngine.Interface
{
    public interface IObjectLayer : ILayer
    {
        IReadOnlyList<GameObjectBase> Objects { get; }
        void AddObject(GameObjectBase obj);
        void RemoveObject(GameObjectBase obj);

        IEnumerator<GameObjectBase> GetEnumerator() => Objects.GetEnumerator();
    }
}
