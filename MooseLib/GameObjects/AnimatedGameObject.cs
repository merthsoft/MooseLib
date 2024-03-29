﻿using Merthsoft.Moose.MooseEngine.Defs;
using MonoGame.Extended.Sprites;

namespace Merthsoft.Moose.MooseEngine.GameObjects;

public class AnimatedGameObject : GameObjectBase
{
    public AnimatedGameObjectDef AnimatedGameObjectDef { get; }

    public AnimatedSprite Sprite { get; set; }
    public SpriteEffects SpriteEffects { get; set; }
    public Action? StateCompleteAction { get; set; }

    public virtual string PlayKey => Direction == "" ? State.ToLower() : $"{State.ToLower()}_{Direction.ToLower()}";

    protected string PreviousPlayKey = "";

    public override RectangleF WorldRectangle
        => new(Position, (Sprite.TextureRegion.Size * Scale).ToSize());

    public AnimatedGameObject(AnimatedGameObjectDef def, Vector2? position = null, string? layer = null, Vector2? transformLocation = null, float rotation = 0, Vector2? scale = null, string state = "", string direction = "")
        : base(def, position, direction: direction, layer: layer)
    {
        AnimatedGameObjectDef = def;
        Sprite = def.SpriteSheet == null ? null! : new AnimatedSprite(def.SpriteSheet) { Origin = Origin };
        DrawOffset = transformLocation ?? Vector2.Zero;
        Rotation = rotation;
        State = state;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        if (PlayKey != PreviousPlayKey)
        {
            Sprite.Play(PlayKey, StateCompleteAction);
            PreviousPlayKey = PlayKey;
        }

        Sprite.Update(gameTime);
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
        => spriteBatch.Draw(Sprite.TextureRegion.Texture,
                WorldRectangle.Move(Origin).ToRectangle(), 
                Sprite.TextureRegion.Bounds,
                Color, Rotation, Origin, SpriteEffects, LayerDepth);
}
