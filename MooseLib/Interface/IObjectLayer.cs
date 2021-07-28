using MooseLib.GameObjects;
using System.Collections.Generic;

namespace MooseLib.Interface
{
    public interface IObjectLayer : ILayer
    {
        IReadOnlyList<GameObjectBase> Objects { get; }
        void AddObject(GameObjectBase obj);
        void RemoveObject(GameObjectBase obj);

        IEnumerator<GameObjectBase> GetEnumerator() => Objects.GetEnumerator();
    }
}
