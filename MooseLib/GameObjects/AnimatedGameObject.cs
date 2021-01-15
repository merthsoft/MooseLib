using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MooseLib.Defs;

namespace MooseLib.GameObjects
{
    public class AnimatedGameObject : GameObjectBase
    {
        public new AnimatedGameObjectDef Def {
            get => (base.Def as AnimatedGameObjectDef)!;
            set => base.Def = value; 
        }
        
        public AnimatedSprite Sprite { get; set; }

        public SpriteEffects SpriteEffects { get; set; }

        public Transform2 SpriteTransform { get; set; }

        public virtual string PlayKey => State.ToLower();

        private string PreviousPlayKey = "";

        public override RectangleF WorldRectangle 
            => Sprite.GetBoundingRectangle(WorldPosition + SpriteTransform.WorldPosition, SpriteTransform.WorldRotation, SpriteTransform.WorldScale);

        public AnimatedGameObject(MooseGame parentGame, AnimatedGameObjectDef def, Vector2? position = null, int layer = 0, float rotation = 0, Vector2? scale = null, string state = "")
            : base(parentGame, def, position, layer)
        {
            Sprite = new AnimatedSprite(def.SpriteSheet) { Origin = def.Origin };
            SpriteTransform = new(Sprite.Origin, rotation, scale);
            WorldSize = new(Sprite.TextureRegion.Width, Sprite.TextureRegion.Height);
            State = state;
        }

        public override void Draw(SpriteBatch spriteBatch)
            => Sprite.Draw(spriteBatch, WorldPosition, SpriteTransform, SpriteEffects);

        public override void Update(GameTime gameTime)
        {
            if (PlayKey != PreviousPlayKey)
            {
                Sprite.Play(PlayKey, StateCompleteAction);
                PreviousPlayKey = PlayKey;
            }

            Sprite.Update(gameTime);
        }
    }
}
