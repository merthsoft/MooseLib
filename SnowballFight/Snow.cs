using Merthsoft.MooseEngine;
using Merthsoft.MooseEngine.Defs;
using Merthsoft.MooseEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.SnowballFight
{
    public class Snow : AnimatedGameObject
    {
        public enum Strength { Light, Heavy }
        
        public Snow(AnimatedGameObjectDef def, int layer, Strength strength) : base(def, Vector2.Zero, layer, state: strength.ToString().ToLower())
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (var x = 0; x < ParentMap!.Width * ParentMap!.TileWidth; x += Sprite.TextureRegion.Width)
                for (var y = 0; y < ParentMap!.Height * ParentMap!.TileHeight; y += Sprite.TextureRegion.Height)
                    Sprite.Draw(spriteBatch, new Vector2(x, y), SpriteTransform, SpriteEffects);
        }
    }
}
