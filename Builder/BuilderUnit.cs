using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Builder;
public record BuilderUnitDef : TextureGameObjectDef {
    public int DrawIndex { get; set; }
    
    public BuilderUnitDef(int drawIndex, string defName) : base(defName, "tileset")
    {
        DrawIndex = drawIndex;
        DefaultLayer = "units";
        DefaultOrigin = new(6, 6);
        DefaultSize = new(12, 12);
    }
}

public class BuilderUnit : TextureGameObject
{
    public BuilderUnitDef BuilderUnitDef;
    public int DrawIndex = 0;

    public List<BuilderTask> TaskQueue = new();

    public BuilderUnit(BuilderUnitDef def, int cellX, int cellY) : base(def, new(cellX * 12, cellY * 12), layer: "units")
    {
        BuilderUnitDef = def;
        DrawIndex = def.DrawIndex;
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
        => spriteBatch.Draw(TextureGameObjectDef.Texture,
                (Rectangle)WorldRectangle.Move(6, 6), 
                TextureGameObjectDef.Texture.GetSourceRectangle(DrawIndex, 12, 12, 1, 1),
                Color, Rotation, TextureGameObjectDef.DefaultOrigin, SpriteEffects, .5f);

    public override void Update(MooseGame game, GameTime gameTime)
    { 
        base.Update(game, gameTime);

        if (!BuilderGame.MoveTick)
            return;

        BuilderTask? task = null;
        if (TaskQueue.Count == 0)
        {
            var tasks = BuilderGame.NearestTasks((int)Position.X / 12, (int)Position.Y / 12);
            task = tasks.FirstOrDefault(t => !t.Started);

            if (task == null)
            {
                var deltaX = game.Random.Next(-1, 2);
                var deltaY = game.Random.Next(-1, 2);
                TweenToPosition(Position + 12 * new Vector2(deltaX, deltaY), .25f);
                return;
            }
            TaskQueue.Add(task);
        }
        else
            task = TaskQueue.First();

        if (!task.Started)
            task.Start(this);

        var path = ParentMap.FindCellPath(GetCell(), task.GetCell());

        if (path.Count() > 3)
        {
            var newPos = 12 * (path.Skip(1).First().ToVector2());
            TweenToPosition(newPos, .25f);
            return;
        }

        task.DoWork(1);
        if (task.Completed)
            TaskQueue.Remove(task);
    }
}
