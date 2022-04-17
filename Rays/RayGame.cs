using System.Globalization;

namespace Merthsoft.Moose.Rays;

public class RayGame : MooseGame
{
    public static new RayGame Instance = null!;

    public RayMap RayMap = null!;
    public RayPlayer Player = null!;

    public Texture2D WeaponTexture = null!;

    public BasicEffect Effect = null!;

    public static bool Busy;

    public SpriteFont Font = null!;

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
        Effect = new(GraphicsDevice);
        Effect.Alpha = 1;
        Effect.TextureEnabled = true;
        Effect.Texture = ContentManager.LoadImage("Textures");
        Effect.VertexColorEnabled = true;
        Effect.FogEnabled = true;
        Effect.FogStart = 64;
        Effect.FogEnd = 800;

        AddRenderer("walls", new Ray3DRenderer(GraphicsDevice, Effect));

        AddDef(new RayPlayerDef());

        WeaponTexture = ContentManager.LoadImage("Weapons");

        AddDef(new TreasureDef(100, "chest", 68));
        AddDef(new RayGameObjectDef("pillar", 72, ObjectRenderMode.Sprite));
        AddDef(new RayGameObjectDef("armor", 74, ObjectRenderMode.Sprite));
        AddDef(new RayGameObjectDef("ceiling-light", 100, ObjectRenderMode.Sprite));

        AddDef(new WeaponDef("knife", 0, 0, Keys.D1, 5, new() { 3 }));
        AddDef(new WeaponDef("pistol", 0, 1, Keys.D2, 5, new() { 2 }));
        AddDef(new WeaponDef("machine-gun", 94, 2, Keys.D3, 5, new() { 2 }));
        AddDef(new WeaponDef("chain-gun", 95,3, Keys.D4, 5, new() { 2, 3 }));
        AddDef(new WeaponDef("rocket-launcher", 107, 4, Keys.D5, 5, new() { 2 }));

        AddDef(new DoorDef());

        AddDef(new ActorDef("guard", 124)
        {
            Health = 5,
            States =
            {
                { ActorStates.StandState, new(0, 250, Shootable: true) { NextState = ActorStates.StandState } },
                { ActorStates.HitState,  new(42, 100, RenderMode: ObjectRenderMode.Sprite, EndAction: Actor.PostHit) },
                { ActorStates.DyingState,  new(43, 100, RenderMode: ObjectRenderMode.Sprite) {
                                        Next = new(44, 100, RenderMode: ObjectRenderMode.Sprite) {
                                        Next = new(45, 100, RenderMode: ObjectRenderMode.Sprite) {
                                        Next = new (46, 100, RenderMode: ObjectRenderMode.Sprite) {
                                        NextState = ActorStates.DeadState
                                        } }} } },
                { ActorStates.DeadState, new(47, 1000, RenderMode: ObjectRenderMode.Sprite) { NextState = ActorStates.StandState } },
                { ActorStates.ShootState, new(0, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite) {
                                        Next = new(40, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite) {
                                        Next = new(41, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite) {
                                        Next = new(40, 250, Shootable: true, RenderMode: ObjectRenderMode.Sprite) {
                                        NextState = ActorStates.ChaseState
                                        } } } } },
                { ActorStates.ChaseState, new(8, 200, Shootable: true) {
                                        Next = new(16, 200, Shootable: true) {
                                        Next = new(24, 200, Shootable: true) {
                                        Next = new(32, 200, Shootable: true) {
                                        NextState = ActorStates.ChaseState
                                        }} } } },
            }
        });

        RayMap = new();
        ActiveMaps.Add(RayMap);

        Font = ContentManager.BakeFont("Tomorrow_Night", 30);
    }

    protected override void PostLoad()
    {
        base.PostLoad();

        Mouse.SetPosition(ScreenWidth / 2, ScreenHeight / 2);

        Player = AddObject(new RayPlayer(GetDef<RayPlayerDef>()!, 57, 35) { FacingDirection = Vector3.Down });

        Player.Weapons.Add(GetDef<WeaponDef>("knife"));
        Player.Weapons.Add(GetDef<WeaponDef>("pistol"));
        Player.Weapons.Add(GetDef<WeaponDef>("machine-gun"));
        Player.Weapons.Add(GetDef<WeaponDef>("chain-gun"));
        Player.Weapons.Add(GetDef<WeaponDef>("rocket-launcher"));

        Player.CurrentWeapon = Player.Weapons[1];

        var map = 
"""
1111111111111111112121121111232111112141121221221111111111111111
11111111111111111122   1                         211111111111111
11111111111111111121   |                         211111111111111
11111111111111111122   1                         111111111111111
11111111111111111111121112 11311   1114211 212   211111111111111
1111111111111123121311111   21221Z1111111   11   211111111111111
111111111    2       2112   1         211   22   111111111111111
111111111    6       6111   2         111   11   211111111111111
111111111    1       11311321         11213212   211111111111111
111111111    2       1      1         2          111111111111111
11111111111  4       |      |         |          41 111111111111
11111111111112       1      1         1          136311111111111
11111111111111       12321311         11213221   1   11111111111
11111111111116       61111112         21111112   1   1  11111111
11111111111111       21111111         21111111   211 1  11111111
11111111111111232-1311111111113123123111111112   1   11111111111
1111111111111111   111111111111111121111111112   1   11111111111
1111111111212112   2111111111111111111111111232-1111111111111111
1111111111         111111111111111111111111111   1  111111111111
1111111112         611111111111111111111111112   2 2111111111111
1111111112         111111111111111111111111111   111111111111111
1111111111   111   211111111111111111111111116   611111111111111
1111111112   112   111111111111111111111111112   111111111111111
1111111112   111111111111111111111111111111111   211111111111111
1111111113   3111112111111111111111111111111123-3111111111111111
1111111111   1111111111111111111111111111111111 1111111111111111
1111111112   1111111CCCCCCCC121612262111111111111218888888888888
1111111111   2111111CCCCCCCC11       111111111111189889889888988
111111CCCAC-CBCCC111CC    CC12       41111111111188            5
111111CC       CC111CC    CC11       11111111111189            9
111111CC       CCCCCCCACCCCC13       21111111111188            8
111111CA       ACCCCCC CCCCC12       11888888888889            5
111111CC       CCCBCCC CCCACC         8898998989988            8
111111CC       C            A         8           8            9
111111CC       |            |         |           | g          7
111111CC       C            A         9           8            8
111111CC       CCCBCCC CCCACC         89998988998988-8988-89   9
111111CA       ACCCCCC CCCCC21       18888888988888    8   8   5
111111CC       CCC11CCCCCC1123       22111111111188    8   8   9
111111CC       CCC11CCCCCC1121       42111111111188    8   9   8
111111CCCAC-CBCCCC111111111122       12111111111189    8   8   5
111111CCCC   CCCCC111111111121       221111111111889999889888888
111111CCCC   CC11111111111112126   62121111111111888888888888888
11111111CC   CC111111111111122299-882221111111111111111111111111
11111111CCCACCC11111111111111198   82111111111111111111111111111
11111111CCCCCCC11111111111111198   88888888888881111111111111111
11111111111111111111111111111198    8998989889881111111111111111
11111111111111111111111111111199              981111111111111111
11111111111111111111111111111198              881111111111111111
11111111111111111111111111111198    8989898   881111111111111111
11111111111111111111111111111198   988888888Z8881111111111111111
11111111111111111111111111111199   98899989   981111111111111111
11111111111111111111111111199999   8899998     81111111111111111
111111111111111111111111111988888Z989899988   881111111111111111
11111111111111111111111111198         9898     81111111111111111
11111111111111111111111111199         89989   881111111111111111
11111111111111111111111111198          998     81111111111111111
11111111111111111111111111198         99989   881111111111111111
11111111111111111111111111198         8998     81111111111111111
11111111111111111111111111199          9988   881111111111111111
11111111111111111111111111199         999898Z8981111111111111111
11111111111111111111111111199         89988888881111111111111111
111111111111111111111111111988989Z989899911111111111111111111111
1111111111111111111111111119999999999999911111111111111111111111
""";
        var splitMap = map.Split("\r\n").Reverse().Select(s => s.ToArray()).ToArray();
        var wallMap = new List<List<int>>();
        int x = 0;
        int y = 0;
        for (var lineNumber = 0; lineNumber < splitMap.Length; lineNumber++)
        {
            wallMap.Add(new());
            var line = splitMap[lineNumber];
            foreach (var lineItem in line)
            {
                if (char.IsNumber(lineItem) || char.IsUpper(lineItem))
                {
                    var wall = int.TryParse(lineItem.ToString(), NumberStyles.HexNumber, null, out var i) ? i : -1;
                    wallMap[lineNumber].Add(wall);
                } else
                {
                    wallMap[lineNumber].Add(-1);
                    var obj = lineItem switch
                    {
                        'c' => new Treasure(GetDef<TreasureDef>("chest"), x, y),
                        'i' => new RayGameObject(GetDef<RayGameObjectDef>("pillar"), x, y),
                        'a' => new RayGameObject(GetDef<RayGameObjectDef>("armor"), x, y),
                        'l' => new RayGameObject(GetDef<RayGameObjectDef>("ceiling-light"), x, y),
                        '#' => new Weapon(GetDef<WeaponDef>("machine-gun"), x, y),
                        '$' => new Weapon(GetDef<WeaponDef>("chain-gun"), x, y),
                        '%' => new Weapon(GetDef<WeaponDef>("rocket-launcher"), x, y),
                        'g' => new Actor(GetDef<ActorDef>("guard"), x, y),
                        '-' => new Door(GetDef<DoorDef>()!, x, y, true),
                        '|' => new Door(GetDef<DoorDef>()!, x, y, false),
                        _ => null
                    };

                    if (obj != null)
                    {
                        AddObject(obj);
                        if (obj is Actor a)
                            a.Initialize();
                    }
                }
                x++;
            }
            y++;
            x = 0;
        }

        RayMap.InitializeWalls(wallMap);
    }

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        DrawWeapon();
        DrawStatusBar();

        var fov = MathHelper.ToRadians(50);
        SpriteBatch.DrawString(Font, $"FPS: {FramesPerSecondCounter.FramesPerSecond}", new(2, 2), Color.Black);
        SpriteBatch.DrawString(Font, $"FPS: {FramesPerSecondCounter.FramesPerSecond}", new(3, 3), Color.DarkGray);

        SpriteBatch.End();
    }

    void DrawWeapon()
    {
        if (Player.CurrentWeapon != null)
            SpriteBatch.Draw(WeaponTexture,
                destinationRectangle: new Rectangle(240, ScreenHeight - 260, 160, 160),
                sourceRectangle: new Rectangle(Player.AttackFrame * 32, Player.CurrentWeapon.TextureRow * 32, 32, 32),
                Color.White);
    }

    void DrawStatusBar()
    {
        SpriteBatch.FillRect(new RectangleF(0, ScreenHeight - 100, ScreenWidth, 100), Color.DarkBlue, Color.DarkSlateBlue);
    }
}