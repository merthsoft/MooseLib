﻿using MonoGame.Extended.Sprites;

namespace Merthsoft.Moose.MooseEngine.Defs;

public record AnimatedGameObjectDef(string DefName, string AnimationKey) : GameObjectDef(DefName)
{
    public Vector2 Origin { get; set; } = Vector2.Zero;

    public SpriteSheet SpriteSheet { get; protected set; } = null!;

    public override void LoadContent(MooseContentManager contentManager)
        => SpriteSheet = contentManager.LoadAnimatedSpriteSheet($"Animations/{AnimationKey}");
}
