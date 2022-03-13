﻿using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Monsters;
public record MonsterDef : DungeonCreatureDef
{
    public MonsterTile Monster { get; set; }

    public MonsterDef(string DefName, MonsterTile monster) : base(DefName, "monsters", "Monsters")
    {
        Monster = monster;
    }
}

public abstract class Monster : DungeonCreature
{
    public MonsterDef MonsterDef { get; }
    
    public override int DrawIndex => (int)MonsterDef.Monster;

    public bool SeenYet;

    public Monster(MonsterDef def, Vector2? position) : base(def, position, "Up", 0, new(16, 16), "monsters")
    {
        MonsterDef = def;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);

        if (SeenCount > 0 && !SeenYet)
        {
            SeenYet = true;
            CurrentlyBlockingInput = true;
            this.TweenToScale(new(3, 3), 1.2f,
                onEnd: _ => this.TweenToScale(Vector2.One, .7f, .1f,
                    onEnd: _ => CurrentlyBlockingInput = false));
            return;
        }

        if (!DungeonPlayer.Instance.HasInputThisFrame)
            return;

        if (FrozenTurnCount != 0)
            MonsterUpdate(gameTime);
    }

    protected abstract void MonsterUpdate(GameTime gameTime);
}
