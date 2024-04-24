using Merthsoft.Moose.MooseEngine.PathFinding.Maps;
using Merthsoft.Moose.Rays.Actors;

namespace Merthsoft.Moose.Rays;

public record RayPlayerDef() : RayGameObjectDef("player", 0, ObjectRenderMode.Directional, false) { }
public class RayPlayer : RayGameObject
{
    public static RayPlayer Instance = null!;

    public RayPlayerDef RayPlayerDef;

    public List<WeaponDef> Weapons = [];
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

        if (FaceTick <= 0)
        {
            var r = game.RandomSingle();
            FaceIndex = r switch
            {
                > .8f => 1,
                < .2f => 2,
                _ => 0,
            };
            FaceTick = game.RandomInt(25, 70);
        }
        else
            FaceTick--;

        UpdateAttack(gameTime, rayGame);

        if (Busy)
            return;

        if (rayGame.WasKeyJustPressed(Keys.Space))
        {
            var checkCell3 = PositionIn3dSpace + 16 * FacingDirection;
            var checkCell = new Point((int)(checkCell3.X / 16), (int)(checkCell3.Y / 16));
            var actor = rayGame.ReadObjects.FirstOrDefault(a => a.Cell == checkCell);
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

        if (moveVector != Vector3.Zero)
        {
            var moveX = 64 * moveVector.X.Round();
            var moveY = 64 * moveVector.Y.Round();
            
            if ((rayGame.MainMap as PathFinderMap)?.GetBlockingVector((int)Position.X + moveX, (int)Position.Y + moveY).All(i => i <= 0) ?? false)
                Position = new(Position.X + moveX, Position.Y + moveY);

            Position.Round();
        }
        //var diff = rayGame.ScreenWidth / 2 - rayGame.CurrentMouseState.X;
        //if (rayGame.CurrentMouseState != rayGame.PreviousMouseState)
        //{
        //    var rot = Matrix.CreateRotationZ(diff * MathF.PI / 360f);
        //    FacingDirection = Vector3.Transform(FacingDirection, rot);
        //}

        //if (rayGame.IsActive)
        //    Mouse.SetPosition(rayGame.ScreenWidth / 2, rayGame.ScreenHeight / 2);
    }

    private void UpdateAttack(GameTime gameTime, RayGame rayGame)
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
                CurrentWeapon.Attack(this);
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
}
