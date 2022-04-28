﻿using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public class ObjectLayer<TObject> : IObjectLayer where TObject : GameObjectBase
{
    public string? RendererKey { get; set; }

    readonly SortedSet<TObject> objects = new();
    public IEnumerable<TObject> Objects => objects;

    public string Name { get; }
    public int Width { get; }
    public int Height { get; }
    public bool IsHidden { get; set; } = false;
    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawSize { get; set; }
    public Color DrawColor { get; set; } = Color.White;
    IEnumerable<GameObjectBase> IObjectLayer.Objects => objects;

    public List<TObject>[,] ObjectMap;

    public ObjectLayer(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;

        ObjectMap = new List<TObject>[width, height];
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                ObjectMap[x, y] = new();
    }

    public void AddObject(TObject obj)
        => objects.Add(obj);

    public void RemoveObject(TObject obj)
        => objects.Remove(obj);
    public void AddObject(GameObjectBase obj) => AddObject((TObject)obj);
    public void RemoveObject(GameObjectBase obj) => RemoveObject((TObject)obj);

    public void Update(GameTime gameTime)
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                ObjectMap[x, y].Clear();
    }

    public void ObjectUpdate(GameObjectBase obj)
    {
        var cell = obj.GetCell();
        var (x, y) = cell;
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            ObjectMap[x, y].Add((obj as TObject)!);
    }
    
    public IEnumerable<TObject> GetObjects(int x, int y)
        => x < 0 || x >= Width || y < 0 || y >= Height ? Enumerable.Empty<TObject>() : ObjectMap[x, y];
    
    IEnumerable<GameObjectBase> IObjectLayer.GetObjects(int x, int y) 
        => x < 0 || x >= Width || y < 0 || y >= Height ? Enumerable.Empty<GameObjectBase>() : ObjectMap[x, y];
}
