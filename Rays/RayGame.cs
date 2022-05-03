using Merthsoft.Moose.Rays.Actors;
using Merthsoft.Moose.Rays.Serialization;
using System.Text.Json;

namespace Merthsoft.Moose.Rays;

public class RayGame : MooseGame
{
    public static new RayGame Instance = null!;

    public RayMap RayMap = null!;
    public RayPlayer Player = null!;

    public Texture2D WeaponTexture = null!;

    public BasicEffect Effect = null!;

    public static bool Busy;

    public SpriteFont Font30 = null!;
    public SpriteFont Font50 = null!;

    Texture2D WallTexture = null!;
    public int WallCount;

    Texture2D FloorTexture = null!;
    public int FloorCount;

    Texture2D DoorTexture = null!;
    public int DoorCount;

    Texture2D StaticObjectsTexture = null!;
    public int StaticObjectCount;
    
    public int ActorFrameCount = 0;

    public Texture2D TextureAtlas = null!;
    public Texture2D UxTexture = null!;

    Definitions Definitions;
    
    public RayGame()
    {
        Instance = this;
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        DefaultBackgroundColor = Color.Black,
        ScreenHeight = 480*2,
        ScreenWidth = 640*2,
        IsMouseVisible = false
    };

    protected override void Load()
    {
        WallTexture = ContentManager.LoadImage("Walls");
        WallCount = WallTexture.Width / 16;

        DoorTexture = ContentManager.LoadImage("Doors");
        DoorCount = DoorTexture.Width / 16;

        StaticObjectsTexture = ContentManager.LoadImage("Objects");
        StaticObjectCount = StaticObjectsTexture.Width / 16;
        
        UxTexture = ContentManager.LoadImage("UI");

        Effect = new(GraphicsDevice)
        {
            Alpha = 1,
            TextureEnabled = true,
            VertexColorEnabled = true,
            FogEnabled = true,
            FogStart = 32,
            FogEnd = 400
        };

        AddRenderer("walls", new Ray3DRenderer(GraphicsDevice, Effect));

        AddDef(new RayPlayerDef());

        WeaponTexture = ContentManager.LoadImage("Weapons");
        
        AddDef(new WeaponDef("knife", 0, 0, Keys.D1, 5, new() { 3 }, Weapon.KnifeAttack));
        AddDef(new WeaponDef("pistol", 0, 1, Keys.D2, 5, new() { 2 }, Weapon.RayAttack));
        AddDef(new WeaponDef("machine-gun", 94, 2, Keys.D3, 5, new() { 2 }, Weapon.RayAttack));
        AddDef(new WeaponDef("chain-gun", 95, 3, Keys.D4, 5, new() { 2, 3 }, Weapon.RayAttack));
        AddDef(new WeaponDef("rocket-launcher", 107, 4, Keys.D5, 5, new() { 2 }, Weapon.Rocket));

        AddDef(new RocketDef());

        AddDef(new DoorDef());
        AddDef(new SecretWallDef());
        AddDef(new ElevatorDef());

        var guardDef = AddDef(new ActorDef("Guard")
        {
            Health = 2,
            States =
            {
                { ActorStates.StandState, new() { new(0, 250, Shootable: true) { NextState = ActorStates.StandState } } },
                { ActorStates.HitState,  new () { new (42, 100, RenderMode: ObjectRenderMode.Sprite, EndAction: Actor.PostHit) } },
                { ActorStates.DyingState,  new () { new (43, 100, RenderMode: ObjectRenderMode.Sprite),
                                            new(44, 100, RenderMode: ObjectRenderMode.Sprite),
                                            new(45, 100, RenderMode: ObjectRenderMode.Sprite) { NextState = ActorStates.DeadState } } },
                { ActorStates.DeadState, new() { new(46, RenderMode: ObjectRenderMode.Sprite, Blocking: false) } },
                { ActorStates.ShootState, new() { new(0, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite),
                                            new(40, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite),
                                            new(41, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite),
                                            new(40, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite) { NextState = ActorStates.ChaseState } } },
                { ActorStates.ChaseState, new() {new(8, 200, Shootable: true),
                                            new(9, 200, Shootable: true),
                                            new(10, 200, Shootable: true),
                                            new(11, 200, Shootable: true) { NextState = ActorStates.ChaseState } } }
            }
        });

        AddDef(new ActorDef("SS")
        {
            Health = 4,
            States = guardDef.States
        });

        AddDef(new ActorDef("Officer")
        {
            Health = 5,
            States = guardDef.States
        });

        RayMap = new();
        ActiveMaps.Add(RayMap);

        Font30 = ContentManager.BakeFont("Tomorrow_Night", 30);
        Font50 = ContentManager.BakeFont("Tomorrow_Night", 50);

        Definitions = JsonSerializer.Deserialize<Definitions>(File.ReadAllText("Definitions.json"))!;
        foreach (var objDef in Definitions.Objects)
            AddDef(new RayGameObjectDef(objDef));
    }

    public SecretWall SpawnSecretWall(int x, int y)
    {
        var wall = RayMap.WallLayer.GetTileValue(x, y);
        RayMap.WallLayer.SetTileValue(x, y, -1);
        return AddObject(new SecretWall(GetDef<SecretWallDef>()!, wall, x, y));
    }
    
    public Actor SpawnActor(string actor, int x, int y, Vector3 facing)
        => AddObject(new Actor(GetDef<ActorDef>(actor), x, y) { FacingDirection = facing });

    public Door SpawnDoor(string doorName, int x, int y)
    {
        var doorDefinition = Definitions.Doors.First(d => d.Name == doorName);
        var horizontal = RayMap.WallLayer.GetTileValue(x - 1, y) > 0;
        RayMap.WallLayer.SetTileValue(x, y, -1);
        return AddObject(new Door(GetDef<DoorDef>()!, x, y, horizontal, WallCount + doorDefinition.Index));
    }

    public RayGameObject SpawnStatic(string objectName, int x, int y)
        => AddObject(new RayGameObject(GetDef<RayGameObjectDef>(objectName), x, y));

    public RayGameObject SpawnStaticOverlay(int drawIndex, int x, int y)
        => AddObject(new RayGameObject(GetDef<RayGameObjectDef>("static-object"), x, y)
        {
            TextureIndex = drawIndex,
            ObjectRenderMode = ObjectRenderMode.Wall
        });

    public Elevator SpawnElevator(bool up, int x, int y)
        => AddObject(new Elevator(GetDef<ElevatorDef>()!, up, x, y));

    public Missile SpawnRocket(int x, int y, Vector3 facing)
        => AddObject(new Missile(GetDef<RocketDef>()!, x, y) { MoveDirection = facing });

    protected override void PostLoad()
    {
        base.PostLoad();

        ActorFrameCount = 0;
        foreach (var actorDef in GetDefs<ActorDef>())
        {
            if (actorDef.ObjectRenderMode is ObjectRenderMode.Door or ObjectRenderMode.Wall)
                continue;
            actorDef.DefaultTextureIndex = WallCount + DoorCount + StaticObjectCount + ActorFrameCount;
            ActorFrameCount += actorDef.FrameCount;
        }

        var textureWidth = 16 * (WallCount + DoorCount + StaticObjectCount + ActorFrameCount);
        TextureAtlas = new Texture2D(GraphicsDevice, textureWidth, 16);

        TextureAtlas.Draw(WallTexture, WallTexture.Bounds);
        TextureAtlas.Draw(DoorTexture, new(WallCount * 16, 0, DoorCount * 16, 16));
        TextureAtlas.Draw(StaticObjectsTexture, new((WallCount + DoorCount) * 16, 0, StaticObjectCount * 16, 16));

        ActorFrameCount = 0;
        foreach (var actorDef in GetDefs<ActorDef>())
        {
            if (actorDef.ObjectRenderMode is ObjectRenderMode.Door or ObjectRenderMode.Wall)
                continue;
            TextureAtlas.Draw(actorDef.Texture, new(16 * actorDef.DefaultTextureIndex, 0, actorDef.Texture.Width, 16));
            ActorFrameCount += actorDef.FrameCount;
        }
        Effect.Texture = TextureAtlas;

        Mouse.SetPosition(ScreenWidth / 2, ScreenHeight / 2);

        Player = AddObject(new RayPlayer(GetDef<RayPlayerDef>()!, 3, 52) { FacingDirection = Vector3.Down });

        Player.Weapons.Add(GetDef<WeaponDef>("knife"));
        Player.Weapons.Add(GetDef<WeaponDef>("pistol"));

        Player.CurrentWeapon = Player.Weapons[0];


        var options = new JsonSerializerOptions { DefaultBufferSize = int.MaxValue };
        var mapFile = JsonSerializer.Deserialize<SaveMap>(File.ReadAllText("Content/Maps/testmap.json"))!;
        RayMap.Load(this, Definitions, mapFile);
    }

    public void SetPlayerPosition(int x, int y)
        => Player.Position = new(x * 16 + 8, y * 16 + 8);
    
    public void SetPlayerFacing(Vector3 vector3) 
        => Player.FacingDirection = vector3;

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        DrawWeapon();
        DrawStatusBar();

        var fov = MathHelper.ToRadians(50);
        SpriteBatch.DrawString(Font30, $"FPS: {FramesPerSecondCounter.FramesPerSecond}", new(2, 2), Color.Black);
        SpriteBatch.DrawString(Font30, $"FPS: {FramesPerSecondCounter.FramesPerSecond}", new(3, 3), Color.DarkGray);

        SpriteBatch.End();
    }

    void DrawWeapon()
    {
        var x = ScreenWidth / 2 - 160;
        var y = ScreenHeight - 470;

        if (Player.CurrentWeapon != null)
            SpriteBatch.Draw(WeaponTexture,
                destinationRectangle: new(x, y, 320, 320),
                sourceRectangle: new(Player.AttackFrame * 32, Player.CurrentWeapon.TextureRow * 32, 32, 32),
                Color.White);
    }

    void DrawStatusBar()
    {
        DrawLevel();
        DrawHealth();
        DrawKeys();
        DrawFace();
        DrawAmmo();
        DrawWeapons();
    }

    public void DrawText(SpriteFont font, string text, int x, int y, Color color, Color? highlightColor = null)
    {
        if (highlightColor != null)
            SpriteBatch.DrawString(font, text, new(x - 1, y - 1), highlightColor.Value);
        SpriteBatch.DrawString(font, text, new(x, y), color);
    }

    public void CenterText(SpriteFont font, string text, int x, int y, int width, int height, Color color, Color? highlightColor = null)
    {
        var stringSie = font.MeasureString(text);
        x = x + width / 2 - (int)stringSie.X / 2;
        y = y + height / 2 - (int)stringSie.Y / 2;
        DrawText(font, text, x, y, color, highlightColor);
    }

    private void DrawLevel()
    {
        var x = 0;
        var y = ScreenHeight - 150;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 150, 150),
            sourceRectangle: new(8, 144, 24, 24),
            Color.White);
        DrawText(Font30, "Floor", x + 15, y + 10, Color.Black, Color.DarkGray);
        CenterText(Font50, "01", x, y + 5, 150, 150, Color.Black, Color.DarkGray);
    }

    private void DrawHealth()
    {
        var x = 150;
        var y = ScreenHeight - 150;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 150, 150),
            sourceRectangle: new(8, 48, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 150, 50),
            sourceRectangle: new(8, 40, 24, 8),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x + 55, y, 40, 40),
            sourceRectangle: new(8, 176, 8, 8),
            Color.White);

        CenterText(Font50, Player.Health.ToString(), x, y + 65, 150, 25, Color.Black, Color.DarkGray);
        CenterText(Font50, Player.MaxHealth.ToString(), x, y + 95, 150, 41, Color.Black, Color.DarkGray);
    }

    private void DrawKeys()
    {
        var x = 300;
        var y = ScreenHeight - 150;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 150, 150),
            sourceRectangle: new(8, 48, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 150, 50),
            sourceRectangle: new(8, 40, 24, 8),
            Color.White);

        CenterText(Font30, "Keys", x, y, 150, 50, Color.Black, Color.DarkGray);
        
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y + 80, 50, 50),
            sourceRectangle: new(24, 176, 8, 8),
            Color.White);

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x + 45, y + 80, 50, 50),
            sourceRectangle: new(24, 176, 8, 8),
            Color.White);

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x + 90, y + 80, 50, 50),
            sourceRectangle: new(24, 176, 8, 8),
            Color.White);
    }

    private void DrawAmmo()
    {
        var x = 450;
        var y = ScreenHeight - 150;

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 115, 150),
            sourceRectangle: new(8, 48, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 115, 50),
            sourceRectangle: new(8, 32, 24, 8),
            Color.White);

        CenterText(Font50, "-", x, y + 50, 115, 100, Color.Black, Color.DarkGray);
    }

    private void DrawFace()
    {
        var x = ScreenWidth / 2 - 75;
        var y = ScreenHeight - 150;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 150, 150),
            sourceRectangle: new(8, 0, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 150, 150),
            sourceRectangle: new(32 + 24 * Player.FaceIndex, 96 + 24 * Player.HealthIndex, 24, 24),
            Color.White);
    }
    private void DrawWeapons()
    {
        var x = ScreenWidth / 2 + 75;
        var y = ScreenHeight - 150;
        var width = ScreenWidth / 2 - 75;

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 50, 150),
            sourceRectangle: new(8, 0, 8, 24),
            Color.White);

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x + 50, y, width - 100, 150),
            sourceRectangle: new(16, 0, 8, 24),
            Color.White);

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x + width - 50, y, 50, 150),
            sourceRectangle: new(24, 0, 8, 24),
            Color.White);

        //CenterText(Font30, "Weapons", x, y + 5, 170, 33, Color.DarkGray, Color.Black);
        x += 15;
        y += 68;
        var weapons = GetDefs<WeaponDef>().ToList();
        for (var weaponIndex = 0; weaponIndex < weapons.Count; weaponIndex++)
        {
            var weapon = weapons[weaponIndex];

            SpriteBatch.Draw(UxTexture,
                destinationRectangle: new(x + 120 * weaponIndex, y, 120, 144),
                sourceRectangle: new(8 + 24 * weaponIndex, 192, 24, 24),
                Player.Weapons.Contains(weapon)
                    ? Player.CurrentWeapon == weapon
                        ? Color.White
                        : Color.Gray
                    : Color.Black, MathF.PI / 2, new(12, 16), SpriteEffects.None, 1);

        }
    }
}