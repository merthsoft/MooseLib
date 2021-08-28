using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class SpriteBatchPrimitiveRectangleRenderer : SpriteBatchObjectRenderer
    {
        public new static string DefaultRenderKey = "SpriteBatchPrimitiveRectangleRenderer_Default";

        private List<Color> Palette { get; } = new();

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public SpriteBatchPrimitiveRectangleRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, params Color[] colors) : base(spriteBatch)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Palette.AddRange(colors);
        }

        public override void Draw(GameTime _, ILayer layer, int layerNumber)
        {
            if (layer is not TileLayer<int> tileLayer)
                throw new Exception("TileLayer<int> layer expected");

            for (int i = 0; i < tileLayer.Width; i++)
                for (int j = 0; j < tileLayer.Height; j++)
                    SpriteBatch.FillRectangle(
                        i * TileWidth, j * TileHeight, 
                        TileWidth, TileHeight, 
                        Palette[tileLayer.Tiles[i, j]]);
        }
    }
}
