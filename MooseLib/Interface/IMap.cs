using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace MooseLib.Interface
{
    public interface IMap
    {
        int Height { get; }
        int Width { get; }
        int TileWidth { get; }
        int TileHeight { get; }

        public Size2 TileSize => new(TileWidth, TileHeight);
        public Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2);

        IReadOnlyList<ILayer> Layers { get; }

        IEnumerable<int> ObjectLayerIndices { get; }
        IEnumerable<int> TileLayerIndices { get; }

        void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null);

        void Update(GameTime gameTime);
        IEnumerable<int> GetBlockingMap(int cellX, int cellY);
        IEnumerable<int> GetBlockingMap(Vector2 worldPosition)
            => GetBlockingMap((int)(worldPosition.X / TileWidth), (int)(worldPosition.Y / TileHeight));
    }
}
