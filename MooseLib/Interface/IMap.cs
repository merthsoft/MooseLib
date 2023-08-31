﻿using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.MooseEngine.Interface;
public interface IMap
{
    int Height { get; }
    int Width { get; }
    int TileWidth { get; }
    int TileHeight { get; }

    public Size2 TileSize => new(TileWidth, TileHeight);
    public Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2);

    IReadOnlyList<ILayer> Layers { get; }
    IReadOnlyDictionary<string, ILayer> LayerMap { get; }

    void Update(MooseGame game, GameTime gameTime);

    public bool CellIsInBounds(Point cell);

    TLayer GetLayer<TLayer>(int layerNumber) where TLayer : ILayer;
    TLayer GetLayer<TLayer>(string layerName) where TLayer : ILayer;
    void MoveObject(GameObjectBase gameObject, Vector2 toLocation) { }
}
