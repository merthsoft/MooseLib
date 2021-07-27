using System.Collections.Generic;

namespace MooseLib.Interface
{
    public interface IMap
    {
        int Height { get; }
        int Width { get; }
        int TileWidth { get; }
        int TileHeight { get; }

        IReadOnlyList<ILayer> Layers { get; }

        void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null);
    }
}
