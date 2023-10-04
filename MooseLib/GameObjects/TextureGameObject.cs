using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.MooseEngine.GameObjects;

public class TextureGameObject : GameObjectBase
{
    public TextureGameObjectDef TextureGameObjectDef;

    public SpriteEffects SpriteEffects { get; set; }
    
    public TextureGameObject(TextureGameObjectDef def, Vector2? position = null, string? direction = null, float? rotation = null, Vector2? size = null, string? layer = null) : base(def, position, direction, rotation, size, layer)
    {
        TextureGameObjectDef = def;
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
        => spriteBatch.Draw(TextureGameObjectDef.Texture,
                WorldRectangle.Move(Origin).ToRectangle(), 
                TextureGameObjectDef.SourceRectangle,
                Color, Rotation, Origin, SpriteEffects, LayerDepth);
}
