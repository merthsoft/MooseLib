using Merthsoft.MooseEngine.GameObjects;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class ObjectLayer : IObjectLayer
    {
        readonly SortedSet<GameObjectBase> objects = new();
        public IReadOnlyList<GameObjectBase> Objects => objects.ToList().AsReadOnly();

        public string Name { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public string RendererKey { get; set; }
        public int Number { get; }

        public ObjectLayer(string name, string rendererKey, int number)
            => (Name, RendererKey, Number)
             = (name, rendererKey, number);

        public void AddObject(GameObjectBase obj)
            => objects.Add(obj);

        public void RemoveObject(GameObjectBase obj)
            => objects.Remove(obj);
    }
}
