namespace Merthsoft.Moose.Dungeon.Entities.Monster;
public abstract class DungeonMonster : DungeonCreature
{
    public MonsterDef MonsterDef { get; }
    
    public override int DrawIndex => (int)MonsterDef.Monster;

    public DungeonMonster(MonsterDef def, Vector2? position) : base(def, position, "Up", 0, new(16, 16), "monsters")
    {
        MonsterDef = def;
    }

    public override void PostUpdate(MooseGame game, GameTime gameTime)
    {
        base.PostUpdate(game, gameTime);
        var dungeonGame = (game as DungeonGame)!;
        if (!dungeonGame.Player.HasInputThisFrame)
            return;

        MonsterUpdate(dungeonGame, gameTime);
    }

    protected abstract void MonsterUpdate(DungeonGame dungeonGame, GameTime gameTime);
}
