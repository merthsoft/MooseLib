using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public class ObjectLayer : IObjectLayer
{
    public string? RendererKey { get; set; }

    readonly SortedSet<GameObjectBase> objects = new();
    public IEnumerable<GameObjectBase> Objects => objects;

    public string Name { get; }
    public bool IsHidden { get; set; } = false;
    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawSize { get; set; }

    public ObjectLayer(string name)
        => Name
         = name;

    public void AddObject(GameObjectBase obj)
        => objects.Add(obj);

    public void RemoveObject(GameObjectBase obj)
        => objects.Remove(obj);
}
