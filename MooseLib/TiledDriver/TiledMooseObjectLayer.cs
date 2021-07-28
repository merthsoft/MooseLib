using MonoGame.Extended.Tiled;
using MooseLib.BaseDriver;
using MooseLib.GameObjects;
using MooseLib.Interface;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib.Tiled
{
    public record TiledMooseObjectLayer(TiledMapObjectLayer Layer) : IObjectLayer
    {
        readonly SortedSet<GameObjectBase> objects = new();
        public IReadOnlyList<GameObjectBase> Objects => objects.ToList().AsReadOnly();

        public string Name => Layer.Name;
        
        public bool IsVisible
        {
            get => Layer.IsVisible;
            set => Layer.IsVisible = value;
        }

        public float Opacity
        {
            get => Layer.Opacity;
            set => Layer.Opacity = value;
        }

        public string RendererKey { get; set; } = SpriteBatchRenderer.DefaultRenderKey;

        public void AddObject(GameObjectBase obj)
            => objects.Add(obj);

        public void RemoveObject(GameObjectBase obj)
            => objects.Remove(obj);
    }
}
