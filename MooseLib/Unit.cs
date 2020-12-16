using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;

namespace MooseLib
{
    public class Unit
    {
        public Vector2 Location { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public Direction Direction { get; set; } = Direction.Down;

        public State State { get; set; } = State.Idle;

        public int Speed { get; set; }

        public SpriteEffects SpriteEffects { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;

        private string PlayKey
            => Direction == Direction.None
                ? State.ToString().ToLower()
                : $"{State.ToString().ToLower()}_{Direction}";
        
        private string PreviousPlayKey = "";

        private readonly Vector2 SpriteOffset;

        public Unit(SpriteSheet spriteSheet, int cellX, int cellY, Direction direction = Direction.Down, State state = State.Idle)
        {
            Sprite = new AnimatedSprite(spriteSheet);
            Location = new(cellX * 16, cellY * 16);
            SpriteOffset = new Vector2(8, 8);
            Direction = direction;
            State = state;
        }

        public void Draw(SpriteBatch spriteBatch)
            => Sprite.Draw(spriteBatch, Location + SpriteOffset, Rotation, Scale, SpriteEffects);

        public void Update(GameTime gameTime)
        {
            if (PlayKey != PreviousPlayKey)
            {
                Sprite.Play(PlayKey);
                PreviousPlayKey = PlayKey;
            }

            Sprite.Update(gameTime);
        }

        public bool Clicked(Vector2 worldLocation)
            => worldLocation.X >= Location.X && worldLocation.X < (Location.X + 16)
            && worldLocation.Y >= Location.Y && worldLocation.Y < (Location.Y + 16);

        public Vector2 GetCell(TiledMap map)
            => Location / new Vector2(map.TileWidth, map.TileHeight);

        public bool InCell(TiledMap map, int x, int y)
            => (Location / new Vector2(map.TileWidth, map.TileHeight)) == new Vector2(x, y);
    }
}
