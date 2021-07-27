using MonoGame.Extended.Tiled;
using MooseLib.GameObjects;
using MooseLib.Interface;
using System.Collections.Generic;

namespace MooseLib.Tiled
{
    public record TiledMooseObjectLayer(TiledMapObjectLayer Layer) : IObjectLayer
    {
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

        public IEnumerable<GameObjectBase> ObjectsAt(int x, int y, IMap map)
        {
            yield break;
        }
    }
}
