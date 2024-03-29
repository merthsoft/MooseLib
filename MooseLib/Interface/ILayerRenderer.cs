﻿namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ILayerRenderer
{
    Vector2 DrawOffset { get; set; }
    Vector2 DrawScale { get; set; }

    void LoadContent(MooseContentManager contentManager);

    void Update(MooseGame game, GameTime gameTime, ILayer layer) { }

    bool PreDraw(MooseGame game, GameTime gameTime, ILayer layer);

    void Begin(Matrix transformMatrix) { }
    void Draw(MooseGame game, GameTime gameTime, ILayer layer);
    void End() { }
}
