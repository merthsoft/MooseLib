using Merthsoft.Moose.MooseEngine.Defs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.GameObjects;

public class TextureGameObject : GameObjectBase
{
    public new TextureGameObjectDef Def => (base.Def as TextureGameObjectDef)!;

    public SpriteEffects SpriteEffects { get; set; }

    public TextureGameObject(TextureGameObjectDef def, Vector2? position = null, int? layer = null, Vector2? size = null, string? direction = null) : base(def, position, layer, size, direction)
    {
    }

    public override void Update(GameTime gameTime) { }

    public override void Draw(SpriteBatch spriteBatch)
        => spriteBatch.Draw(Def.Texture,
                (Rectangle)base.WorldRectangle, Def.SourceRectangle,
                Color.White, Rotation, Def.Origin, SpriteEffects, 0);
}
