using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
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
        public const string Idle = nameof(Idle);
        public const string Walk = nameof(Walk);
        public const string Step = nameof(Step);
        public const string IdleStep = nameof(IdleStep);
    }

    private FlowField? flowField;
    private bool followFlowField = false;

    private Vector2 MoveDirection;
    private Vector2 NextPosition;
    Size2 tileSize = Size2.Empty;

    private int idleTimer = 0;

    public RtsMap Map => (ParentMap as RtsMap)!;

    public Unit(UnitDef def, Vector2 position) 
        : base(def, position)
    {
        State = States.Idle;

        StateMap[States.Idle] = Idle;
        StateMap[States.Walk] = Walk;
        StateMap[States.Step] = Step;
        StateMap[States.IdleStep] = Step;
    }

    public override void OnAdd()
    {
        base.OnAdd();

        tileSize = ParentMap!.TileSize;
    }

    public void SetFlow(FlowField flowField)
    {
        this.flowField = flowField;
        followFlowField = true;
    }

    private string Idle(MooseGame mooseGame, GameTime gameTime)
    {
        if (followFlowField)
            return States.Walk;
        
        if (idleTimer == 0)
        {
            idleTimer = MooseGame.Random.Next(25, 100);
            var tiles = new List<Vector2>();
            var (cX, cY) = Cell;
            for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    var blockingVector = ParentMap.GetBlockingVector(cX + x, cY + y);
                    if (blockingVector.Any() && blockingVector.Sum() == 0)
                        tiles.Add(new(x, y));
                }

            if (!tiles.Any())
                return States.Idle;

            MoveDirection = tiles.RandomElement();
            NextPosition = Position + MoveDirection * tileSize;
            return States.IdleStep;
        }
        idleTimer--;
        return States.Idle;
    }

    private string Walk(MooseGame mooseGame, GameTime gameTime)
    {
        if (flowField == null)
        {
            followFlowField = false;
            return States.Idle;
        }

        var cell = Cell;
        var f = flowField.CostMap[cell.X, cell.Y];

        if (f.CostValue == 0 || !f.Valid)
        {
            followFlowField = false;
            return States.Idle;
        }

        MoveDirection = new(2 * (f.NextX - cell.X), 2 * (f.NextY - cell.Y));
        NextPosition = new(f.NextX * tileSize.Width, f.NextY * tileSize.Height);
        NextPosition.Floor();
        return States.Step;
    }

    private string Step(MooseGame mooseGame, GameTime gameTime)
    {
        if (MoveDirection != Vector2.Zero)
        {
            Position = Position + MoveDirection;
            if (Position.GetFloor() != NextPosition)
                return State;

            MoveDirection = Vector2.Zero;
        }
        return State == States.IdleStep ? States.Idle : States.Walk;
    }
}
