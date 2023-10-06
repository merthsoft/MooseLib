using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.Moose.Rts;

internal record UnitDef : TextureGameObjectDef
{
    public UnitTile UnitTile { get; }
    public UnitDef(string defName, UnitTile unitTile) : base($"Unit_{defName}", $"Units/{defName}", new(0, 0, 8, 8))
    {
        DefaultLayer = "units";
        DefaultSize = new(8, 8);
        UnitTile = unitTile;
    }

    public override void LoadContent(MooseContentManager contentManager) {}

}

internal class Unit : TextureGameObject
{
    public class States
    {
        public const string Idle = nameof(Idle);
        public const string Walk = nameof(Walk);
        public const string Step = nameof(Step);
        public const string IdleStep = nameof(IdleStep);
        public const string Harvest = nameof(Harvest);
    }

    private FlowField? flowField;
    private bool followFlowField = false;

    private Vector2 MoveDirection;
    private Vector2 NextPosition;
    Size2 tileSize = Size2.Empty;

    private int stateTimer = 0;

    public RtsMap Map => (ParentMap as RtsMap)!;

    public Unit(UnitDef def, Vector2 position) 
        : base(def, position)
    {
        State = States.Idle;

        StateMap[States.Idle] = Idle;
        StateMap[States.Walk] = Walk;
        StateMap[States.Step] = Step;
        StateMap[States.IdleStep] = Step;
        StateMap[States.Harvest] = Harvest;
        DrawIndex = (int)def.UnitTile;
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

    private string Harvest(MooseGame mooseGame, GameTime gameTime)
    {
        if (followFlowField)
            return States.Walk;

        var (cX, cY) = Cell;
        cX += (int)MoveDirection.X;
        cY += (int)MoveDirection.Y;
        if (!Map.CanHarvest(cX, cY))
        {
            stateTimer = 0;
            return States.Idle;
        }

        if (stateTimer == 0)
        {
            Map.HarvestTile(cX, cY, this);
            stateTimer = 0;
            return States.Idle;
        }
        stateTimer--;
        return States.Harvest;
    }


    private string Idle(MooseGame mooseGame, GameTime gameTime)
    {
        if (followFlowField)
            return States.Walk;
        
        if (stateTimer == 0)
        {
            var (cX, cY) = Cell;
            if (Map.CanHarvest(cX, cY))
            {
                MoveDirection = Vector2.Zero;
                stateTimer = Map.GetHarvestDelay(cX, cY);
                return States.Harvest;
            }

            var tiles = new List<Vector2>();
            for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    tiles.Add(new(x, y));
                }

            foreach (var tile in tiles)
            {
                var tX = cX + (int)tile.X;
                var tY = cY + (int)tile.Y;

                if (Map.CanHarvest(tX, tY))
                {
                    MoveDirection = tile;
                    stateTimer = Map.GetHarvestDelay(tX, tY);
                    return States.Harvest;
                }
            }

            foreach (var tile in tiles.Shuffle())
            {
                var tX = cX + (int)tile.X;
                var tY = cY + (int)tile.Y;

                var blockingVector = ParentMap.GetBlockingVector(tX, tY);
                if (blockingVector.Any() && blockingVector.Sum() == 0)
                {
                    MoveDirection = tile;
                    stateTimer = MooseGame.Random.Next(25, 100);
                    NextPosition = Position + MoveDirection * tileSize;
                    return States.IdleStep;
                }
            }

            // We can return in the foreach if we find something to work with
            return States.Idle;
        }
        stateTimer--;
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
