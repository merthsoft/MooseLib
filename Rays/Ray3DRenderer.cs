using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.ThreeD;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.Rays.Actors;

namespace Merthsoft.Moose.Rays;
public class Ray3DRenderer : GraphicsDevice3DTriangleListTextureMapRenderer
{
    private const int TextureSize = RayGame.TextureSize;

    private static Vector3[] Vectors = new Vector3[4];

    protected RayPlayer Player = null!;
    protected float TextureWidth = 0;
    public int WallCount = 0;
    public int DoorCount = 0;

    public Ray3DRenderer(GraphicsDevice graphics, BasicEffect effect) : base(graphics, effect)
    {
    }

    public override void Update(MooseGame game, GameTime gameTime, IMap iMap)
    {
        VertexBufferIndex = 0;
        IndexBufferIndex = 0;
        PrimitiveCount = 0;
        TextureWidth = RayGame.Instance.TextureAtlas.Width;
        Player ??= RayGame.Instance.Player;
        WallCount = RayGame.Instance.WallCount;
        DoorCount = RayGame.Instance.DoorCount;

        //VertexBuffer.Clear();
        //IndexBuffer.Clear();
        //PrimitiveCount = 0;

        var map = (iMap as RayMap)!;
        var floorLayer = map.FloorLayer;
        var ceilingLayer = map.CeilingLayer;
        var wallLayer = map.WallLayer;
        var objectLayer = map.ObjectLayer;

        for (var x = -1; x <= wallLayer.Width; x++)
            for (var y = -1; y <= wallLayer.Height; y++)
            {
                var wall = wallLayer.GetTileValue(x, y);
                if (wall <= 0)
                {
                    var floor = floorLayer.GetTileValue(x, y);
                    var ceiling = ceilingLayer.GetTileValue(x, y);
                    CreateWall(x * TextureSize, y * TextureSize, ceiling, 4);
                    CreateWall(x * TextureSize, y * TextureSize, ceiling, 5);
                    continue;
                }
                var neighbors = 0;
                if (x > 0)
                    neighbors += (wallLayer.GetTileValue(x - 1, y) > 0) ? 0b0001 : 0;
                if (y < wallLayer.Height)
                    neighbors += (wallLayer.GetTileValue(x, y + 1) > 0) ? 0b0010 : 0;
                if (x < wallLayer.Width)
                    neighbors += (wallLayer.GetTileValue(x + 1, y) > 0) ? 0b0100 : 0;
                if (y > 0)
                    neighbors += (wallLayer.GetTileValue(x, y - 1) > 0) ? 0b1000 : 0;

                if (neighbors == 0)
                {
                    CreateWalls(x * TextureSize, y * TextureSize, wall);
                    continue;
                }

                var direction = neighbors switch
                {
                    0b0011 => 6,
                    0b1001 => 7,
                    0b1100 => 8,
                    0b0110 => 9,
                    _ => -1,
                };

                if (direction == -1)
                    CreateWalls(x * TextureSize, y * TextureSize, wall);
                else
                {
                    CreateWall(x * TextureSize, y * TextureSize, wall, direction);
                    CreateWall(x * TextureSize, y * TextureSize, 0, 4);
                    CreateWall(x * TextureSize, y * TextureSize, 0, 5);
                }
            }

        foreach (var obj in objectLayer.Objects.Cast<RayGameObject>().OrderByDescending(o => o.DistanceSquaredTo(Player)))
        {
            if (obj is RayPlayer)
                continue;

            var (x, y) = obj.Position;
            switch (obj.ObjectRenderMode)
            {
                case ObjectRenderMode.Sprite:
                case ObjectRenderMode.Directional:
                    CreateSprite(x, y, obj);
                    break;
                case ObjectRenderMode.Wall:
                    CreateWalls(x - 8, y - 8, obj.TextureIndex + obj.TextureIndexOffset);
                    break;
                case ObjectRenderMode.Overlay:
                    CreateWalls(x - 8, y - 8, WallCount + DoorCount + obj.TextureIndex + obj.TextureIndexOffset);
                    break;
                case ObjectRenderMode.Door:
                    CreateDoor(x, y, (obj as Door)!);
                    break;
            }
        }
    }

    private void CreateDoor(float x, float y, Door door)
{
        x -= 8;
        y -= 8;
        if (door.Horizontal)
        {
            CreateWall(x - TextureSize * door.OpenPercent, y + 8, door.TextureIndex, 0);
            CreateWall(x - .01f, y, 57, 1);
            CreateWall(x + .01f, y, 57, 3);
        }
        else
        {
            CreateWall(x + 8, y - TextureSize * door.OpenPercent, door.TextureIndex, 3);
            CreateWall(x, y + .01f, 57, 0);
            CreateWall(x, y - .01f, 57, 2);
        }
    }

    private void CreateWalls(float x, float y, int wall)
    {
        CreateWall(x, y, wall, 0);
        CreateWall(x, y, wall, 1);
        CreateWall(x, y, wall, 2);
        CreateWall(x, y, wall, 3);
        CreateWall(x, y, wall, 4);
        CreateWall(x, y, wall, 5);
    }

    private void CreateSprite(float x, float y, RayGameObject obj)
    {
        var textureIndex = WallCount + DoorCount + obj.TextureIndex + obj.TextureIndexOffset;

        if (obj.ObjectRenderMode == ObjectRenderMode.Directional)
        {
            var frames = (obj as Actor)?.CurrentState?.Count ?? 1;
            var objectRotation = (obj.FacingDirection.Atan2() - (Player.Position - obj.Position).Atan2())
                .ToDegrees().CardinalDirection8IndexDegrees();

            textureIndex += frames * objectRotation;
        }

        var drawOffset = obj is Actor || obj.RayGameObjectDef.Type != Serialization.ObjectType.Pickup
            ? 8 : 4;
        var drawBottom = obj.YDraw - (drawOffset - 1);
        var drawTop = obj.YDraw + 9;

        Vectors[0] = new Vector3(-drawOffset, 0, drawBottom);
        Vectors[1] = new Vector3(-drawOffset, 0, drawTop);
        Vectors[2] = new Vector3(drawOffset, 0, drawTop);
        Vectors[3] = new Vector3(drawOffset, 0, drawBottom); 
        
        var xStart = (textureIndex * TextureSize) / TextureWidth;
        var xEnd = ((textureIndex + 1) * TextureSize) / TextureWidth;
        var yStart = 0;
        var yEnd = 1;

        var radians = MathF.Atan2(obj.Position.Y - Player.Position.Y, obj.Position.X - Player.Position.X);
        var rot = Matrix.CreateRotationZ(MathF.PI / 2 + radians);
        for (var i = 0; i < Vectors.Length; i++)
            Vectors[i] = Vector3.Transform(Vectors[i], rot) + new Vector3(x, y, 0);

        AddQuad(Vectors, xStart, xEnd, yStart, yEnd, Color.White);
    }

    private void CreateWall(float x, float y, int wall, int direction, Color? color = null)
    {
        switch (direction)
        {
            case 0:
                Vectors[0] = new Vector3(x, y, 0);
                Vectors[1] = new Vector3(x, y, TextureSize);
                Vectors[2] = new Vector3(x + TextureSize, y, TextureSize);
                Vectors[3] = new Vector3(x + TextureSize, y, 0);
                break;
            case 1:
                Vectors[0] = new Vector3(x + TextureSize, y, 0);
                Vectors[1] = new Vector3(x + TextureSize, y, TextureSize);
                Vectors[2] = new Vector3(x + TextureSize, y + TextureSize, TextureSize);
                Vectors[3] = new Vector3(x + TextureSize, y + TextureSize, 0);
                break;
            case 2:
                Vectors[0] = new Vector3(x + TextureSize, y + TextureSize, 0);
                Vectors[1] = new Vector3(x + TextureSize, y + TextureSize, TextureSize);
                Vectors[2] = new Vector3(x, y + TextureSize, TextureSize);
                Vectors[3] = new Vector3(x, y + TextureSize, 0);
                break;
            case 3:
                Vectors[0] = new Vector3(x, y, 0);
                Vectors[1] = new Vector3(x, y, TextureSize);
                Vectors[2] = new Vector3(x, y + TextureSize, TextureSize);
                Vectors[3] = new Vector3(x, y + TextureSize, 0);
                break;
            case 4:
                Vectors[0] = new Vector3(x, y, 0);
                Vectors[1] = new Vector3(x + TextureSize, y, 0);
                Vectors[2] = new Vector3(x + TextureSize, y + TextureSize, 0);
                Vectors[3] = new Vector3(x, y + TextureSize, 0);
                break;
            case 5:
                Vectors[0] = new Vector3(x, y, TextureSize);
                Vectors[1] = new Vector3(x + TextureSize, y, TextureSize);
                Vectors[2] = new Vector3(x + TextureSize, y + TextureSize, TextureSize);
                Vectors[3] = new Vector3(x, y + TextureSize, TextureSize);
                break;
            case 6:
            case 8:
                Vectors[0] = new Vector3(x, y, 0);
                Vectors[1] = new Vector3(x, y, TextureSize);
                Vectors[2] = new Vector3(x + TextureSize, y + TextureSize, TextureSize);
                Vectors[3] = new Vector3(x + TextureSize, y + TextureSize, 0);
                break;
            case 7:
            case 9:
                Vectors[0] = new Vector3(x + TextureSize, y, 0);
                Vectors[1] = new Vector3(x + TextureSize, y, TextureSize);
                Vectors[2] = new Vector3(x, y + TextureSize, TextureSize);
                Vectors[3] = new Vector3(x, y + TextureSize, 0);
                break;
        }

        var xEnd = 
            (wall * TextureSize) / TextureWidth;
        var xStart = 
            ((wall + 1) * TextureSize) / TextureWidth;
        var yStart = 0;
        var yEnd = 1;

        var c = color ?? (direction is 1 or 3 or 8 or 9
            ? new Color(170, 170, 170) 
            : direction is 4 
                ? new Color(125, 125, 170)
                : direction is 5 
                    ? new Color(70, 70, 70)
                    : Color.White);
        
        AddQuad(Vectors, xStart, xEnd, yStart, yEnd, c);
    }
}
