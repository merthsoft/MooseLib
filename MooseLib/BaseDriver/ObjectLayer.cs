using MooseLib.GameObjects;
using MooseLib.Interface;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib.BaseDriver
{
    public class ObjectLayer : IObjectLayer
    {
        readonly SortedSet<GameObjectBase> objects = new();
        public IReadOnlyList<GameObjectBase> Objects => objects.ToList().AsReadOnly();

        public string Name { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public string RendererKey { get; set; }

        public ObjectLayer(string name, string rendererKey)
            => (Name, RendererKey)
             = (name, rendererKey);

        public void AddObject(GameObjectBase obj)
            => objects.Add(obj);

        public void RemoveObject(GameObjectBase obj)
            => objects.Remove(obj);
    }
}
