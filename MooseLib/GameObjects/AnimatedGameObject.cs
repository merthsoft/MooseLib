﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using Merthsoft.MooseEngine.Defs;

namespace Merthsoft.MooseEngine.GameObjects
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

        public virtual string PlayKey => Direction == null ? State.ToLower() : $"{State.ToLower()}_{Direction.ToLower()}";

        private string PreviousPlayKey = "";

        public override RectangleF WorldRectangle 
            => Sprite.GetBoundingRectangle(WorldPosition + SpriteTransform.WorldPosition, SpriteTransform.WorldRotation, SpriteTransform.WorldScale);

        public AnimatedGameObject(AnimatedGameObjectDef def, Vector2? position = null, int layer = 0, Vector2? transformLocation = null, float rotation = 0, Vector2? scale = null, string state = "", string? direction = null)
            : base(def, position, layer, direction: direction)
        {
            Sprite = new AnimatedSprite(def.SpriteSheet) { Origin = def.Origin };
            SpriteTransform = new(transformLocation ?? Vector2.Zero, rotation, scale);
            WorldSize = new(Sprite.TextureRegion.Width, Sprite.TextureRegion.Height);
            State = state;
        }

        public override (Texture2D texture, Rectangle textureRectangle) GetTexture()
            => (Sprite.TextureRegion.Texture, Sprite.TextureRegion.Bounds);

        public override void Update(GameTime gameTime)
        {
            if (PlayKey != PreviousPlayKey)
            {
                Sprite.Play(PlayKey, StateCompleteAction);
                PreviousPlayKey = PlayKey;
            }

            Sprite.Update(gameTime);

            base.Update(gameTime);
        }
    }
}
