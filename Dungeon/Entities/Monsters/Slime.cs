namespace Merthsoft.Moose.Dungeon.Entities.Monsters;
public class Slime : DungeonMonster
{
    public Slime(MonsterDef def, Vector2? position) : base(def, position)
    {
    }

    protected override void MonsterUpdate(GameTime gameTime) {
        if (SeenCount == 0)
            return;
    }
}
