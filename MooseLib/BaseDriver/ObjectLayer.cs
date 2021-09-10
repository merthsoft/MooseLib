﻿using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.MooseEngine.BaseDriver
{
    public class ObjectLayer : IObjectLayer
    {
        readonly SortedSet<GameObjectBase> objects = new();
        public IReadOnlyList<GameObjectBase> Objects => objects.ToList().AsReadOnly();

        public string Name { get; }
        public bool IsHidden { get; set; }
        public Vector2 DrawOffset { get; set; }

        public ObjectLayer(string name)
            => Name
             = name;

        public void AddObject(GameObjectBase obj)
            => objects.Add(obj);

        public void RemoveObject(GameObjectBase obj)
            => objects.Remove(obj);
    }
}
