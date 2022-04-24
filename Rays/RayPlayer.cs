using Merthsoft.Moose.Rays.Actors;

namespace Merthsoft.Moose.Rays;

public record RayPlayerDef() : RayGameObjectDef("player", 0, ObjectRenderMode.Directional) { }
public class RayPlayer : RayGameObject
{
    public static RayPlayer Instance = null!;

    public RayPlayerDef RayPlayerDef;

    public List<WeaponDef> Weapons = new();
    public WeaponDef? CurrentWeapon;

    public int AttackFrame = 0;
    public double AttackTime = 0;
    public bool AttackedThisFrame;

    public bool Busy;

    public int Health = 5;
    public int MaxHealth = 10;
    public float HealthPercent => (float)Health / MaxHealth;
    public int HealthIndex => HealthPercent switch
    {
        > .9f => 0,
        > .65f => 1,
        > .35f => 2,
        _ => 3,
    };

    public int FaceIndex = 0;
    public int FaceTick = 0;

    public RayPlayer(RayPlayerDef def, int x, int y) : base(def, x, y)
    {
        Instance = this;
        RayPlayerDef = def;
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch) { }
    public override void Update(MooseGame game, GameTime gameTime)
    {
        var rayGame = (game as RayGame)!;
        var actors = VisibleActors().ToList();

        if (FaceTick <= 0)
        {
            var r = game.Random.NextSingle();
            FaceIndex = r switch
            {
                > .8f => 1,
                < .2f => 2,
                _ => 0,
            };
            FaceTick = game.Random.Next(25, 70);
        }
        else
            FaceTick--;

        UpdateAttack(gameTime, rayGame, actors);

        if (Busy)
            return;

        if (rayGame.WasKeyJustPressed(Keys.Space))
        {
            var checkCell3 = PositionIn3dSpace + 16 * FacingDirection;
            var checkCell = new Point((int)(checkCell3.X / 16), (int)(checkCell3.Y / 16));
            var actor = rayGame.ReadObjects.FirstOrDefault(a => a.GetCell() == checkCell);
            (actor as Actor)?.Interact();
            return;
        }

        var moveVector = Vector3.Zero;

        if (rayGame.WasKeyJustPressed(Keys.Up, Keys.W))
            moveVector = FacingDirection;

        if (rayGame.WasKeyJustPressed(Keys.Down, Keys.S))
            moveVector = -FacingDirection;

        if (rayGame.WasKeyJustPressed(Keys.Left, Keys.A))
        {
            if (rayGame.IsKeyDown(Keys.LeftAlt, Keys.RightAlt))
                moveVector = Vector3.Transform(FacingDirection, Matrix.CreateRotationZ(-MathF.PI / 2));
            else
                FacingDirection = Vector3.Transform(FacingDirection, Matrix.CreateRotationZ(-MathF.PI / 4));
        }

        if (rayGame.WasKeyJustPressed(Keys.Right, Keys.D))
        {
            if (rayGame.IsKeyDown(Keys.LeftAlt, Keys.RightAlt))
                moveVector = Vector3.Transform(FacingDirection, Matrix.CreateRotationZ(MathF.PI / 2));
            else
                FacingDirection = Vector3.Transform(FacingDirection, Matrix.CreateRotationZ(MathF.PI / 4));
        }

        if (AttackFrame == 0)
            foreach (var weapon in Weapons)
                if (rayGame.IsKeyDown(weapon.Key))
                {
                    CurrentWeapon = weapon;
                    break;
                }

        var moveX = 16 * moveVector.X.Round();
        var moveY = 16 * moveVector.Y.Round();

        //if (rayGame.MainMap.GetBlockingVector(new(Position.X + 4*moveX, Position.Y + 4*moveY))[1] == 0)
        Position = new(Position.X + moveX, Position.Y + moveY);

        Position.Round();

        //var diff = rayGame.ScreenWidth / 2 - rayGame.CurrentMouseState.X;
        //if (rayGame.CurrentMouseState != rayGame.PreviousMouseState)
        //{
        //    var rot = Matrix.CreateRotationZ(diff * MathF.PI / 360f);
        //    FacingDirection = Vector3.Transform(FacingDirection, rot);
        //}

        //if (rayGame.IsActive)
        //    Mouse.SetPosition(rayGame.ScreenWidth / 2, rayGame.ScreenHeight / 2);
    }

    private void UpdateAttack(GameTime gameTime, RayGame rayGame, List<Actor> actors)
    {
        if (CurrentWeapon != null && AttackFrame > 0)
        {
            AttackTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (AttackTime > 100)
            {
                AttackFrame++;
                AttackTime = 0;
                AttackedThisFrame = false;
            }
            if (AttackFrame >= CurrentWeapon.NumFrames)
            {
                AttackFrame = 0;
                Busy = false;
            }
            if (CurrentWeapon.AttackFrames.Contains(AttackFrame) && !AttackedThisFrame)
            {
                var enemy = actors.FirstOrDefault(a => a.Shootable);
                enemy?.TakeDamage(1);
                AttackedThisFrame = true;
            }
        }
        else if (CurrentWeapon != null && rayGame.WasKeyJustPressed(Keys.LeftControl, Keys.RightControl))
        {
            AttackFrame = 1;
            AttackTime = 0;
            Busy = true;
        }
    }

    public IEnumerable<Actor> VisibleActors()
    {
        var parentMap = (ParentMap as RayMap)!;

        var playerRotation = -MathF.Atan2(FacingDirection.Y, FacingDirection.X);
        var matrix = Matrix.CreateRotationZ(playerRotation);
        var fov = MathHelper.ToRadians(50);

        foreach (var obj in RayGame.Instance.ReadObjects.OfType<Actor>().OrderBy(o => o.DistanceSquaredTo(this)))
        {
            if (obj is Door)
                continue;

            var obscured = false;

            var relative = Vector2.Transform(obj.Position - Position, matrix);
            var rads = MathF.Atan2(relative.Y, relative.X);
            if (rads > fov || rads < -fov)
                obscured = true;
            else
            {
                var ray = Position.CastRay(obj.Position, false, true);
                foreach (var cell in ray)
                {
                    if (parentMap.WallLayer.GetTileValue((int)(cell.X / 16), (int)(cell.Y / 16)) > 0)
                    {
                        obscured = true;
                        break;
                    }
                }
            }
            if (!obscured)
                yield return obj;
        }
    }
}
