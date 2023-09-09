using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
using System.Linq;

namespace Merthsoft.Moose.Rts;
public class MooseRtsGame : MooseGame
{
    public static int MapSize = 500;
    RtsMap Map = new(MapSize, MapSize);
    SpriteFont font = null!;
    SpriteFont smallFont = null!;
    int reservation = 2;
    FlowField flowField = null!;

    public MooseRtsGame()
    {
        
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        ScreenHeight = 800,
        ScreenWidth = 800,
        IsMouseVisible = true,
        DefaultBackgroundColor = new(18, 14, 25),
    };

    protected override void Load()
    {
        font = ContentManager.BakeFont("Capital_Hill_Monospaced", 12);
        smallFont = ContentManager.BakeFont("Capital_Hill_Monospaced", 6);
        ActiveMaps.Add(Map);
        var archerDef = AddDef(new UnitDef("Archer"));

        AddDefaultRenderer<TileLayer<int>>("tiles", new SpriteBatchPrimitiveRectangleRenderer(SpriteBatch, TileWidth, TileHeight, [Color.Transparent, Color.Green, Color.Red]));
        AddDefaultRenderer<ObjectLayer<Unit>>("units", new SpriteBatchObjectRenderer(SpriteBatch));

        for (var i = 0; i < MapSize; i++)
        {
            Map.SetReservationIfInBounds(0, i, 2);
            Map.SetReservationIfInBounds(MapSize - 1, i, 2);
            Map.SetReservationIfInBounds(i, 0, 2);
            Map.SetReservationIfInBounds(i, MapSize - 1, 2);
        }

        //AddObject(new Unit(archerDef, new(0, 0)), Map);
        //AddObject(new Unit(archerDef, new(0, 16)), Map);
        //AddObject(new Unit(archerDef, new(16, 0)), Map);
        //AddObject(new Unit(archerDef, new(16, 16)), Map);
        //AddObject(new Unit(archerDef, new(24, 32)), Map);
        //AddObject(new Unit(archerDef, new(32, 24)), Map);

        MainCamera.Origin = new(0, 0);
        MainCamera.ZoomIn(1);
    }

    private void UpdateCamera(GameTime gameTime, Point mousePoint)
    {
        var movementDirection = Vector2.Zero;
        if (IsKeyDown(Keys.Down) || IsKeyDown(Keys.S))
            movementDirection += Vector2.UnitY;
        if (IsKeyDown(Keys.Up) || IsKeyDown(Keys.W))
            movementDirection -= Vector2.UnitY;
        if (IsKeyDown(Keys.Left) || IsKeyDown(Keys.A))
            movementDirection -= Vector2.UnitX;
        if (IsKeyDown(Keys.Right) || IsKeyDown(Keys.D))
            movementDirection += Vector2.UnitX;

        MainCamera.Move(movementDirection * 400 * gameTime.GetElapsedSeconds());

        var delta = GetScrollWheelDelta() / 1200f;
        if (delta != 0 && (delta > 0 || MainCamera.Zoom > .5f))
            MainCamera.ZoomToPoint(delta, new(mousePoint.X * TileWidth + TileWidth / 2, mousePoint.Y * TileHeight + TileHeight / 2));
        if (MainCamera.Zoom < .5f)
            MainCamera.Zoom = .5f;
    }

    protected override void PostUpdate(GameTime gameTime)
    {
        if (!IsActiveAndMouseInBounds)
            return;

        var mouseVec = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        var mousePoint = new Point((int)mouseVec.X / TileWidth, (int)mouseVec.Y / TileHeight);
        
        UpdateCamera(gameTime, mousePoint);


        if (!Map.CellIsInBounds(mousePoint))
            return;

        if (WasLeftMouseJustPressed() || (IsLeftMouseDown() && WasMouseMoved()))
        {
            flowField = (Map.PathFinder as FlowFieldPathFinder)?.GetFlow(Map.BlockingGrid, mousePoint);
            foreach (var unit in ReadObjects.OfType<Unit>())
            {
                unit.MoveTo(mousePoint);
            }
        }

        if (WasMiddleMouseJustPressed())
        {
            AddObject(new Unit(GetDef<UnitDef>("Unit_Archer"), new(mousePoint.X * TileWidth, mousePoint.Y * TileHeight)), Map);
        }

        if (WasRightMouseJustPressed())
        {
            reservation = Map.ReservationMap[mousePoint.X, mousePoint.Y] == 0 ? 2 : 0;
        }

        if (IsRightMouseDown())
        {
            Map.IsBlockingMapDirty = true;
            Map.SetReservationIfInBounds(mousePoint.X, mousePoint.Y, reservation);
            Map.SetReservationIfInBounds(mousePoint.X + 1, mousePoint.Y, reservation);
            Map.SetReservationIfInBounds(mousePoint.X, mousePoint.Y + 1, reservation);
            Map.SetReservationIfInBounds(mousePoint.X + 1, mousePoint.Y + 1, reservation);
            Map.SetReservationIfInBounds(mousePoint.X - 1, mousePoint.Y, reservation    );
            Map.SetReservationIfInBounds(mousePoint.X, mousePoint.Y - 1, reservation);
            Map.SetReservationIfInBounds(mousePoint.X - 1, mousePoint.Y - 1, reservation);
        }
    }

    private void DrawUnderUnits()
    {
        var transformMatrix = MainCamera.GetViewMatrix();
        SpriteBatch.Begin(transformMatrix: transformMatrix);

        for (var x = 0; x < MapWidth; x++)
            for (var y = 0; y < MapHeight; y++)
            {
                //var cell = Map.BlockingGrid.GetNode(x, y);
                //        //if (!cell.Incoming.Any(e => e.IsConnected))
                //        //    SpriteBatch.FillRectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight, Color.Red);
                //        //if (!cell.Outgoing.Any(e => e.IsConnected))
                //        //    SpriteBatch.FillRectangle(x * TileWidth + TileWidth / 2, y * TileHeight, TileWidth / 2, TileHeight, Color.Green);

                //        //if (flowField != null)
                //        //{
                //        //    var flow = flowField.CostMap[x, y];
                //        //    var startX = x * TileWidth + TileWidth / 2;
                //        //    var startY = y * TileHeight + TileHeight / 2;
                //        //    if (flow.Valid)
                //        //    {
                //        //        if (flow.CostValue < flowField.MaxCost)
                //        //            SpriteBatch.FillRectangle(x * TileWidth, y * TileWidth, TileHeight, TileWidth,
                //        //                GraphicsExtensions.ColorGradientPercentage(Color.White, Color.Black, flow.CostValue / flowField.MaxCost));
                //        //        var endX = startX + (flow.NextX - x) * 4;
                //        //        var endY = startY + (flow.NextY - y) * 4;
                //        //        SpriteBatch.DrawLine(startX, startY, endX, endY, Color.DarkRed);
                //        //        SpriteBatch.DrawLine(endX, endY, endX, endY, Color.Red);
                //        //    }
                //        //    else
                //        //    {
                //        //        SpriteBatch.DrawCircle(startX, startY, 4, 10, Color.Yellow);
                //        //    }
                //        //}
                //var color = Map.ReservationMap[x, y] > 0 ? Color.Red : (Color?)null;
                
                //if (color.HasValue)
                //    SpriteBatch.FillRectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight, color.Value);
            }

        var mouseCell = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        mouseCell = new Vector2((int)mouseCell.X / TileWidth * TileWidth, (int)mouseCell.Y / TileHeight * TileHeight);
        SpriteBatch.DrawRectangle(mouseCell, new(TileWidth, TileHeight), Color.AliceBlue);

        foreach (var unit in Map.UnitLayer.Objects.Cast<Unit>())
        {
            var prevCell = new Vector2(unit.Position.X + TileWidth / 2, unit.Position.Y + TileHeight / 2);
            SpriteBatch.DrawCircle(prevCell, TileWidth / 2, 25, Color.Green);
            //if (unit.State != Unit.States.Walk)
            //    continue;
            foreach (var cell in unit.MoveQueue)
            {
                var nextCell = new Vector2(cell.X * TileWidth + TileWidth / 2, cell.Y * TileHeight + TileHeight / 2);
                SpriteBatch.DrawLine(prevCell, nextCell, Color.Green);
                prevCell = nextCell;
            }
        }

        SpriteBatch.End();
    }

    protected override bool PreMapDraw(GameTime gameTime)
    {
        DrawUnderUnits();
        return true;
    }

    protected override void PostDraw(GameTime gameTime)
    {
        var mouseCell = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        var mousePoint = new Point((int)mouseCell.X / TileWidth, (int)mouseCell.Y / TileHeight);
        var cost = Map.CellIsInBounds(mousePoint) ? flowField?.CostMap[mousePoint.X, mousePoint.Y].CostValue*10 ?? 0 : -1;
        SpriteBatch.Begin();
        SpriteBatch.DrawStringShadow(font, $"FPS {FramesPerSecondCounter.FramesPerSecond} - Zoom: {MainCamera.Zoom} - {mousePoint} costs {cost}", new(4, 4), Color.White);
        SpriteBatch.End();
    }
}
