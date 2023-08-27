using System.Runtime.CompilerServices;

namespace Merthsoft.Moose.MooseEngine.Topologies;
public static class TopologyHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Cycle(int n, int c) 
        => ((n) + (c)) % (c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ShiftBool(this bool b, int amount)
        => b ? 1 << amount : 0;

    public static Vector2 TranslateVector(this Vector2 vector, Topology toTopology, int mapWidth, int mapHeight)
        => TranslatePoint((int)vector.X, (int)vector.Y, toTopology, mapWidth, mapHeight).ToVector2();

    public static Point TranslatePoint(this Point p, Topology toTopology, int mapWidth, int mapHeight)
        => TranslatePoint(p.X, p.Y, toTopology, mapWidth, mapHeight);

    public static Point TranslatePoint(int c, int r, Topology toTopology, int mapWidth, int mapHeight)
    {
        var lC = 0;
        var lR = 0;

        var lE = c <= -1;
        var tEE = r <= -1;
        var rE = c >= mapWidth;
        var bE = r >= mapHeight;
        var ovC = lE || rE;
        var ovR = tEE || bE;

        if (!ovC && !ovR || toTopology == Topology.Plane)
            return new(c, r);

        switch (toTopology)
        {
            case Topology.Plane:
                lC = c; lR = r;
                break;
            case Topology.Ring:
                lC = ovC ? Cycle(c - 1, mapWidth) + 1 : c;
                lR = r;
                break;
            case Topology.Mobius:
                lC = ovC ? Cycle(c - 1, mapWidth) + 1 : c;
                lR = ovC ? mapHeight - r + 2 : r;
                break;
            case Topology.Torus:
                lC = ovC ? Cycle(c, mapWidth) : c;
                lR = ovR ? Cycle(r, mapHeight) : r;
                break;
            case Topology.Sphere:
                {
                    /* This only makes sense if the board is square. Otherwise, all hell will break loose */
                    var mask = lE.ShiftBool(3) | rE.ShiftBool(2) | tEE.ShiftBool(1) | (bE ? 1 : 0);
                    switch (mask)
                    {
                        case 0xA:
                        case 0x5:
                            return Point.Zero;
                        case 0x9:
                            lC = mapWidth;
                            lR = 1;
                            break;
                        case 0x8:
                            lC = r;
                            lR = 1;
                            break;
                        case 0x6:
                            lC = 1;
                            lR = mapHeight;
                            break;
                        case 0x4:
                            lC = r;
                            lR = mapHeight;
                            break;
                        case 0x2:
                            lC = 1;
                            lR = c;
                            break;
                        case 0x1:
                            lC = mapWidth;
                            lR = c;
                            break;
                        default:
                            lC = c;
                            lR = r;
                            break;
                    }
                    break;
                }
            case Topology.Klein:
                lC = ovC ? Cycle(c - 1, mapWidth) + 1 : c;
                lR = ovR ? Cycle(r - 1, mapHeight) + 1 : r;
                lR = ovC ? mapHeight - lR + 2 : lR;
                break;
            case Topology.Proj:
                lC = ovC ? Cycle(c - 1, mapWidth) + 1 : c;
                lC = ovR ? mapWidth - lC + 2 : lC;
                lR = ovR ? Cycle(r - 1, mapHeight) + 1 : r;
                lR = ovC ? mapHeight - lR + 2 : lR;
                break;
        }
        return new(lC, lR);
    }

}
