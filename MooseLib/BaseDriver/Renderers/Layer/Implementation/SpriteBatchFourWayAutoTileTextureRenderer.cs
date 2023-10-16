using Merthsoft.Moose.MooseEngine.Interface;
using System.Runtime.CompilerServices;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;

public class SpriteBatchFourWayAutoTileTextureRenderer : SpriteBatchTileTextureRenderer
{
    public Dictionary<int, Texture2D> AutoTileTextureMap { get; } = new();

    public Texture2D this[int index]
    {
        get => AutoTileTextureMap[index];
        set => AutoTileTextureMap[index] = value;
    }

    public SpriteBatchFourWayAutoTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0)
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual int GetTileIndex(int tile, int neighborCount)
        => neighborCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual int CountNeighbors(int tile, int x, int y, ITileLayer layer)
        => GetNeighborValue(tile, x + 0, y + -1, layer, 1)
         + GetNeighborValue(tile, x + -1, y + 0, layer, 2)
         + GetNeighborValue(tile, x + 1, y + 0, layer, 4)
         + GetNeighborValue(tile, x + 0, y + 1, layer, 8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual int GetNeighborValue(int tile, int x, int y, ITileLayer layer, int neighborValue)
        => x < 0 || y < 0 || x >= layer.Width || y >= layer.Height || tile == layer.GetTileIndex(x, y) ? 0 : neighborValue;

    public override void DrawSprite(int spriteIndex, int i, int j, ILayer layer, Vector2 drawOffset, float layerDepth = 1)
    {
        var texture = AutoTileTextureMap.GetValueOrDefault(spriteIndex);

        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset + drawOffset);
        if (destRect == null)
            return;

        if (texture == null)
        {
            SpriteBatch.Draw(SpriteSheet,
                position: destRect.Value.Position,
                sourceRectangle: GetSourceRectangle(spriteIndex),
                color: layer.DrawColor, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, scale: DrawScale, layerDepth: layerDepth);
            return;
        }

        var neighborCount = CountNeighbors(spriteIndex, i, j, (layer as ITileLayer)!);
        var tileIndex = GetTileIndex(spriteIndex, neighborCount);
        SpriteBatch.Draw(texture,
                position: destRect.Value.Position, scale: DrawScale,
                sourceRectangle: GetSourceRectangle(tileIndex, texture),
                color: layer.DrawColor, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);
    }
}
