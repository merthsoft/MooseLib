using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.Moose.Rts;

internal record UnitDef : TextureGameObjectDef
{
    public UnitDef(string defName) : base($"Unit_{defName}", $"Units/{defName}", new(0, 0, 8, 8)) 
    {
        DefaultLayer = "units";
        DefaultSize = new(8, 8);
    }

    public override void LoadContent(MooseContentManager contentManager) => base.LoadContent(contentManager);

}

internal class Unit : TextureGameObject
{
    public class States
    {
        public const string Seeking = "seeking";
        public const string Idle = "idle";
        public const string Walk = "walk";
        public const string Attack = "attack";
    }

    public Queue<Point> MoveQueue { get; } = new Queue<Point>();
    public Vector2 MoveDirection = Vector2.Zero;
    public Vector2 NextLocation = Vector2.Zero;
    public Point NextCell = Point.Zero;
    
    public Point FinalCell = Point.Zero;
    public Point? StartCell = null;
    
    private bool stepFlag;
    int seekCount = 0;

    public RtsMap Map => (ParentMap as RtsMap)!;

    public Unit(UnitDef def, Vector2 position) 
        : base(def, position)
    {
        
    }

    public void MoveTo(Point point)
    {
        FinalCell = point;
        State = States.Seeking;
        MoveQueue.Clear();
        StartCell = Cell;
    }

    private void FillMoveQueue()
    {
        if (!StartCell.HasValue)
            return;

        var path = ParentMap.FindCellPath(StartCell!.Value, FinalCell);
        if (path.Any())
        {
            foreach (var c in path)
                MoveQueue.Enqueue(c);

            State = States.Walk;
        }
    }

    public override void Update(MooseGame mooseGame, GameTime gameTime)
    {
        base.Update(mooseGame, gameTime);

        switch (State)
        {
            case States.Seeking:
                Seek();
                break;
            case States.Walk:
                Walk();
                break;
            case States.Idle:
                Idle();
                break;
        }
    }

    private void Idle()
    {
        StartCell = null;
        MoveDirection = Vector2.Zero;
        NextLocation = Vector2.Zero;
        Map.ReserveLocation(Layer, Cell);
    }

    private void Walk()
    {
        if (MoveDirection != Vector2.Zero)
        {
            takeStep();
            //if (stepFlag)
            //    takeStep();
            //stepFlag = !stepFlag;
        }
        else if (MoveQueue.Count == 0)
        {
            if (StartCell.HasValue && StartCell.Value == FinalCell)
            {
                StartCell = null;
                State = States.Idle;
            }
            else
            {
                StartCell = Cell;
                State = States.Seeking;
            }
        }
        else
        {
            NextCell = MoveQueue.Dequeue();
            if (Map.IsBlockedAt(Layer, NextCell.X, NextCell.Y) > 0)
            {
                StartCell = Cell;
                State = States.Seeking;
                MoveQueue.Clear();
            }
            else
            {
                var cell = Cell;
                MoveDirection = new(NextCell.X - cell.X, NextCell.Y - cell.Y);
                NextLocation = NextCell.ToVector2() * ParentMap!.TileSize;
                Map.ClearReservation(Layer, cell);
                Map.ReserveLocation(Layer, NextCell);
                takeStep();
            }
        }
    }

    private void Seek()
    {
        FillMoveQueue();
        if (State == States.Seeking)
        {
            seekCount++;
            if (seekCount == 50)
            {
                seekCount = 0;
                State = States.Idle;
            }
        }
        else
        {
            seekCount = 0;
        }
    }

    void takeStep()
    {
        Map.MoveObject(this, Position + MoveDirection);
        if (Position.GetFloor() == NextLocation.GetFloor())
            MoveDirection = Vector2.Zero;
    }
}
