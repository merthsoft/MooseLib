using Merthsoft.MooseEngine.BaseDriver;
using Merthsoft.MooseEngine.GameObjects;
using Merthsoft.MooseEngine.Interface;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.MooseEngine.TiledDriver
{
    public record TiledMooseObjectLayer(int Number, TiledMapObjectLayer Layer) : IObjectLayer
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

        public string RendererKey { get; set; } = SpriteBatchObjectRenderer.DefaultRenderKey;

        public void AddObject(GameObjectBase obj)
            => objects.Add(obj);

        public void RemoveObject(GameObjectBase obj)
            => objects.Remove(obj);
    }
}
