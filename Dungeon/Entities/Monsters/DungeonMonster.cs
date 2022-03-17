using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Monsters;
public record MonsterDef : DungeonCreatureDef
{
    public MonsterTile Monster { get; set; }

    public MonsterDef(string DefName, MonsterTile monster) : base(DefName, "monsters", "Monsters")
    {
        Monster = monster;
    }
}

public abstract class DungeonMonster : DungeonCreature
{
    public MonsterDef MonsterDef { get; }
    
    public bool SeenYet;

    public string NextMove;

    public DungeonMonster(MonsterDef def, Vector2? position) : base(def, position, "Up", 0, new(16, 16), "monsters")
    {
        MonsterDef = def;
        MiniMapTile = MiniMapTile.Monster;
        DrawIndex = (int)def.Monster;
        NextMove = "";
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);

        if (!DungeonPlayer.Instance.HasInputThisFrame)
            return;

        if (FrozenTurnCount != 0 && SeenCount > 0)
            NextMove = MonsterUpdate(gameTime);

        if (SeenCount > 0 && !SeenYet)
        {
            SeenYet = true;
            CurrentlyBlockingInput = true;
            this.TweenToScale(new(3, 3), .5f,
                onEnd: _ => this.TweenToScale(Vector2.One, .7f, .1f,
                    onEnd: _ => CurrentlyBlockingInput = false));
            return;
        }
    }

    protected abstract string MonsterUpdate(GameTime gameTime);

    public override void Draw(MooseGame mooseGame, GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(mooseGame, gameTime, spriteBatch);
        if (NextMove != "")
            spriteBatch.Draw(game.ArrowTexture, WorldRectangle.Move(AnimationPosition.X + 8, AnimationPosition.Y + 8).ToRectangle(), null, Color.White,
                CalculateRotation(NextMove), new(8, 8), SpriteEffects.None, 1f);
    }
}
