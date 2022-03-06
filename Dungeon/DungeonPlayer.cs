using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;

class DungeonPlayer : GameObjectBase
{
    public const string Left = "Left";
    public const string Right = "Right";
    public const string Up = "Up";
    public const string Down = "Down";

    public bool CanMove { get; set; } = true;

    public float AnimationRotation { get; set; }

    private DungeonPlayerDef PlayerDef => (Def as DungeonPlayerDef)!;

    public DungeonPlayer(DungeonPlayerDef def) : base(def)
    {

    }

    public override void Draw(SpriteBatch spriteBatch)
        => spriteBatch.Draw(PlayerDef.Texture, WorldRectangle.Move(new(8, 8)).ToRectangle(), new(32, 48, 16, 16), Color.White, Rotation + AnimationRotation, new(8, 8), Effects, 1f);
    
    public override void Update(GameTime gameTime)
    {
        if (CanMove)
        {
            var moveDelta = Vector2.Zero;
            var shouldRotate = false;
            if (MooseGame.Instance.IsKeyDown(Keys.Left))
            {
                moveDelta = new(-1, 0);
                Direction = Left;
                shouldRotate = true;
            }
            else if (MooseGame.Instance.IsKeyDown(Keys.Right))
            {
                moveDelta = new(1, 0);
                Direction = Right;
                shouldRotate = true;
            }
            else if (MooseGame.Instance.IsKeyDown(Keys.Down))
            {
                moveDelta = new(0, 1);
                Direction = Up;
                shouldRotate = true;
            }
            else if (MooseGame.Instance.IsKeyDown(Keys.Up))
            {
                moveDelta = new(0, -1);
                Direction = Down;
                shouldRotate = true;
            }

            if (shouldRotate)
                this.AddTween(p => p.AnimationRotation, Direction == Left ? -1 : 1, .1f, autoReverse: true, onEnd: _ => AnimationRotation = 0);
            
            if (moveDelta != Vector2.Zero)
            {
                CanMove = false;

                var newTilePosition = Position / 16 + moveDelta;
                var tile = DungeonGame.GetDungeonTile((int)newTilePosition.X, (int)newTilePosition.Y);
                if (tile >= Tile.BLOCKING_START)
                    moveDelta = Vector2.Zero;
                TweenToPosition(Position + moveDelta * 16, .2f, onEnd: _ => CanMove = true);
            }

        }

        (Rotation, Effects) = Direction switch
        {
            Up => (MathF.PI/2, SpriteEffects.None),
            Down => (-MathF.PI/2, SpriteEffects.None),
            Left => (0, SpriteEffects.FlipHorizontally),
            _ => (0, SpriteEffects.None),
        };
    }
}
