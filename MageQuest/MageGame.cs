using Merthsoft.Moose.MageQuest.Spells;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MageQuest;

public class MageGame : MooseGame
{
    public static new MageGame Instance = null!;

    public MagePlayer Player = null!;
    public SpellBook SpellBook = new();

    public MageGame()
    {
        Instance = this;
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        DefaultBackgroundColor = new(197, 203, 87)
    };

    protected override void Load()
    {
        AddDef(new MagePlayerDef());

        AddDefaultRenderer<TiledMooseTileLayer>("map", new TiledMooseMapRenderer(GraphicsDevice));
        AddDefaultRenderer<ObjectLayer>("objects", new SpriteBatchObjectRenderer(SpriteBatch));

        LoadMap("home");

        ZoomIn(2f);

        AddSpellDef(new FireballDef(), (spellDef, start, end) => new Fireball(spellDef, start, end));
    }
    private void LoadMap(string mapName)
    {
        ActiveMaps.Clear();
        ActiveMaps.Add(new TiledMooseMap(Content.Load<TiledMap>($"Maps/{mapName}")));
        GetRenderer<TiledMooseMapRenderer>("map").Load(MainMap);
    }

    protected override void PostLoad()
    {
        base.PostLoad();

        Player = AddObject(new MagePlayer(GetDef<MagePlayerDef>()!, new(80, 80)));
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        base.PreUpdate(gameTime);

        MainCamera.Position = Player.Position - ScreenSize / 2 / MainCamera.Zoom;
    }

    private SpellDef AddSpellDef(SpellDef spellDef, Func<SpellDef, Vector2, Vector2, Spell> generator)
    {
        AddDef(spellDef);
        SpellBook.AddSpell(spellDef, generator);
        return spellDef;
    }

    public void Cast(SpellDef spellDef, Vector2 start, Vector2 end)
    {
        var spell = SpellBook.Cast(spellDef, start, end);

        var container = spell as SpellContainer;

        var spellsToAdd = container != null ? container!.Spells.ToArray() : new[] { spell };
        var manaCost = spell.ManaCost;

        foreach (var childSpell in spellsToAdd)
            AddObject(childSpell);
    }
    public bool BlocksPlayer(Vector2 vector2)
    {
        var blockingVector = MainMap.GetBlockingVector(vector2);
        if (blockingVector[0] > 0 || blockingVector[1] > 0 || blockingVector[4] > 0)
            return true;
        return false;
    }
}
