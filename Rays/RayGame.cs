using Merthsoft.Moose.Rays.Actors;
using Merthsoft.Moose.Rays.Serialization;
using System.Globalization;
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
    Texture2D StaticObjectsTexture = null!;
    public int StaticObjectCount;
    Texture2D DoorTexture = null!;
    public int DoorCount;

    public int ActorFrameCount = 0;

    public Texture2D TextureAtlas = null!;
    public Texture2D UxTexture = null!;
    public MapFile MapFile = null!;

    public RayGame()
    {
        Instance = this;
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        DefaultBackgroundColor = Color.Black,
        ScreenHeight = 480,
        ScreenWidth = 640,
        IsMouseVisible = false
    };

    protected override void Load()
    {
        WallTexture = ContentManager.LoadImage("Walls");
        WallCount = WallTexture.Width / 16;
        StaticObjectsTexture = ContentManager.LoadImage("Objects");
        StaticObjectCount = StaticObjectsTexture.Width / 16;
        DoorTexture = ContentManager.LoadImage("Doors");
        DoorCount = DoorTexture.Width / 16;

        UxTexture = ContentManager.LoadImage("UI");

        Effect = new(GraphicsDevice);
        Effect.Alpha = 1;
        Effect.TextureEnabled = true;
        Effect.VertexColorEnabled = true;
        Effect.FogEnabled = true;
        Effect.FogStart = 32;
        Effect.FogEnd = 400;

        AddRenderer("walls", new Ray3DRenderer(GraphicsDevice, Effect));

        AddDef(new RayPlayerDef());

        WeaponTexture = ContentManager.LoadImage("Weapons");

        AddDef(new TreasureDef(100, "chest", 68));
        AddDef(new RayGameObjectDef("static-object", 0, ObjectRenderMode.Sprite));

        AddDef(new WeaponDef("knife", 0, 0, Keys.D1, 5, new() { 3 }));
        AddDef(new WeaponDef("pistol", 0, 1, Keys.D2, 5, new() { 2 }));
        AddDef(new WeaponDef("machine-gun", 94, 2, Keys.D3, 5, new() { 2 }));
        AddDef(new WeaponDef("chain-gun", 95, 3, Keys.D4, 5, new() { 2, 3 }));
        AddDef(new WeaponDef("rocket-launcher", 107, 4, Keys.D5, 5, new() { 2 }));

        AddDef(new DoorDef());
        AddDef(new SecretWallDef());
        AddDef(new ElevatorDef());

        AddDef(new BlinkingLightDef());

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
                { ActorStates.DeadState, new() { new(46, RenderMode: ObjectRenderMode.Sprite) } },
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

        var options = new JsonSerializerOptions { DefaultBufferSize = int.MaxValue};
        MapFile = JsonSerializer.Deserialize<MapFile>(File.ReadAllText("Content/Maps/Map2.json"))!;
    }

    public SecretWall SpawnSecretWall(int wall, int x, int y) 
        => AddObject(new SecretWall(GetDef<SecretWallDef>()!, wall, x, y));
    public Actor SpawnActor(string actor, int x, int y, Vector3 facing)
        => AddObject(new Actor(GetDef<ActorDef>(actor), x, y) { FacingDirection = facing });

    public Door SpawnDoor(int x, int y, bool horizontal, int tile)
        => AddObject(new Door(GetDef<DoorDef>()!, x, y, horizontal, tile));

    public RayGameObject SpawnStatic(int drawIndex, int x, int y)
        => AddObject(new RayGameObject(GetDef<RayGameObjectDef>("static-object"), x, y) { TextureIndex = drawIndex });

    public RayGameObject SpawnStaticOverlay(int drawIndex, int x, int y)
        => AddObject(new RayGameObject(GetDef<RayGameObjectDef>("static-object"), x, y)
        {
            TextureIndex = drawIndex,
            ObjectRenderMode = ObjectRenderMode.Wall
        });

    public Elevator SpawnElevator(bool up, int x, int y)
        => AddObject(new Elevator(GetDef<ElevatorDef>()!, up, x, y));

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
        Player.Weapons.Add(GetDef<WeaponDef>("machine-gun"));
        Player.Weapons.Add(GetDef<WeaponDef>("chain-gun"));
        Player.Weapons.Add(GetDef<WeaponDef>("rocket-launcher"));

        Player.CurrentWeapon = Player.Weapons[1];

        RayMap.Load(this, MapFile);
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
        if (Player.CurrentWeapon != null)
            SpriteBatch.Draw(WeaponTexture,
                destinationRectangle: new(240, ScreenHeight - 260, 160, 160),
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
        var y = ScreenHeight - 100;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 100, 100),
            sourceRectangle: new(8, 144, 24, 24),
            Color.White);
        DrawText(Font30, "Floor", x + 9, y + 7, Color.Black, Color.DarkGray);
        CenterText(Font50, "01", x, y + 15, 100, 100, Color.Black, Color.DarkGray);
    }

    private void DrawHealth()
    {
        var x = 100;
        var y = ScreenHeight - 100;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 100, 100),
            sourceRectangle: new(8, 48, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 100, 33),
            sourceRectangle: new(8, 40, 24, 8),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x + 33, y - 1, 32, 32),
            sourceRectangle: new(8, 176, 8, 8),
            Color.White);

        CenterText(Font50, "100", x, y + 15, 100, 100, Color.Black, Color.DarkGray);
    }

    private void DrawKeys()
    {
        var x = 200;
        var y = ScreenHeight - 100;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 70, 100),
            sourceRectangle: new(8, 48, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 70, 33),
            sourceRectangle: new(8, 40, 24, 8),
            Color.White);

        CenterText(Font30, "Keys", x, y + 2, 70, 33, Color.Black, Color.DarkGray);

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y + 60, 33, 33),
            sourceRectangle: new(24, 176, 8, 8),
            Color.White);

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x + 33, y + 60, 33, 33),
            sourceRectangle: new(24, 176, 8, 8),
            Color.White);
    }

    private void DrawAmmo()
    {
        var x = ScreenWidth / 2 + 50;
        var y = ScreenHeight - 100;

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 100, 100),
            sourceRectangle: new(8, 48, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 100, 33),
            sourceRectangle: new(8, 32, 24, 8),
            Color.White);

        CenterText(Font50, "100", x, y + 15, 100, 100, Color.Black, Color.DarkGray);
    }

    private void DrawFace()
    {
        var x = ScreenWidth / 2 - 50;
        var y = ScreenHeight - 100;
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 100, 100),
            sourceRectangle: new(8, 0, 24, 24),
            Color.White);
        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 100, 100),
            sourceRectangle: new(32 + 24 * Player.FaceIndex, 96 + 24 * Player.HealthIndex, 24, 24),
            Color.White);
    }
    private void DrawWeapons()
    {
        var x = ScreenWidth / 2 + 150;
        var y = ScreenHeight - 100;

        SpriteBatch.Draw(UxTexture,
            destinationRectangle: new(x, y, 170, 100),
            sourceRectangle: new(8, 112, 24, 24),
            Color.White);
        CenterText(Font30, "Weapons", x, y + 5, 170, 33, Color.DarkGray, Color.Black);

    }
}