using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;

namespace MooseLib.GameObjects
{
    public class AnimatedGameObject : GameObjectBase, IComparable<AnimatedGameObject>
    {
        public AnimatedSprite Sprite { get; set; }

        public SpriteEffects SpriteEffects { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public Vector2 DrawOffset { get; set; } = Vector2.Zero;

        public virtual string PlayKey => State.ToLower();
        
        private string PreviousPlayKey = "";


        public AnimatedGameObject(MooseGame parentGame, string animationKey, Vector2 position, Vector2 spriteOffset, string state = "idle", int layer = 0)
            : base(parentGame, layer, position)
        {
            Sprite = new AnimatedSprite(parentGame.LoadAnimatedSpriteSheet(animationKey));
            WorldPosition = position;
            DrawOffset = spriteOffset;
            State = state;
            ParentGame = parentGame;
            Layer = layer;
        }

        public override void Draw(SpriteBatch spriteBatch)
            => Sprite.Draw(spriteBatch, WorldPosition + DrawOffset, Rotation, Scale, SpriteEffects);

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
