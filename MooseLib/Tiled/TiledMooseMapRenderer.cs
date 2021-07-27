using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MooseLib.Interface;
using System;

namespace MooseLib.Tiled
{
    public record TiledMooseMapRenderer : IMapRenderer
    {
        private TiledMapRenderer MapRenderer { get; }

        public TiledMooseMapRenderer(GraphicsDevice graphicsDevice)
            => MapRenderer = new(graphicsDevice);

        private static TiledMapLayer GetLayer(ILayer layer)
            => ((layer as TiledMooseTileLayer)?.Layer as TiledMapLayer)
            ?? ((layer as TiledMooseObjectLayer)?.Layer as TiledMapLayer)
            ?? throw new Exception("Unsupported layer typed.");

        public void Draw(ILayer layer, Matrix? viewMatrix = null)
            => MapRenderer.Draw(GetLayer(layer), viewMatrix);

        public void Draw(int layerIndex, Matrix? viewMatrix = null)
            => MapRenderer.Draw(layerIndex, viewMatrix);

        public void LoadMap(IMap map)
            => MapRenderer.LoadMap((map as TiledMooseMap)?.Map);

        public void Update(GameTime gameTime)
            => MapRenderer.Update(gameTime);
    }
}
