namespace Merthsoft.Moose.Rays;
public record RayPlayerDef() : RayGameObjectDef("player", 0, ObjectRenderMode.Directional) { }
public class RayPlayer : RayGameObject
{
    public static RayPlayer Instance = null!;

    public RayPlayerDef RayPlayerDef;

    public Weapon CurrentWeapon = Weapon.ChainGun;
    public bool[] HasWeapon = new[] {true, true, false, false};

    public int AttackFrame = 0;

    public RayPlayer(RayPlayerDef def, int x, int y) : base(def, x, y)
    {
        Instance = this;
        RayPlayerDef = def;
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch) { }
    public override void Update(MooseGame game, GameTime gameTime) 
    {
        var rayGame = (game as RayGame)!;

        var moveVector = Vector3.Zero;

        if (rayGame.WasKeyJustPressed(Keys.Up, Keys.W))
            moveVector = FacingDirection;

        if (rayGame.WasKeyJustPressed(Keys.Down, Keys.S))
            moveVector = -FacingDirection;

        if (rayGame.WasKeyJustPressed(Keys.Left, Keys.A))
        {
            var rot = Matrix.CreateRotationZ(-MathF.PI / 4);
            FacingDirection = Vector3.Transform(FacingDirection, rot);
        }

        if (rayGame.WasKeyJustPressed(Keys.Right, Keys.D))
        {
            var rot = Matrix.CreateRotationZ(MathF.PI / 4);
            FacingDirection = Vector3.Transform(FacingDirection, rot);
        }

        var moveX = 16*moveVector.X.Round();
        var moveY = 16*moveVector.Y.Round();

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
}
