﻿using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchTileTextureRenderer : SpriteBatchAbstractTileRenderer
{

    public SpriteBatchTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites, int textureMargin = 0, int tilePadding = 0) 
        : base(spriteBatch, tileWidth, tileHeight, sprites, textureMargin, tilePadding)
    {
        
    }


    public override void Draw(MooseGame game, GameTime _gameTime, ILayer layer, Vector2 drawOffset)
    {
        if (layer is not ITileLayer tileLayer)
            throw new Exception("TileLayer layer expected");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
            {
                var tileValue = tileLayer.GetTileIndex(i, j);
                if (tileValue >= 0)
                    DrawSprite(tileValue, i, j, tileLayer, drawOffset);
            }
    }
}