using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.MooseEngine.Interface;

public interface IObjectLayer : ILayer {
    IEnumerable<GameObjectBase> Objects { get; }
    void AddObject(GameObjectBase obj);
    void RemoveObject(GameObjectBase obj);

    IEnumerator<GameObjectBase> GetEnumerator() => Objects.GetEnumerator();

    void ObjectUpdate(GameObjectBase obj);

    IEnumerable<GameObjectBase> GetObjects(int x, int y);
}
