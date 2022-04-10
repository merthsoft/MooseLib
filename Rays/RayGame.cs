using System.Globalization;

namespace Merthsoft.Moose.Rays;

public class RayGame : MooseGame
{
    public static new RayGame Instance = null!;
    public int ViewWidth => (int)(4f * ScreenHeight / 3f);
    public int ViewHeight => ScreenHeight;

    public RayMap RayMap = null!;
    public List<RayTexture> Walls = new();
    public List<RayTexture> DarkWalls = new();

    public Vector3 CamTarget;
    public Vector3 CamPosition;

    public RayGame()
    {
        Instance = this;
    }

    protected override void Load()
    {
        DefaultBackgroundColor = Color.Black;
        CamTarget = new Vector3(23.375f, -101.75f, -1);
        CamPosition = new Vector3(82, -40, -70);

        var map = @"
11111111111111111111
1       1 1        1
1         7        1
1       1 1        1
111111111 1111111111
1       4          1
1       4          1
1       4 5        1
1       4 5        1
1       4 5        1
1         5        1
1         5        1
144444444 555 5555 1
1                  1
1                  1
1                  1
111111111122222222 1
1         2        1
1         2        1
1                  1
1         2        1
11111111111111111111";
        var splitMap = map.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToArray()).ToArray();
        var wallMap = new List<List<int>>();
        for (var lineNumber = 0; lineNumber < splitMap.Length; lineNumber++)
        {
            wallMap.Add(new());
            var line = splitMap[lineNumber];
            foreach (var lineItem in line)
            {
                var c = lineItem;
                var wall = int.TryParse(c.ToString(), NumberStyles.HexNumber, null, out var i) ? i : -1;
                wallMap[lineNumber].Add(wall);
            }
        }

        RayMap = new(wallMap);
        ActiveMaps.Add(RayMap);

        var effect = new BasicEffect(GraphicsDevice);
        effect.Alpha = 1;
        effect.TextureEnabled = true;
        effect.Texture = ContentManager.LoadImage("Walls");
        
        AddRenderer("walls", new Ray3DRenderer(GraphicsDevice, effect));
        
        AddObject(new RayPlayer(new MooseEngine.Defs.GameObjectDef("player"), new(32, 96), Directions.East));
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        base.PreUpdate(gameTime);

        if (IsKeyDown(Keys.Right, Keys.D))
        {
            CamPosition.X -= 1f;
        }
        if (IsKeyDown(Keys.Left, Keys.A))
        {
            CamPosition.X += 1f;
        }
        if (IsKeyDown(Keys.Down, Keys.S))
        {
            CamPosition.Y -= 1f;
        }
        if (IsKeyDown(Keys.Up, Keys.W))
        {
            CamPosition.Y += 1f;
        }
        if (IsKeyDown(Keys.OemPlus))
        {
            CamPosition.Z += 1f;
        }
        if (IsKeyDown(Keys.OemMinus))
        {
            CamPosition.Z -= 1f;
        }
    }
}
