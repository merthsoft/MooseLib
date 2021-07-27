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

        public void Draw(ITileLayer layer, Matrix? viewMatrix = null)
            => MapRenderer.Draw((layer as TiledMooseTileLayer)?.Layer, viewMatrix);

        public void Draw(int layerIndex, Matrix? viewMatrix = null)
            => MapRenderer.Draw(layerIndex, viewMatrix);

        public void LoadMap(IMap map)
            => MapRenderer.LoadMap((map as TiledMooseMap)?.Map);

        public void Update(GameTime gameTime)
            => MapRenderer.Update(gameTime);
    }
}
