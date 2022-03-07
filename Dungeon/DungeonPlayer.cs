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
    
    private readonly Stack<Keys> moveBuffer = new();

    public float AnimationRotation { get; set; }

    private DungeonPlayerDef PlayerDef => (Def as DungeonPlayerDef)!;

    public DungeonPlayer(DungeonPlayerDef def) : base(def)
    {

    }

    public override void Draw(SpriteBatch spriteBatch)
        => spriteBatch.Draw(PlayerDef.Texture, WorldRectangle.Move(new(8, 8)).ToRectangle(), new(32, 48, 16, 16), Color.White, Rotation + AnimationRotation, new(8, 8), Effects, 1f);
    
    public override void Update(GameTime gameTime)
    {
        bool keyPress(Keys key) => MooseGame.Instance.WasKeyJustPressed(key) || (CanMove && MooseGame.Instance.IsKeyHeldLong(key));

        if (keyPress(Keys.Left))
            moveBuffer.Push(Keys.Left);
        else if (keyPress(Keys.Right))
            moveBuffer.Push(Keys.Right);
        else if (keyPress(Keys.Down))
            moveBuffer.Push(Keys.Down);
        else if (keyPress(Keys.Up))
            moveBuffer.Push(Keys.Up);

        if (CanMove && moveBuffer.Count > 0)
        {
            var moveDelta = Vector2.Zero;
            switch (moveBuffer.Pop())
            {
                case Keys.Left:
                    moveDelta = new(-1, 0);
                    Direction = Left;
                    break;
                case Keys.Right:
                    moveDelta = new(1, 0);
                    Direction = Right;
                    break;
                case Keys.Down:
                    moveDelta = new(0, 1);
                    Direction = Up;
                    break;
                case Keys.Up:
                    moveDelta = new(0, -1);
                    Direction = Down;
                    break;
            }

            this.AddTween(p => p.AnimationRotation, Direction == Left ? -1 : 1, .075f,
                onEnd: _ => this.AddTween(p => p.AnimationRotation, 0, .075f));

            if (moveDelta != Vector2.Zero)
            {
                CanMove = false;

                var newTilePosition = Position / 16 + moveDelta;
                var tile = DungeonGame.GetDungeonTile((int)newTilePosition.X, (int)newTilePosition.Y);
                if (tile >= Tile.BLOCKING_START)
                {
                    moveDelta = Vector2.Zero;
                    moveBuffer.Clear();
                    CanMove = true;
                } else
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
