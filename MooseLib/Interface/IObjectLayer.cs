using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.MooseEngine.Interface
{
    public interface IObjectLayer : ILayer
    {
        IReadOnlyList<GameObjectBase> Objects { get; }
        void AddObject(GameObjectBase obj);
        void RemoveObject(GameObjectBase obj);

        IEnumerator<GameObjectBase> GetEnumerator() => Objects.GetEnumerator();
    }
}
