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
        CamTarget = new Vector3(0f, 0f, 0f);
        CamPosition = new Vector3(0f, 0f, -100f);

        var map = @"
00000000000000000000
0       1 1        0
0         7        0
0       1 1        0
011111111 1111111110
0       4          0
0       4          0
0       4 5        0
0       4 5        0
0       4 5        0
0         5        0
0         5        0
044444444 555 5555 0
0                  0
0                  0
0                  0
011111111122222222 0
0         2        0
0         2        0
0                  0
0         2        0
00000000000000000000";
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

        var wallImage = ContentManager.LoadImage("Walls");
        for (var i = 0; i < wallImage.Width; i += 32)
        {
            var wallData = new uint[1024];
            wallImage.GetData(0, new(i, 0, 32, 32), wallData, 0, 1024);
            
            var rayTexture = new RayTexture(32, 32);
            for (int x = 0; x < 32; x++)
                for (int y = 0; y < 32; y++)
                    rayTexture.Add(wallData[x * 32 + y]);

            Walls.Add(rayTexture);
            DarkWalls.Add(rayTexture.GenerateNorthWall());
        }

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

        if (IsKeyDown(Keys.Right))
        {
            CamPosition.X -= 1f;
            CamTarget.X -= 1f;
        }
        if (IsKeyDown(Keys.Left))
        {
            CamPosition.X += 1f;
            CamTarget.X += 1f;
        }
        if (IsKeyDown(Keys.Down))
        {
            CamPosition.Y -= 1f;
            CamTarget.Y -= 1f;
        }
        if (IsKeyDown(Keys.Up))
        {
            CamPosition.Y += 1f;
            CamTarget.Y += 1f;
        }
        if (IsKeyDown(Keys.OemPlus))
        {
            CamPosition.Z += 1f;
        }
        if (IsKeyDown(Keys.OemMinus))
        {
            CamPosition.Z -= 1f;
        }

        if (IsKeyDown(Keys.Z))
        {
            var rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
            CamPosition = Vector3.Transform(CamPosition, rotationMatrix);
            CamTarget = Vector3.Transform(CamTarget, rotationMatrix);
        }
        if (IsKeyDown(Keys.X))
        {
            var rotationMatrix = Matrix.CreateRotationY(-MathHelper.ToRadians(1f));
            CamPosition = Vector3.Transform(CamPosition, rotationMatrix);
        }
    }
}
