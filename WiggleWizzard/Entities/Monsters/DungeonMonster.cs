using Merthsoft.Moose.Dungeon.Tiles;
using MonoGame.Extended.TextureAtlases;

namespace Merthsoft.Moose.Dungeon.Entities.Monsters;
public record MonsterDef : DungeonCreatureDef
{
    public MonsterTile Monster { get; set; }

    public MonsterDef(string DefName, MonsterTile monster) : base(DefName, "monsters", "Monsters")
    {
        Monster = monster;
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        var index = (int)Monster;
        SpriteSheet = new();
        SpriteSheet.TextureAtlas = TextureAtlas.Create(AnimationKey, contentManager.LoadImage(AnimationKey), 16, 16);

        SpriteSheet.Cycles["idle"] = new()
        {
            FrameDuration = .2f,
            IsLooping = true,
            Frames = [new(index), new(index + 1), new(index + 2)]
        };
    }
}

public abstract class DungeonMonster : DungeonCreature
{
    public MonsterDef MonsterDef { get; }
    
    public bool SeenYet;

    public string NextMove;

    public DungeonMonster(MonsterDef def, Vector2? position) : base(def, position, "Up", 0, "monsters")
    {
        MonsterDef = def;
        MiniMapTile = MiniMapTile.Monster;
        DrawIndex = (int)def.Monster;
        NextMove = "";
        Origin = new(8, 8);
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
        }

        if (State == Dead)
            player.NumKills++;
    }

    protected abstract string MonsterUpdate(GameTime gameTime);

    public override void Draw(MooseGame mooseGame, GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(mooseGame, gameTime, spriteBatch);
        if (NextMove != "" && !CurrentlyBlockingInput)
            spriteBatch.Draw(game.ArrowTexture, WorldRectangle.Move(AnimationPosition.X + 8, AnimationPosition.Y + 8).ToRectangle(), null, Color.White,
                CalculateRotation(NextMove), new(8, 8), SpriteEffects.None, 1f);
    }
}
