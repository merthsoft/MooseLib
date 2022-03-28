using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.MageQuest;

public record MagePlayerDef : AnimatedGameObjectDef
{
    public MagePlayerDef() : base("player", "Player")
    {
        DefaultLayer = "objects";
        DefaultOrigin = new(16, 24);
        DefaultSize = new(32, 32);
    }
}

public class MagePlayer : AnimatedGameObject
{
    public MagePlayerDef MagePlayerDef;

    public MagePlayer(MagePlayerDef def, Vector2 position) : base(def, position, layer: "objects", state: States.Walk, direction: Directions.South)
    {
        MagePlayerDef = def;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        var mageGame = (game as MageGame)!;
        base.Update(game, gameTime);
        string direction = null;
        var moveDelta = Vector2.Zero;
        if (game.IsKeyDown(Keys.Left))
        {
            direction = Directions.West;
            moveDelta += Directions.WestVector;
        }
        else if (game.IsKeyDown(Keys.Right))
        {
            direction = Directions.East;
            moveDelta += Directions.EastVector;
        }
        
        if (game.IsKeyDown(Keys.Down))
        {
            direction ??= Directions.South;
            moveDelta += Directions.SouthVector;
        }
        else if (game.IsKeyDown(Keys.Up))
        {
            direction ??= Directions.North;
            moveDelta += Directions.NorthVector;
        } else if (game.WasKeyJustPressed(Keys.Space))
        {
            var shotDelta = Directions.GetVector(Direction);
            mageGame.Cast(MageGame.GetDef<Spells.FireballDef>()!, Position, Position + 120 * shotDelta);
            return;
        }

        if (direction != null)
        {
            Direction = direction;

            var deltaX = new Vector2(moveDelta.X, 0);
            var deltaY = new Vector2(0, moveDelta.Y);

            if (!mageGame.BlocksPlayer(Position + 8 * deltaX))
                Position += 2*deltaX;

            if (!mageGame.BlocksPlayer(Position + 8 * deltaY))
                Position += 2*deltaY;
        }
    }
}
