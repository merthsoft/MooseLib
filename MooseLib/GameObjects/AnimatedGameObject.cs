using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace MooseLib.GameObjects
{
    public class AnimatedGameObject : GameObjectBase
    {
        public AnimatedSprite Sprite { get; set; }

        public SpriteEffects SpriteEffects { get; set; }

        public Transform2 SpriteTransform { get; set; }

        public virtual string PlayKey => State.ToLower();

        private string PreviousPlayKey = "";

        public override RectangleF WorldRectangle 
            => Sprite.GetBoundingRectangle(WorldPosition + SpriteTransform.WorldPosition, SpriteTransform.WorldRotation, SpriteTransform.WorldScale);

        public AnimatedGameObject(MooseGame parentGame, string animationKey, Vector2 position, string state = "idle", int layer = 0, float rotation = 0, Vector2? scale = null)
            : base(parentGame, layer, position)
        {
            Sprite = new AnimatedSprite(parentGame.LoadAnimatedSpriteSheet(animationKey));
            SpriteTransform = new(Sprite.Origin, rotation, scale);
            WorldPosition = position;
            WorldSize = new(Sprite.TextureRegion.Width, Sprite.TextureRegion.Height);
            State = state;
            ParentGame = parentGame;
            Layer = layer;
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
