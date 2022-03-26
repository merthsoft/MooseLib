using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

namespace Merthsoft.Moose.Builder;

public class BuilderGame : MooseGame
{
    public static new BuilderGame Instance = null!;
    public static Texture2D Tiles = null!;
    public static bool MoveTick = false;
    public static BuilderMap BuilderMap = null!;

    public static List<BuilderTask> Tasks = new();

    TimeSpan LastUpdate = TimeSpan.Zero;

    private static Dictionary<string, Func<BuilderUnitDef, int, int, BuilderUnit>> unitGenerator = new();

    public BuilderGame()
    {
        Instance = this;
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        DefaultBackgroundColor = Color.DarkGreen
    };

    protected override void Load()
    {
        Tiles = ContentManager.LoadImage("tileset");

        ActiveMaps.Add(BuilderMap = new BuilderMap());

        AddDefaultRenderer<ObjectLayer>("objects", new SpriteBatchObjectRenderer(SpriteBatch));
        AddDefaultRenderer<TileLayer<BuilderTiles>>("tiles", 
            new SpriteBatchWangTileTextureRenderer<BuilderTiles>(SpriteBatch, 12, 12, Tiles, 1, 1));

        AddUnitDef(new BuilderUnitDef(450, "WorkerMale"),
            (def, x, y) => new BuilderUnit(def, x, y));

        ZoomIn(2f);

        DefaultRenderHooks = new();
        DefaultRenderHooks["units"] = new(PostHook: _ => DrawTasks());
    }

    protected override void PostLoad()
    {
        SpawnUnit<BuilderUnit>(GetDef<BuilderUnitDef>("WorkerMale"), 10, 10);
        SpawnUnit<BuilderUnit>(GetDef<BuilderUnitDef>("WorkerMale"), 10, 11);
        SpawnUnit<BuilderUnit>(GetDef<BuilderUnitDef>("WorkerMale"), 10, 12);
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        if ((gameTime.TotalGameTime - LastUpdate).TotalSeconds >= .5)
        {
            MoveTick = true;
            LastUpdate = gameTime.TotalGameTime;
        }

        var mouseX = (int)(WorldMouse.X / 12);
        var mouseY = (int)(WorldMouse.Y / 12);

        if (IsLeftMouseDown())
            AddTask(BuilderMap.AddBlueprint(BuilderTiles.WoodenWall, mouseX, mouseY));
    }

    public void DrawTasks()
    {
        foreach (var task in Tasks)
        {
            if (!task.Started || task.Completed || task.Cancelled)
                continue;

            SpriteBatch.FillRectangle(task.X * 12, task.Y * 12, 12 * task.Percent, 5, Color.Red, 1);
            SpriteBatch.DrawRectangle(task.X * 12, task.Y * 12, 12, 5, Color.Black);
        }
    }

    public static void AddTask(BuilderTask? task)
    {
        if (task == null)
            return;
        Tasks.Add(task);
    }

    public static IEnumerable<BuilderTask> NearestTasks(int x, int y)
        => Tasks.OrderBy(t => (x - t.X) * (x - t.X) + (y - t.Y) * (y - t.Y));

    protected override void PostUpdate(GameTime gameTime)
    {
        MoveTick = false;
        Tasks.RemoveAll(t => t.Completed || t.Cancelled);
    }

    public void AddUnitDef(BuilderUnitDef def, Func<BuilderUnitDef, int, int, BuilderUnit> generator)
    {
        AddDef(def);
        unitGenerator[def.DefName] = generator;
    }

    public static TUnit SpawnUnit<TUnit>(BuilderUnitDef def, int cellX, int cellY) where TUnit : BuilderUnit
        => Instance.AddObject((unitGenerator[def.DefName](def, cellX, cellY) as TUnit)!);
}
