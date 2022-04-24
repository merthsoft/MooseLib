using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public class ObjectLayer<TObject> : IObjectLayer where TObject : GameObjectBase
{
    public string? RendererKey { get; set; }

    readonly SortedSet<TObject> objects = new();
    public IEnumerable<TObject> Objects => objects;

    public string Name { get; }
    public bool IsHidden { get; set; } = false;
    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawSize { get; set; }
    public Color DrawColor { get; set; } = Color.White;
    IEnumerable<GameObjectBase> IObjectLayer.Objects => objects;

    public ObjectLayer(string name)
        => Name
         = name;

    public void AddObject(TObject obj)
        => objects.Add(obj);

    public void RemoveObject(TObject obj)
        => objects.Remove(obj);
    public void AddObject(GameObjectBase obj) => AddObject((TObject)obj);
    public void RemoveObject(GameObjectBase obj) => RemoveObject((TObject)obj);
}
