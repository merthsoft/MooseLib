namespace Merthsoft.Moose.Dungeon;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using var game = new DungeonGame();
        game.Run();
    }
}
