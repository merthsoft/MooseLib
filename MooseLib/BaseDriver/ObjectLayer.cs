using Merthsoft.MooseEngine.BaseDriver.Renderers;
using Merthsoft.MooseEngine.GameObjects;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class ObjectLayer : IObjectLayer
    {
        readonly SortedSet<GameObjectBase> objects = new();
        public IReadOnlyList<GameObjectBase> Objects => objects.ToList().AsReadOnly();

        public string Name { get; }
        public string RendererKey { get; set; } = DefaultRenderKeys.SpriteBatchObjectRenderer;
        public bool IsVisible { get; set; }

        public ObjectLayer(string name)
            => Name
             = name;

        public void AddObject(GameObjectBase obj)
            => objects.Add(obj);

        public void RemoveObject(GameObjectBase obj)
            => objects.Remove(obj);
    }
}
